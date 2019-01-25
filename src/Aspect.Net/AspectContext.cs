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
            ReturnType = ProxyMethod.ReturnType;
        }
        public object[] Arguments { get; set; }

        public MethodInfo ProxyMethod { get; }

        public object Instance { get; }

        public object ReturnValue { get; set; }

        public Type ReturnType { get; }

        public async Task InvokeAsync()
        {
            var method = ProxyMethod;
            var arguments = Arguments;
            var instance = Instance;
            var del = method.BuildDynamicDelegate(instance);
            if (ReturnType == typeof(Task))
            {
                await (Task)del.DynamicInvoke(arguments);
            }
            else if (ReturnType.IsGenericType && ReturnType.BaseType == typeof(Task))
            {
                ReturnValue = await (dynamic)del.DynamicInvoke(arguments);
            }
            else
            {
                ReturnValue = del.DynamicInvoke(arguments);
            }
        }
    }
}
