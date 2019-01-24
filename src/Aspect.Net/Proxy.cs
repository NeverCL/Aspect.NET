using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Aspect.Net
{
    public class Proxy
    {
        private IAspect _aspect;
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
            var proxyType = typeBuilder.CreateTypeInfo();
            var proxy = Activator.CreateInstance(proxyType, _aspect);
            return proxy as T;
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
