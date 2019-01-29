using System;
using System.Collections.Generic;
using System.Text;

namespace Aspect.Net.Cache
{
    public interface ICache
    {
        T GetOrAdd<T>(string key, Func<T> func);
    }
}
