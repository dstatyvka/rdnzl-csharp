using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rdnzl.Backend
{
    class InvokationResult
    {
        public InvokationResult()
        {
        }

        public InvokationResult(Object value)
        {
            Init(value, null, false);
        }

        public InvokationResult(Object value, bool exception)
        {
            Init(value, null, exception);
        }

        public InvokationResult(Object value, Type type, bool exception)
        {
            Init(value, type, exception);
        }

        private void Init(Object value, Type type, bool exception)
        {
            Result = new DotNetContainer(value, type);
            IsException = exception;
            IsVoid = false;
        }

        private bool _IsVoid = true;

        public bool IsVoid
        {
            get { return _IsVoid; }
            private set { _IsVoid = value; }
        }

        private bool _IsException = false;

        public bool IsException
        {
            get { return _IsException; }
            private set { _IsException = value; }
        }

        DotNetContainer _Result = null;

        internal DotNetContainer Result
        {
            get { return _Result; }
            private set { _Result = value; }
        }
    }
}
