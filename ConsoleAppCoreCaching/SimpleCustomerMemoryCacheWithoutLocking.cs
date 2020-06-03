using CommonCachingDomain;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCoreCaching
{
    public class SimpleCustomerMemoryCacheWithoutLocking : ICustomerService
    {
        private readonly ICustomerService _actualService;
        private readonly IMemoryCache _memoryCache;
        public SimpleCustomerMemoryCacheWithoutLocking(ICustomerService actualService, IMemoryCache memoryCache)
        {
            if (actualService == null) { throw new ArgumentNullException(nameof(actualService)); }
            _actualService = actualService;
            _memoryCache = memoryCache;
        }
        public string Name => nameof(SimpleCustomerMemoryCacheWithoutLocking);
        public int CreatedCount => _actualService.CreatedCount;

        private static int _secondAttemptCounter = 0;
        public string SecondAttempt => $"Second attempt counter for {nameof(SimpleCustomerMemoryCacheWithoutLocking)} {_secondAttemptCounter}";

        public async Task<Customer> GetOrAddCustomer(Guid customerId)
        {   
            if (_memoryCache.TryGetValue(customerId, out var cacheEntry)) 
            {
                return cacheEntry as Customer;
            }
                          
            var customer = await _actualService.GetOrAddCustomer(customerId);
            if (_memoryCache.TryGetValue(customerId, out var secondAttempt))
            {
                Interlocked.Increment(ref _secondAttemptCounter);
                return secondAttempt as Customer;
            }

            _memoryCache.Set(customerId, customer);
            return customer;            
        }
    }
}
