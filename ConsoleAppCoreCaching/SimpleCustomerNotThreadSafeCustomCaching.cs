using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonCachingDomain
{  
    public class SimpleCustomerNotThreadSafeCustomCaching : ICustomerService
    {
        private static Dictionary<Guid, Customer> _cache = new Dictionary<Guid, Customer>();

        private readonly ICustomerService _actualService;

        public string Name => nameof(SimpleCustomerNotThreadSafeCustomCaching);

        public int CreatedCount => _actualService.CreatedCount;

        public string SecondAttempt => $"No second attempt for {nameof(SimpleCustomerNotThreadSafeCustomCaching)}";

        public SimpleCustomerNotThreadSafeCustomCaching(ICustomerService actualService)
        {
            if (actualService == null) { throw new ArgumentNullException(nameof(actualService)); }
            _actualService = actualService;
        }
        public async Task<Customer> GetOrAddCustomer(Guid customerId)
        {
            if (_cache.TryGetValue(customerId, out var customer))
            {
                return customer;
            }
            
            try
            {
                var created = await _actualService.GetOrAddCustomer(customerId);                
                _cache.Add(customerId, created);
                return created;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured while add a customer to my cache: => {e.Message}");
                return null;
            }
        }
    }
}
