using CommonCachingDomain;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCoreCaching
{
    public class SimpleCustomerMemoryCacheWithLocking : ICustomerService
    {
        private readonly ICustomerService _actualService;
        private readonly IMemoryCache _memoryCache;
        public SimpleCustomerMemoryCacheWithLocking(ICustomerService actualService, IMemoryCache memoryCache)
        {
            if (actualService == null) { throw new ArgumentNullException(nameof(actualService)); }
            _actualService = actualService;
            _memoryCache = memoryCache;
        }

        public string Name => nameof(SimpleCustomerMemoryCacheWithLocking);
        public int CreatedCount => _actualService.CreatedCount;

        private static int _secondAttemptCounter = 0;
        public string SecondAttempt => $"Second attempt succeeded after locking {_secondAttemptCounter}";

        public async Task<Customer> GetOrAddCustomer(Guid customerId)
        {
            if (_memoryCache.TryGetValue(customerId, out var cacheEntry)) 
            {
                return cacheEntry as Customer;
            }
            
            var customer = await _actualService.GetOrAddCustomer(customerId);
            lock (_memoryCache)
            {
                if (_memoryCache.TryGetValue(customerId, out var cacheSecondAttempt))
                {
                    Interlocked.Increment(ref _secondAttemptCounter);
                    return cacheSecondAttempt as Customer;
                    
                }
                _memoryCache.Set(customerId, customer);
            }   
            
            return customer as Customer;
        }
    }
}
