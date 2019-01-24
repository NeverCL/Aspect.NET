using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aspect.Net.Extensions
{
    public static class MethodExtensions
    {
        public static Type[] GetParameterTypes(this MethodInfo method)
        {
            return method.GetParameters().Select(x => x.ParameterType).ToArray();
        }
    }
}
