using CommonCachingDomain;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCoreCaching
{
    public class SimpleCustomerMemoryCacheWithEvictionPolicy : ICustomerService
    {
        private readonly ICustomerService _actualService;
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
        public SimpleCustomerMemoryCacheWithEvictionPolicy(ICustomerService actualService, IMemoryCache memoryCache)
        {
            if (actualService == null) { throw new ArgumentNullException(nameof(actualService)); }
            _actualService = actualService;
            _memoryCache = memoryCache;
        }

        public string Name => nameof(SimpleCustomerMemoryCacheWithEvictionPolicy);
        public int CreatedCount => _actualService.CreatedCount;

        private static int _secondAttemptCounter = 0;
        public string SecondAttempt => $"Second attempt succeeded after locking in SimpleCustomerMemoryCacheWithEvictionPolicy => {_secondAttemptCounter}";

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
                var policy = CreatePolicy();
                _memoryCache.Set(customerId, customer, policy);
                return customer as Customer;
            }
            finally
            {
                slimLocker.Release();
            }
        }

        private MemoryCacheEntryOptions CreatePolicy()
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                //Size amount
                .SetSize(1)
                //Priority on removing when reaching size limit (memory pressure)
                .SetPriority(CacheItemPriority.High)
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(1))
                // Remove from cache after this time, regardless of sliding expiration
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(2))
                //Register a callback function
                .RegisterPostEvictionCallback((k,v,r,s) => LogEviction(k, v, r, s));
            
            return cacheEntryOptions;
        }

        private void LogEviction(object key, object value, EvictionReason reason, object state)
        {
            Console.WriteLine($"Eviction called for key {key} => evicted because {reason}");
        }
    }
}
