using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Input
{
    public class ScrollInputEventArgs : MouseInputEventArgs
    {
        public int Delta { get; }

        public ScrollInputEventArgs((int x, int y) absoluteLocation, int delta)
            : base(absoluteLocation)
        {
            Delta = delta;
        }
    }
}
