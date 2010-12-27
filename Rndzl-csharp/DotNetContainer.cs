using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Rdnzl.Backend
{
    class DotNetContainer : IDisposable
    {
        void Init(object o, Type type)
        {
            FreeIfHasValue();
            if (o != null) 
                Handle = GCHandle.Alloc(o);
            else
                Handle = null;

            if (Type != null)
                Type = type;
            else if (o != null)
                Type = o.GetType();
            else
                Type = null;
        }

        public DotNetContainer(object o, Type type)
        {
            Init(o, type);
        }

        public DotNetContainer(object o)
        {
            Init(o, null);
        }


        static DotNetContainer Init<T>(T obj)
        {
            return new DotNetContainer(obj, typeof(object));
        }

        public object Target
        {
            get { return Handle.Value.Target; }
        }

        GCHandle? Handle;
        Type _Type;

        public Type Type
        {
            get { return _Type; }
            private set { _Type = value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            FreeIfHasValue();
        }

        private void FreeIfHasValue()
        {
            if (Handle.HasValue)
            {
                Handle.Value.Free();
                Handle = null;
            }
        }

        #endregion

        internal void refContainerType()
        {
            Type type = Target as Type;
            if (!type.IsByRef)
                Init(type.MakeByRefType());
        }

        internal void unrefContainerType()
        {
            Type type = Target as Type;
            if (type.IsByRef)
                Init(type.GetElementType());
        }

    }
}
