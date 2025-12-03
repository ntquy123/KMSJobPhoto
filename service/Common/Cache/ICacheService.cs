using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.service.Common.Cache
{
    public interface ICacheService
    {
        /// <summary>
        /// Insert value into the cache using
        /// appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="o">Item to be cached</param>
        /// <param name="key">Name of item</param>
        void Add<T>(T o, string key);
        void Add<T>(T o, string key, int minutes);
        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        void Remove(string key);
        /// <summary>
        /// Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of cached item</param>
        /// <param name="value">Cached value. Default(T) if item doesn't exist.</param>
        /// <returns>Cached item as type</returns>
        bool Get<T>(string key, out T value);

        T Get<T>(string key);
    }
}
