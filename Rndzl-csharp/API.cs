using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Reflection;

namespace Rdnzl.Backend
{
    public unsafe class API
    {
        #region DotNetContainer API
        public static bool DotNetContainerIsNull(void* ptr)
        {
            return FromPointer<DotNetContainer>(ptr).Target == null;
        }

        public static void* copyDotNetContainer(void* ptr)
        {
            var original = FromPointer<DotNetContainer>(ptr);
            var copy = new DotNetContainer(original.Target, original.Type);
            return ToPointer(copy); ;
        }

        public static void* makeTypedNullDotNetContainer(IntPtr type_name)
        {
            var type = Type.GetType(Marshal.PtrToStringUni(type_name));
            var result = new DotNetContainer(null, type);
            return ToPointer(result);
        }

        public static void* makeTypedNullDotNetContainerFromType(void* type)
        {
            var container = FromPointer<DotNetContainer>(type);
            var result = new DotNetContainer(null, container.Target as Type);
            return ToPointer(result);
            
        }

        public static void* makeTypeFromName(IntPtr type_name)
        {
            var type = Type.GetType(Marshal.PtrToStringUni(type_name));
            var container = new DotNetContainer(type);
            return ToPointer(container);
        }

        public static void* makeDotNetContainerFromBoolean(bool value)
        {
            return ToPointer(new DotNetContainer(value));
        }

        public static void* makeDotNetContainerFromInt(int value)
        {
            return ToPointer(new DotNetContainer(value));
        }

        public static void* makeDotNetContainerFromLong(IntPtr value)
        {
            return ToPointer(new DotNetContainer(
                Int64.Parse(Marshal.PtrToStringUni(value), CultureInfo.InvariantCulture)));
        }

        public static void* makeDotNetContainerFromFloat(float value)
        {
            return ToPointer(new DotNetContainer(value));
        }

        public static void* makeDotNetContainerFromDouble(double value)
        {
            return ToPointer(new DotNetContainer(value));
        }

        public static void* makeDotNetContainerFromString(IntPtr value)
        {
            return ToPointer(new DotNetContainer(Marshal.PtrToStringUni(value)));
        }

        public static int getDotNetContainerTypeStringLength(void* ptr)
        {
            return FromPointer<DotNetContainer>(ptr).Type.FullName.Length;
        }

        public static void getDotNetContainerTypeAsString(void* ptr, IntPtr destination)
        {
            CopyZeroTerminatedString(FromPointer<DotNetContainer>(ptr).Type.FullName, destination);
        }

        public static int getDotNetContainerObjectStringLength(void *ptr)
        {
            return FromPointer<DotNetContainer>(ptr).ToString().Length;
        }

        public static void getDotNetContainerObjectAsString(void *ptr, IntPtr s)
        {
            CopyZeroTerminatedString(FromPointer<DotNetContainer>(ptr).ToString(), s);
        }

        public static void refDotNetContainerType(void* ptr)
        {
            FromPointer<DotNetContainer>(ptr).refContainerType();
        }

        public static void unrefDotNetContainerType(void* ptr)
        {
            FromPointer<DotNetContainer>(ptr).unrefContainerType();
        }


        // unboxing
        public static char getDotNetContainerCharValue(void* ptr)
        {
            return (char)FromPointer<DotNetContainer>(ptr).Target;
        }

        public static int getDotNetContainerIntValue(void* ptr)
        {
            return (int)FromPointer<DotNetContainer>(ptr).Target;
        }

        public static bool getDotNetContainerBooleanValue(void* ptr)
        {
            return (bool)FromPointer<DotNetContainer>(ptr).Target;
        }

        public static double getDotNetContainerDoubleValue(void* ptr)
        {
            return (double)FromPointer<DotNetContainer>(ptr).Target;
        }

        public static float getDotNetContainerSingleValue(void *ptr)
        {
            return (float)FromPointer<DotNetContainer>(ptr).Target;
        }

        public static void freeDotNetContainer(void* ptr)
        {
            var int_ptr = new IntPtr(ptr);
            var handle = GCHandle.FromIntPtr(int_ptr);
            var container = handle.Target as IDisposable;
            if(container != null)
                container.Dispose();
            handle.Free();
        }
        #endregion

