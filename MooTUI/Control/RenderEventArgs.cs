using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Control
{
    public class RenderEventArgs : BubblingEventArgs
    {
        public new Widget Previous => base.Previous!;

        public RenderEventArgs(Widget w) : base(w) { }
    }
}
