using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommonCachingDomain
{
    public class CustomerService : ICustomerService
    {
        public int CreatedCount => _counter;
        private static int _counter = 0; 
        public string SecondAttempt => "No second attempt for the actual customer";

        public string Name => nameof(CustomerService);

        public async Task<Customer> GetOrAddCustomer(Guid customerId)
        {   
            Interlocked.Increment(ref _counter);
            return await Task.FromResult(Customer.Create(customerId));
        }
    }
}