        #region Delegate adapter API
        public void* buildDelegateType(IntPtr typeName, void* returnType, void* argTypes)
        {
            var result = DelegateAdapter.Build(Marshal.PtrToStringUni(typeName),
                (Type)FromPointer<DotNetContainer>(returnType).Target,
                (Type[])FromPointer<DotNetContainer>(argTypes).Target);
            return ToPointer(new DotNetContainer(result));
        }

        delegate object CallbackDelegate(int index, void* args);

        void SetFunctionPointers(IntPtr callback, IntPtr release)
        {
            var cb_delegate = Marshal.GetDelegateForFunctionPointer(callback, typeof(CallbackDelegate))
                    as CallbackDelegate;
            var release_delegate = 
                Marshal.GetDelegateForFunctionPointer(release, typeof(DelegateAdapter.ReleaseDelegate))
                    as DelegateAdapter.ReleaseDelegate;
            DelegateAdapter.SetFunctions(
                (i, args) => cb_delegate(i, ToPointer(new DotNetContainer(args))),
                release_delegate);
        }

        #endregion

        #region Fields API

        private static void* GetFieldValue(object instance, Type type, 
            IntPtr fieldName, BindingFlags binding, string message)
        {
           return WithExceptionsCatched(() =>
                {
                    var field_name = Marshal.PtrToStringUni(fieldName);
                    var field_info = type.GetField(field_name, binding);

                    if (field_info == null)
                        throw new Exception(String.Format("{2}: {0}->{1}",
                            type.FullName, field_name, message));

                    return field_info.GetValue(instance);
                });
        }

        void* getInstanceFieldValue(IntPtr fieldName, void* target)
        {
            var binding = BindingFlags.Instance | BindingFlags.Public;
            var message = "Instance field not found";
            var container = FromPointer<DotNetContainer>(target);
            var type = container.Type;
            var instance = container.Target;

            return GetFieldValue(instance, type, fieldName, binding, message);
        }

        void* getStaticFieldValue(IntPtr fieldName, void* targetType)
        {
            var binding = BindingFlags.Instance | BindingFlags.Public;
            var message = "Static field not found";
            var container = FromPointer<DotNetContainer>(targetType);
            var type = (Type)container.Target;

            return GetFieldValue(null, type, fieldName, binding, message);
        }

        private static void* SetFieldValue(object instance, Type type, object newValue,
            IntPtr fieldName, BindingFlags binding, string message)
        {
            return WithExceptionsCatched(() => 
                {
                    var field_name = Marshal.PtrToStringUni(fieldName);
                    var field_info = type.GetField(field_name, binding);

                    if (field_info == null)
                        throw new Exception(String.Format("{2}: {0}->{1}",
                            type.FullName, field_name, message));

                    field_info.SetValue(instance, newValue);
                });
        }

        static void* setInstanceFieldValue(IntPtr fieldName, void* target, void* newValue)
        {
            var binding = BindingFlags.Instance | BindingFlags.Public;
            var message = "Instance field not found";
            var container = FromPointer<DotNetContainer>(target);
            var type = container.Type;
            var instance = container.Target;
            var new_value = FromPointer<DotNetContainer>(newValue).Target;
            return SetFieldValue(instance, type, new_value, fieldName, binding, message);
        }

        static void* setStaticFieldValue(IntPtr fieldName, void* _type, void* newValue)
        {
            var binding = BindingFlags.Static | BindingFlags.Public;
            var message = "Static field not found";
            var container = FromPointer<DotNetContainer>(_type);
            var type = (Type)container.Target;
            var new_value = FromPointer<DotNetContainer>(newValue).Target;
            return SetFieldValue(null, type, new_value, fieldName, binding, message);
        }

        static void* getInstanceFieldValueDirectly(void* fieldInfo, void* target)
        {
            return WithExceptionsCatched(() =>
                {
                    var container = FromPointer<DotNetContainer>(target);
                    var field_info = (FieldInfo)FromPointer<DotNetContainer>(fieldInfo).Target;
                    return field_info.GetValue(container.Target);
                });
        }

        static void* getStaticFieldValueDirectly(void* fieldInfo)
        {
            return WithExceptionsCatched(() =>
            {
                var field_info = (FieldInfo)FromPointer<DotNetContainer>(fieldInfo).Target;
                return field_info.GetValue(null);
            });
        }

