using CommonCachingDomain;
using ConsoleAppCachingNetFramework;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace ConsoleAppCachingNetFramework
{
    public class SimpleCustomerMemoryCacheWithoutLocking : ICustomerService
    {
        private readonly ICustomerService _actualService;
        private readonly MemoryCache _memoryCache;
        public SimpleCustomerMemoryCacheWithoutLocking(ICustomerService actualService)
        {
            if (actualService == null) { throw new ArgumentNullException(nameof(actualService)); }
            _actualService = actualService;
            _memoryCache = MemoryCache.Default;
        }
        public string Name => nameof(SimpleCustomerMemoryCacheWithoutLocking);
        public int CreatedCount => _actualService.CreatedCount;

        private static int _secondAttemptCounter = 0;
        public string SecondAttempt => $"Second attempt counter for {nameof(SimpleCustomerMemoryCacheWithoutLocking)} {_secondAttemptCounter}";

        public async Task<Customer> GetOrAddCustomer(Guid customerId)
        {   
            if (_memoryCache.TryGetValue(customerId.ToString(), out Customer cacheEntry)) 
            {
                return cacheEntry;
            }                          
            var customer = await _actualService.GetOrAddCustomer(customerId);
            var result = _memoryCache.TryAddValue(customerId.ToString(), customer, DateTimeOffset.UtcNow.AddSeconds(5));
            return result;            
        }
    }
}
