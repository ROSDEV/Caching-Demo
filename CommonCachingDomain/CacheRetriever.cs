
using System;

namespace CommonCachingDomain
{
    public interface ICacheRetriever<TItem>
    {
        TItem GetOrCreate(object key, Func<TItem> createItem);
    }
}
