using System;
using System.Threading.Tasks;

namespace CommonCachingDomain
{
    public interface ICustomerService
    {
        Task<Customer> GetOrAddCustomer(Guid customerId);
        
        int CreatedCount { get; }

        string SecondAttempt { get; }

        string Name { get; }

    }
}