        static void* setInstanceFieldValueDirectly(void* fieldInfo, void* target, void* newValue)
        {
            return WithExceptionsCatched(() =>
                {
                    var container = FromPointer<DotNetContainer>(target);
                    var value = FromPointer<DotNetContainer>(newValue);
                    var field_info = (FieldInfo)FromPointer<DotNetContainer>(fieldInfo).Target;
                    field_info.SetValue(container.Target, value.Target);
                });
        }

        static void* setStaticFieldValueDirectly(void* fieldInfo, void* newValue)
        {
            return WithExceptionsCatched(() =>
                {
                    var value = FromPointer<DotNetContainer>(newValue);
                    var field_info = (FieldInfo)FromPointer<DotNetContainer>(fieldInfo).Target;
                    field_info.SetValue(null, value.Target);
                });
        }

        #endregion

        #region Contructor invokations
        static void* invokeConstructor(void *_type, int nargs, void*[] args)
        {
            return WithExceptionsCatched(() =>
                {
                    var type = (Type)FromPointer<DotNetContainer>(_type).Target;
                    var instance = Activator.CreateInstance(type, ToArray(nargs, args));
                    return instance;
                });
        }

        private static object[] ToArray(int nargs, void*[] args)
        {
            return ToArray(nargs, args, 0);
        }

        private static object[] ToArray(int nargs, void*[] args, int start)
        {
            object[] real_args = new object[nargs - start];
            for (int i = start; i < nargs; ++i)
                real_args[i] = FromPointer<DotNetContainer>(args[i]);
            return real_args;
        }

        #endregion

        #region Properties

        static object GetPropertyValue(object instance, Type type, IntPtr name, int nargs, void*[] args,
            BindingFlags binding, String message)
        {
            var Name = Marshal.PtrToStringUni(name);
            var info = type.GetProperty(Name, binding);

            if (info == null)
                throw new Exception(String.Format("{2}: {0}->{1}",
                    type.FullName, Name, message));

            return info.GetValue(instance, ToArray(nargs, args));
        }

        static void SetPropertyValue(object instance, Type type, IntPtr name, int nargs, void*[] args,
            BindingFlags binding, String message)
        {
            var Name = Marshal.PtrToStringUni(name);
            var info = type.GetProperty(Name, binding);

            if (info == null)
                throw new Exception(String.Format("{2}: {0}->{1}",
                    type.FullName, Name, message));

            info.SetValue(instance, 
                FromPointer<DotNetContainer>(args[0]).Target, 
                ToArray(nargs, args, 1));
        }

        static void* getInstancePropertyValue(IntPtr propertyName, void* target, int nargs, void*[] args)
        {
            return WithExceptionsCatched(() =>
                 {
                     var container = FromPointer<DotNetContainer>(target);
                     return GetPropertyValue(container.Target, container.Type, propertyName, nargs, args,
                         BindingFlags.Instance | BindingFlags.Public, "Instance property not found");
                 });
        }

         static  void *setInstancePropertyValue(IntPtr propertyName, void *target, int nargs, void*[] args)
         {
             return WithExceptionsCatched(() =>
             {
                 var container = FromPointer<DotNetContainer>(target);
                 SetPropertyValue(container.Target, container.Type, propertyName, nargs, args,
                     BindingFlags.Instance | BindingFlags.Public, "Instance property not found");
             });
         }

         static void *getStaticPropertyValue(IntPtr propertyName, void *type, int nargs, void*[] args)
         {
             return WithExceptionsCatched(() =>
             {
                 var container = FromPointer<DotNetContainer>(type);
                 return GetPropertyValue(null, (Type)container.Target, propertyName, nargs, args,
                     BindingFlags.Static| BindingFlags.Public, "Static property not found");
             });
         }

         static void *setStaticPropertyValue(IntPtr propertyName, void *type, int nargs, void*[] args)
         {
             return WithExceptionsCatched(() =>
             {
                 var container = FromPointer<DotNetContainer>(type);
                 SetPropertyValue(null, (Type)container.Target, propertyName, nargs, args,
                     BindingFlags.Static | BindingFlags.Public, "Static property not found");
             });
         }

