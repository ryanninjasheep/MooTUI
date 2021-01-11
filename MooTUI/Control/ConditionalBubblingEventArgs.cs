using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Control
{
    public abstract class ConditionalBubblingEventArgs : BubblingEventArgs
    {
        public bool Continue { get; set; }

        public ConditionalBubblingEventArgs() { }
    }
}
