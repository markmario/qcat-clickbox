using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickBox.Web.Exceptions
{
    public class StripeClickboxStorageException : Exception
    {
        public Exception InnerEx { get; set; }
        public StripeClickboxStorageException(string msg, Exception inner) : base(msg)
        {
            InnerEx = inner;
        }
    }
}