using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            //var method = Instance.GetType().GetMethod(ProxyMethod.Name + "_Proxy", BindingFlags.NonPublic | BindingFlags.Instance);
            var method = ProxyMethod;
            var arguments = Arguments;
            var instance = Instance;
            if (method.ReturnType == typeof(Task))
            {
                //taskFunc = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), instance, method);
                //taskFunc = method.CreateDelegate(typeof(Func<Task>),null);
                //await taskFunc();
                await (Task)method.Invoke(instance, arguments);
            }
            else
            {
                if (arguments.Any())
                {
                    method.Invoke(instance, arguments);
                }
                else
                {
                    ReturnValue = method.Invoke(instance, null);
                }
            }
        }
    }
}
