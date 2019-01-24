using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Aspect.Net
{
    public class DefaultAspect : IAspect
    {
        private Func<AspectContext, Task> _innerFunc;
        public DefaultAspect(Func<AspectContext, Task> innerFunc = null)
        {
            _innerFunc = innerFunc;
        }

        public async Task InvokeAsync(AspectContext context)
        {
            Debug.WriteLine("start:" + DateTime.Now.ToLongTimeString());
            if (_innerFunc != null)
            {
                await _innerFunc(context);
            }
            Debug.WriteLine("end:" + DateTime.Now.ToLongTimeString());
        }
    }
}
