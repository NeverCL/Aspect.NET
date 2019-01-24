using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aspect.Net
{
    public class DefaultAspect : IAspect
    {
        public Task InvokeAsync(AspectContext context)
        {
            throw new NotImplementedException();
        }
    }
}
