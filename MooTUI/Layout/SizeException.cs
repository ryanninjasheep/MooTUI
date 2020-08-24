using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MooTUI.Layout
{
    internal class SizeException : Exception
    {
        public SizeException(string message) : base(message) { }
    }
}
