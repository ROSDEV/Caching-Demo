using System;
using System.Collections.Generic;

namespace CommonCachingDomain
{
    public class GuidGenerator : IGuidGenerator
    {
        public List<Guid> GenerateGuids(int count, bool same = false)
        {
            var collection = new List<Guid>();
            var guid = Guid.NewGuid();
            for (int i = 0; i < count; i++)
            {
                if (!same)
                {
                    guid = Guid.NewGuid();
                }
                collection.Add(guid);
            }

            return collection;
        }
    }
}
