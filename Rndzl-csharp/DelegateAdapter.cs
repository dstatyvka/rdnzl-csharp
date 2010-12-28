using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Rdnzl.Backend
{
    [Serializable]
    public class DelegateAdapter : IDisposable
    {
        int indexInLisp;

        // to be called from CL
        public void init(int index)
        {
            indexInLisp = index;
        }

        public object invoke(object[] args)
        {
            if (Callback != null)
                return Callback(indexInLisp, args);
            else
                return null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (Release != null)
                Release(indexInLisp);
        }

        #endregion

        static CallbackDelegate Callback;
        static ReleaseDelegate Release;

        internal delegate object CallbackDelegate(int index, object[] args);
        internal delegate void ReleaseDelegate(int index);

        static internal void SetFunctions(CallbackDelegate callback, ReleaseDelegate release)
        {
            Callback = callback;
            Release = release;
        }

        #region Building
        public static Type Build(string type_name, Type return_type, Type[] arg_types)
        {
            var type_builder = ModuleBuilder.DefineType(
                string.Concat(PrivateAssemblyName, ".", type_name),
                TypeAttributes.Public | TypeAttributes.Serializable,
                typeof(DelegateAdapter));

            var ctr_builder = type_builder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard,
                new Type[0]);
            BuildConstructor(ctr_builder.GetILGenerator());

            var invoke_builder = type_builder.DefineMethod(
                "InvokeClosure", MethodAttributes.Public, return_type, arg_types);
            BuildInvokeMethod(invoke_builder.GetILGenerator(), return_type, arg_types);

            return type_builder.CreateType();
        }

        private static void BuildInvokeMethod(ILGenerator il, Type return_type, Type[] arg_types)
        {
            // create a System.Object array of the same length as argTypes
            il.DeclareLocal(typeof(Object).MakeArrayType());
            il.Emit(OpCodes.Ldc_I4, arg_types.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc_0);

            // store method arguments in this array
            for (int i = 0; i < arg_types.Length; ++i)
            {
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i + 1);

                var arg_type = arg_types[i];
                if (arg_type.IsValueType)
                    il.Emit(OpCodes.Box, arg_type);
                il.Emit(OpCodes.Stelem_Ref);
            }

            // call "invoke" with this array
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Call, typeof(DelegateAdapter).GetMethod("invoke"));

            // handle return value of "invoke"
            if (return_type.Equals(typeof(void)))
                il.Emit(OpCodes.Pop);
            else
                il.Emit(OpCodes.Unbox_Any, return_type);
            
            il.Emit(OpCodes.Ret);
        }

        private static void BuildConstructor(ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(DelegateAdapter).GetConstructor(new Type[0]));
            il.Emit(OpCodes.Ret);
        }

        static ModuleBuilder _ModuleBuilder;

        public static ModuleBuilder ModuleBuilder
        {
            get 
            { 
                if(DelegateAdapter._ModuleBuilder == null)
                    DelegateAdapter._ModuleBuilder = MakeModuleBuilder();

                return DelegateAdapter._ModuleBuilder;
            }
        }

        private static readonly string PrivateAssemblyName = "RDNZLPrivateAssembly";
        private static ModuleBuilder MakeModuleBuilder()
        {
            var assembly_builder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(PrivateAssemblyName),
                AssemblyBuilderAccess.Run);
            return assembly_builder.DefineDynamicModule(PrivateAssemblyName);
        }
        #endregion
    }
}
