using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Control
{
    public class ResizeEventArgs : ConditionalBubblingEventArgs
    {
        public ResizeEventArgs()
        {
            Continue = false;
        }
    }
}
