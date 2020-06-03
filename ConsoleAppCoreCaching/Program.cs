using CommonCachingDomain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppCoreCaching
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }               

        static async Task MainAsync()
        {
            var provider = new ServiceCollection()
                .AddMemoryCache()
                .AddSingleton<IGuidGenerator, GuidGenerator>()
                .AddSingleton<ICustomerService, CustomerService>()
                //.Decorate<ICustomerService, SimpleCustomerNotThreadSafeCustomCaching>()       //using a Dictionary                
                //.Decorate<ICustomerService, SimpleCustomerMemoryCacheWithLocking>()           //using a IMemoryCache with     locking the MemoryCache
                //.Decorate<ICustomerService, SimpleCustomerMemoryCacheWithoutLocking>()        //using a IMemoryCache without  locking the MemoryCache
                //.Decorate<ICustomerService, SimpleCustomerMemoryCacheWithRequestLocking>()    //using a IMemoryCache with     locking the requestKey
                //.Decorate<ICustomerService, SimpleCustomerMemoryCacheWithEvictionPolicy>()    //using a IMemoryCache with     locking the requestKey and eviction
                .BuildServiceProvider();


            var service = provider.GetService<ICustomerService>();
            var generator = provider.GetService<IGuidGenerator>();
            var guids = generator.GenerateGuids(100);

            var threads = StartThread(15, async () => 
            {
                await GetCustomer1Time(service, guids);               
            });
            threads.ForEach(x => x.Join());

            
            Console.WriteLine($"Done!");
            Console.WriteLine($"Done by {service.Name}");
            Console.WriteLine($"Created => {service.CreatedCount}");
            Console.WriteLine($"SecondAttemptRetrieval => {service.SecondAttempt}");
            Console.ReadKey();
        }

        private static List<Thread> StartThread(int count, Action action)
        {
            var threads = new List<Thread>();
            for (int i = 0; i < count; i++)
            {   
                var thread = new Thread(() => action());
                Console.WriteLine($"Starting new thread {thread.ManagedThreadId}");
                thread.Start();
                threads.Add(thread);
            }
            return threads;          
        }

        /// <summary>
        /// Get Customers 1 at a time
        /// </summary>
        /// <param name="service"></param>
        /// <param name="guidGenerator"></param>
        /// <returns></returns>
        private static async Task GetCustomer1Time(ICustomerService service, List<Guid> guids)
        {
            foreach (var id in guids)
            {
                await service.GetOrAddCustomer(id);                
            }
        }
    }
}
