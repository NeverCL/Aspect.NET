using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aspect.Net.Extensions;

namespace Aspect.Net
{
    public class AspectContext
    {
        public AspectContext(object instance, MethodInfo proxyMethod, params object[] arguments)
        {
            ProxyMethod = proxyMethod;
            Instance = instance;
            Arguments = arguments;
        }
        public object[] Arguments { get; set; }

        public MethodInfo ProxyMethod { get; }

        public object Instance { get; }

        public object ReturnValue { get; set; }

        public async Task InvokeAsync()
        {
            var method = ProxyMethod;
            var arguments = Arguments;
            var instance = Instance;
            var del = method.BuildDynamicDelegate(instance);
            if (method.ReturnType == typeof(Task))
            {
                await (Task)del.DynamicInvoke(arguments);
            }
            else
            {
                ReturnValue = del.DynamicInvoke(arguments);
            }
        }
    }
}
