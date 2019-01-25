using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspect.Net
{
    public class DefaultAspect : IAspect
    {
        private readonly Func<AspectContext, Task> _innerFunc;
        public DefaultAspect(Func<AspectContext, Task> innerFunc = null)
        {
            _innerFunc = innerFunc;
        }

        public async Task<T> InvokeAsync<T>(AspectContext context)
        {
            Debug.WriteLine("start:" + DateTime.Now.ToLongTimeString());
            if (_innerFunc != null) 
            {
                await _innerFunc(context);
            }
            else
            {
                await context.InvokeAsync();
            }

            if (context.ProxyMethod.ReturnType != typeof(void))
            {
                return (T)context.ReturnValue;
            }

            Debug.WriteLine("end:" + DateTime.Now.ToLongTimeString());
            return default(T);
        }
    }
}
