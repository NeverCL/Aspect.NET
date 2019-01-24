using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Aspect.Net
{
    public class AspectConsts
    {
        public const string AssemblyName = "As";
        public const string ModuleName = "As";

        public const MethodAttributes OverrideMethodAttributes = MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;

        public const TypeAttributes DefaultClassAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed;

        public const BindingFlags DefaultMethodBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public const MethodAttributes InternalMethodAttributes = MethodAttributes.Private;

        public static string GetTypeName(string typeName)
        {
            return typeName + "_Proxy";
        }

        public static readonly IEnumerable<string> ExcludeMethods = new[]
        {
            "ToString" ,"Equals" ,"GetHashCode" ,"GetType","Finalize","MemberwiseClone"
        };

        public static string GetProxyMethodName(string methodName)
        {
            return methodName + Guid.NewGuid().ToString("N");
        }


        internal static Expression<Func<RuntimeMethodHandle, MethodBase>> GetHandleMethodFunc = handle => MethodBase.GetMethodFromHandle(handle);

        public static readonly MethodInfo GetHandleMethod = (GetHandleMethodFunc.Body as MethodCallExpression)?.Method;
    }
}
