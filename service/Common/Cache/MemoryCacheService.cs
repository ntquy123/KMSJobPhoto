using System;
using System.Collections.Generic;
using System.Text;
using erpsolution.service.Common.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace erpsolution.service.Common.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        private int _defaultCacheTime = 60;//minutes
        public void Add<T>(T o, string key)
        {
            Add(o, key, _defaultCacheTime);
        }

        public void Add<T>(T o, string key, int minutes)
        {
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromMinutes(minutes));
                //.SetSize(1);
            
            // Save data in cache.
            _memoryCache.Set(key, o, cacheEntryOptions);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public bool Get<T>(string key, out T value)
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out value))
                {
                    return true;
                }
                value = default(T);
                return false;
            }
            catch(Exception e)
            {
                value = default(T);
                throw e;
            }
        }
        public T Get<T>(string key)
        {
            try
            {
               return _memoryCache.Get<T>(key);
               
            }
            //catch (Exception e)
            catch
            {               
                return default(T);
//                throw e;
            }
        }
    }
}
