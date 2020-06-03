using System;

namespace CommonCachingDomain
{
    public class Customer
    {
        private Customer(Guid id)
        {
            CustomerId = id;
        }        
        
        public Guid CustomerId { get; }

        public static Customer Create(Guid id)
        {
            return new Customer(id);
        }
    }
}
