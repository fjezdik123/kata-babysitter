using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace PayoutCalcApp.Infrastructure
{
    public class CacheManager
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        private static readonly CacheItemPolicy Policy =
            new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.MaxValue};

        public static void SetCache<T>(T itemToAdd, string key)
        {
            Cache.Set(key,itemToAdd,Policy);
        }

        public static object GetCache(string key)
        {
            return Cache.Get(key);
        }

        public static void RemoveCache(string key)
        {
            Cache.Remove(key);
        }
    }
}