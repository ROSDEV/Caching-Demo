using CommonCachingDomain;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCoreCaching
{
    public class SimpleCustomerMemoryCacheWithRequestLocking : ICustomerService
    {
        private readonly ICustomerService _actualService;
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
        public SimpleCustomerMemoryCacheWithRequestLocking(ICustomerService actualService, IMemoryCache memoryCache)
        {
            if (actualService == null) { throw new ArgumentNullException(nameof(actualService)); }
            _actualService = actualService;
            _memoryCache = memoryCache;
        }

        public string Name => nameof(SimpleCustomerMemoryCacheWithoutLocking);
        public int CreatedCount => _actualService.CreatedCount;

        private static int _secondAttemptCounter = 0;
        public string SecondAttempt => $"Second attempt succeeded after locking in SimpleCustomerMemoryCacheWithRequestLocking => {_secondAttemptCounter}";

        public async Task<Customer> GetOrAddCustomer(Guid customerId)
        {
            if (_memoryCache.TryGetValue(customerId, out var cacheEntry))
            {
                return cacheEntry as Customer;
            }

            var slimLocker = _locks.GetOrAdd(customerId, k => new SemaphoreSlim(1, 1));
            await slimLocker.WaitAsync();
            try
            {
                if (_memoryCache.TryGetValue(customerId, out var cacheSecondAttempt))
                {
                    Interlocked.Increment(ref _secondAttemptCounter);                    
                    return cacheSecondAttempt as Customer;

                }
                var customer = await _actualService.GetOrAddCustomer(customerId);
                _memoryCache.Set(customerId, customer);
                return customer as Customer;
            }
            finally
            {
                slimLocker.Release();
            }
        }
    }
}
