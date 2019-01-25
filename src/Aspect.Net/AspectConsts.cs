using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Aspect.Net
{
    public static class AspectConsts
    {
        public const string AssemblyName = "Aspect.Net";

        public const string ModuleName = nameof(Proxy);

        private static readonly string TypeNamespace = string.Join(".", AssemblyName, ModuleName, "Generated");

        public const MethodAttributes OverrideMethodAttributes = MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;

        public const TypeAttributes DefaultClassAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed;

        public const BindingFlags DefaultMethodBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public const MethodAttributes InternalMethodAttributes = MethodAttributes.Private;

        public static string GetTypeName(string typeName)
        {
            return string.Join(",", TypeNamespace, typeName);
        }

        public static readonly IEnumerable<string> ExcludeMethods = new[]
        {
            "ToString" ,"Equals" ,"GetHashCode" ,"GetType","Finalize","MemberwiseClone"
        };

        public static string GetProxyMethodName(string methodName)
        {
            return Guid.NewGuid().ToString("N");
        }

        private static readonly Expression<Func<RuntimeMethodHandle, MethodBase>> GetHandleMethodFunc = handle => MethodBase.GetMethodFromHandle(handle);

        public static readonly MethodInfo GetHandleMethod = (GetHandleMethodFunc.Body as MethodCallExpression)?.Method;
    }
}
