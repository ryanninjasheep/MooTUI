using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Control
{
    public abstract class BubblingEventArgs : EventArgs
    {
        public Widget? Previous { get; set; }

        public BubblingEventArgs(Widget? previous)
        {
            Previous = previous;
        }
    }
}
