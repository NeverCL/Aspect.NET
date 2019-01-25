using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aspect.Net.Extensions
{
    public static class MethodExtensions
    {
        public static Type[] GetParameterTypes(this MethodInfo method)
        {
            return method.GetParameters().Select(x => x.ParameterType).ToArray();
        }

        /// <summary>
        /// 动态构造委托
        /// </summary>
        /// <param name="methodInfo">方法元数据</param>
        /// <param name="instance">方法实例</param>
        /// <returns>委托</returns>
        public static Delegate BuildDynamicDelegate(this MethodInfo methodInfo, object instance)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            var paramExpressions = methodInfo.GetParameters().Select((p, i) =>
            {
                var name = "param" + (i + 1).ToString(CultureInfo.InvariantCulture);
                return Expression.Parameter(p.ParameterType, name);
            }).ToList();

            MethodCallExpression callExpression;
            if (methodInfo.IsStatic)
            {
                callExpression = Expression.Call(methodInfo, paramExpressions);
            }
            else if (IsTaskResult(methodInfo))
            {
                //async ()=> await 
                callExpression = Expression.Call(Expression.Constant(instance), methodInfo, paramExpressions);
            }
            else
            {
                callExpression = Expression.Call(Expression.Constant(instance), methodInfo, paramExpressions);
            }
            var lambdaExpression = Expression.Lambda(callExpression, paramExpressions);
            return lambdaExpression.Compile();
        }

        private static bool IsTaskResult(MethodInfo methodInfo)
        {
            return methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.BaseType == typeof(Task);
        }
    }
}
