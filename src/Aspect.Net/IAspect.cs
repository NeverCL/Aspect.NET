using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aspect.Net
{
    public interface IAspect
    {
        Task InvokeAsync(AspectContext context);
    }
}
