using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Aspect.Net
{
    public class AspectConsts
    {
        public const string AssemblyName = "As";
        public const string ModuleName = "As";

        public const TypeAttributes DefaultClassAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed;

        public static string GetTypeName(string typeName)
        {
            return typeName + "_Proxy";
        }
    }
}
