using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace PayoutCalcApp.Infrastructure
{
    public class CacheManager : ICacheService
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        private static readonly CacheItemPolicy Policy =
            new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.MaxValue };

        public void SetCache<T>(T itemToAdd, string key)
        {
            Cache.Set(key, itemToAdd, Policy);
        }

        public object GetCache(string key)
        {
            return Cache.Get(key);
        }

        public void RemoveCache(string key)
        {
            Cache.Remove(key);
        }
    }
    public interface ICacheService
    {
        void SetCache<T>(T itemToAdd, string key);
        object GetCache(string key);
        void RemoveCache(string key);
    }
}