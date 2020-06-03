using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCachingNetFramework
{
    public static class CachingExtensions
    {
        public static bool TryGetValue<T>(this MemoryCache cache, string key, out T value)
        {            

            var cacheEntry = cache.Get(key);
            if (cacheEntry != null)
            {
                value = (T)cacheEntry;
                return true;
            }

            value = default(T);
            return false;
        }

        public static T TryAddValue<T>(this MemoryCache cache, string key, T value, DateTimeOffset offSet)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var result = cache.AddOrGetExisting(key, value, offSet);
            if (result != null) {
                return (T)result;
            }
            return value;
        }
    }
}
