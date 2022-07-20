using Microsoft.Extensions.Caching.Memory;
using System;

namespace RestaurantListing.Services
{
    public class CachingProperties
    {
        private  MemoryCacheEntryOptions _cacheOptions;
        public MemoryCacheEntryOptions CacheOptions => _cacheOptions ??=
            new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(600))
                .SetPriority(CacheItemPriority.Normal)
                .SetSize(1024);
    }
}
