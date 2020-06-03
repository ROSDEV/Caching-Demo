using System;
using System.Collections.Generic;

namespace CommonCachingDomain
{
    public interface IGuidGenerator
    {
        List<Guid> GenerateGuids(int count, bool same = false);
    }
}