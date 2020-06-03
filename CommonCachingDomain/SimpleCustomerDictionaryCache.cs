using System;
using System.Collections.Generic;

namespace ConsoleAppCoreCaching
{
    public class SimpleCustomerDictionaryCache<TItem>//: ICacheRetriever<TItem>
    { 
        Dictionary<object, TItem> _cache = new Dictionary<object, TItem>();
    
        public TItem GetOrCreate(object key, Func<TItem> createItem)
        {
            if (!_cache.ContainsKey(key))
            {
                _cache[key] = createItem();
            }
            return _cache[key];
        }     
    }
}