         static void *getInstancePropertyValueDirectly(void *propertyInfo, int nargs, void*[] args)
         {
             return WithExceptionsCatched(() =>
                 {
                     var info = (PropertyInfo)FromPointer<DotNetContainer>(propertyInfo).Target;
                     return info.GetValue(
                         FromPointer<DotNetContainer>(args[0]).Target,
                         ToArray(nargs, args, 1));
                 });
         }

         static void *getStaticPropertyValueDirectly(void *propertyInfo, int nargs, void*[] args)
         {
             return WithExceptionsCatched(() =>
             {
                 var info = (PropertyInfo)FromPointer<DotNetContainer>(propertyInfo).Target;
                 return info.GetValue(null, ToArray(nargs, args));
             });
         }

         static void *setInstancePropertyValueDirectly(void *propertyInfo, int nargs, void*[] args)
         {
             return WithExceptionsCatched(() =>
             {
                 var info = (PropertyInfo)FromPointer<DotNetContainer>(propertyInfo).Target;
                 info.SetValue(
                     FromPointer<DotNetContainer>(args[0]).Target,
                     FromPointer<DotNetContainer>(args[1]).Target,
                     ToArray(nargs, args, 2));
             });
         }

         static void *setStaticPropertyValueDirectly(void *propertyInfo, int nargs, void*[] args)
         {
             return WithExceptionsCatched(() =>
             {
                 var info = (PropertyInfo)FromPointer<DotNetContainer>(propertyInfo).Target;
                 info.SetValue(null,
                     FromPointer<DotNetContainer>(args[0]).Target,
                     ToArray(nargs, args, 1));
             });
         }
        #endregion



        #region InvokationResult API
        static bool InvocationResultIsVoid(void* ptr)
        {
            return FromPointer<InvokationResult>(ptr).IsVoid;
        }

        static bool InvocationResultIsException(void* ptr)
        {
            return FromPointer<InvokationResult>(ptr).IsException;
        }

        static void* getDotNetContainerFromInvocationResult(void* ptr)
        {
            return ToPointer(FromPointer<InvokationResult>(ptr).Result);
        }

        static void freeInvocationResult(void* ptr)
        {
            freeDotNetContainer(ptr);
        }

        #endregion

        #region Private utilities

        delegate object Action();
        static void* WithExceptionsCatched(Action action)
        {
            try
            {
                return ToPointer(new InvokationResult(action()));
            }
            catch (TargetInvocationException e)
            {
                return ToPointer(new InvokationResult(e.InnerException, true));
            }
            catch (Exception e)
            {
                return ToPointer(new InvokationResult(e, true));
            }
        }

        delegate void VoidAction();
        static void* WithExceptionsCatched(VoidAction action)
        {
            try
            {
                action();
                return ToPointer(new InvokationResult());
            }
            catch (TargetInvocationException e)
            {
                return ToPointer(new InvokationResult(e.InnerException, true));
            }
            catch (Exception e)
            {
                return ToPointer(new InvokationResult(e, true));
            }
        }

        private static void CopyZeroTerminatedString(string source, IntPtr destination)
        {
            var type_name = source.ToCharArray();
            Marshal.Copy(type_name, 0, destination, type_name.Length);
            switch (Marshal.SizeOf(typeof(char)))
            {
                case 1:
                    Marshal.WriteByte(destination, type_name.Length, 0);
                    break;
                case 2:
                    Marshal.WriteInt16(destination, type_name.Length, 0);
                    break;
                case 4:
                    Marshal.WriteInt32(destination, type_name.Length, 0);
                    break;
                case 8:
                    Marshal.WriteInt64(destination, type_name.Length, 0);
                    break;
                default:
                    throw new SystemException(
                        String.Format("Unsupported char length: {0}", Marshal.SizeOf(typeof(char))));
            }
        }

        static T FromPointer<T>(void* ptr) where T:class
        {
            var int_ptr = new IntPtr(ptr);
            var handle = GCHandle.FromIntPtr(int_ptr);
            var container = handle.Target as T;
            return container;
        }

        static void* ToPointer<T>(T container) where T:class
        {
            return GCHandle.Alloc(container).AddrOfPinnedObject().ToPointer();
        }

        #endregion
    }
}

