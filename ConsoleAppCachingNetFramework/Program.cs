using System;
using System.Runtime.Caching;
using System.Threading;

namespace ConsoleAppCachingNetFramework
{
    class Program
    {
        static void Main(string[] args)
        { 
            var existingKey = "2";
            var memCache = MemoryCache.Default;
            memCache.Add("2", 2,DateTime.UtcNow.AddSeconds(2));                                   

            if (memCache.Contains(existingKey))            
            {
                Console.WriteLine($"Key exists {existingKey}");
                Console.WriteLine("Sleeping for 2500 ms");
                Thread.Sleep(2500);

                var result2 = memCache.Get(existingKey);
                if (result2 == null) 
                {
                    Console.WriteLine($"Key {existingKey} does not exist anymore");
                }
            }
            Console.ReadKey();
        }
    }
}
