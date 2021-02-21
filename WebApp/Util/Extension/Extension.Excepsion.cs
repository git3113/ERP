using System;
using System.Collections.Generic;
using System.Text;

namespace Util.Extension
{
    public static partial class Extension
    {
        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return ex.InnerException.GetOriginalException();
        }
    }
}
