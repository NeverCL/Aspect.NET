using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Aspect.Net.Extensions;

namespace Aspect.Net
{
    public class Proxy
    {
        private readonly IAspect _aspect;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;

        public Proxy(IAspect defaultAspect = null)
        {
            _aspect = defaultAspect;
        }

        public T Create<T>() where T : class, new()
        {
            if (_aspect == null)
            {
                return new T();
            }

            return CreateProxy<T>();
        }

        private T CreateProxy<T>() where T : class, new()
        {
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(AspectConsts.AssemblyName), AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(AspectConsts.ModuleName);
            var realType = typeof(T);
            var typeBuilder = DefineType(realType);
            var aspectField = typeBuilder.DefineField("_aspect", typeof(IAspect), FieldAttributes.Private);
            DefineConstructor(typeBuilder, aspectField);
            realType.GetMethods(AspectConsts.DefaultMethodBindingFlags)
                .Where(method => !AspectConsts.ExcludeMethods.Contains(method.Name))
                .Aggregate(typeBuilder, (builder, info) => DefineMethod(typeBuilder, info, aspectField));
            var proxyType = typeBuilder.CreateTypeInfo();
            var proxy = Activator.CreateInstance(proxyType, _aspect);
            return proxy as T;
        }

        private TypeBuilder DefineMethod(TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder aspectField)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            #region DefineRealProxy
            var proxyMethodBuilder = typeBuilder.DefineMethod(AspectConsts.GetProxyMethodName(methodInfo.Name),
                AspectConsts.InternalMethodAttributes,
                methodInfo.CallingConvention,
                methodInfo.ReturnType,
                methodInfo.GetParameterTypes());
            var ilGenerator = proxyMethodBuilder.GetILGenerator();
            var parameters = methodInfo.GetParameters();
            if (parameters.Any())
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
                for (int i = 0; i < parameters.Length; i++)
                {
                    ilGenerator.Emit(OpCodes.Ldarg, i + 1);
                }
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
            }

            ilGenerator.Emit(OpCodes.Call, methodInfo);

            ilGenerator.Emit(OpCodes.Ret);
            #endregion

            var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, AspectConsts.OverrideMethodAttributes,
                methodInfo.CallingConvention, methodInfo.ReturnType, methodInfo.GetParameterTypes());

            var il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldtoken, proxyMethodBuilder);
            il.Emit(OpCodes.Call, AspectConsts.GetHandleMethod);
            var paras = il.DeclareLocal(typeof(object[]));
            il.Emit(OpCodes.Ldc_I4, parameters.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc, paras);
            for (int i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldloc, paras);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i + 1);
                if (parameters[i].ParameterType.IsValueType)
                    il.Emit(OpCodes.Box, parameters[i].ParameterType);
                il.Emit(OpCodes.Stelem_Ref);
            }
            il.Emit(OpCodes.Ldloc, paras);
            il.Emit(OpCodes.Newobj, typeof(AspectContext).GetConstructor(new[] { typeof(object), typeof(MethodInfo), typeof(object[]) }));
            il.DeclareLocal(typeof(AspectContext));
            il.Emit(OpCodes.Stloc_1);

            // ITaskAspect.InvokeAsync(context)
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, aspectField);
            il.Emit(OpCodes.Ldloc_1);
            var invokeMethod = typeof(IAspect).GetMethod("InvokeAsync");
            var rtnType = methodInfo.ReturnType;
            invokeMethod = invokeMethod?.MakeGenericMethod(GetGenericType(rtnType));
            il.Emit(OpCodes.Callvirt, invokeMethod);
            //il.DeclareLocal(typeof(Task));
            //il.Emit(OpCodes.Stloc_2);
            if (methodInfo.ReturnType == typeof(void))
            {
                // void
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ret);
                return typeBuilder;
            }
            if (methodInfo.ReturnType.IsValueType)
            {
                var method = typeof(Task<>).MakeGenericType(rtnType).GetProperty("Result").GetGetMethod();
                il.Emit(OpCodes.Callvirt, method);
                il.Emit(OpCodes.Ret);
                return typeBuilder;
            }

            if (methodInfo.ReturnType == typeof(Task) || rtnType.BaseType == typeof(Task))
            {
                il.Emit(OpCodes.Ret);
            }
            else
            {
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Call, typeof(AspectContext).GetProperty("ReturnValue").GetGetMethod());
                if (methodInfo.ReturnType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
                }
                il.Emit(OpCodes.Ret);
            }
            return typeBuilder;
        }

        private Type GetGenericType(Type rtnType)
        {
            if (rtnType == typeof(Task))
            {
                return typeof(object);
            }
            if (rtnType.BaseType == typeof(Task))
            {
                return rtnType.GenericTypeArguments.First();
            }
            return rtnType == typeof(void) ? typeof(object) : rtnType;
        }

        private void DefineConstructor(TypeBuilder typeBuilder, FieldBuilder aspectField)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(IAspect) });
            var ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, aspectField);
            ilGenerator.Emit(OpCodes.Ret);
        }

        private TypeBuilder DefineType(Type type)
        {
            return _moduleBuilder.DefineType(AspectConsts.GetTypeName(type.Name), AspectConsts.DefaultClassAttributes, type, type.GetInterfaces());
        }
    }
}
