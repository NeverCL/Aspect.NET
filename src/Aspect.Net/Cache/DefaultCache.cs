using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Aspect.Net.Cache
{
    public class DefaultCache : ICache
    {
        private readonly ConcurrentDictionary<string, object> _dict = new ConcurrentDictionary<string, object>();

        public static ICache Instance = new DefaultCache();

        public T GetOrAdd<T>(string key, Func<T> func)
        {
            return (T)_dict.GetOrAdd(key, func());
        }
    }
}
