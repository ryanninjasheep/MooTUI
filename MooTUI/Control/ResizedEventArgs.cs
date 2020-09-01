using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Control
{
    public class ResizedEventArgs : BubblingEventArgs
    {
        public ResizedEventArgs(Widget origin) : base(origin) { }
    }
}
