using BlogSitesi.WebUI.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace BlogSitesi.Infrastructure.Caching.Memory
{
    public class Cache : ICache
    {
        private IMemoryCache memoryCache;
        public Cache (IMemoryCache _memoryCache)
        {
            memoryCache = _memoryCache;
        }

        
        public bool TryGetValue(string key, out object value)
        {
            return memoryCache.TryGetValue(key, out value);
        }

        public void Set(string key, object value, int minutesToCache)
        {
            var cacheExOprtions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddDays(minutesToCache),
                Priority = CacheItemPriority.Normal
            };
            memoryCache.Set(key, value, cacheExOprtions);
        }

        public void Remove(string key)
        {
            memoryCache.Remove(key);
        }
    }
}
