using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Control
{
    public class RenderEventArgs : BubblingEventArgs
    {
        public RenderEventArgs(Widget origin) : base(origin) { }
    }
}
