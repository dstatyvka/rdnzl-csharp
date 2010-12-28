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
            _Object = o;

            if (Type != null)
                _Type = type;
            else if (o != null)
                _Type = o.GetType();
            else
                _Type = null;
        }

        public DotNetContainer(object o, Type type)
        {
            Init(o, type);
        }

        public DotNetContainer(object o)
        {
            Init(o, null);
        }

        public object Target
        {
            get { return _Object; }
        }

        object _Object;
        Type _Type;

        public Type Type
        {
            get { return _Type; }
            internal set 
            {
                if (_Type == null || _Type.IsAssignableFrom(value))
                {
                    _Type = value;
                }
                else
                {
                    var new_target = Convert.ChangeType(Target, value);
                    Init(new_target, value);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            FreeIfHasValue();
        }

        private void FreeIfHasValue()
        {
            _Object = null;
        }

        #endregion

        internal void refContainerType()
        {
            Type type = Target as Type;
            if (!type.IsByRef)
                Init(type.MakeByRefType(), null);
        }

        internal void unrefContainerType()
        {
            Type type = Target as Type;
            if (type.IsByRef)
                Init(type.GetElementType(), null);
        }

    }
}
