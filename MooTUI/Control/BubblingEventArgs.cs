using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Control
{
    /// <summary>
    /// Used for any events that travel up the logical tree.
    /// </summary>
    public abstract class BubblingEventArgs : EventArgs
    {
        public Widget? Origin { get; }
        public Widget? Previous { get; set; }

        public BubblingEventArgs(Widget? origin)
        {
            Origin = origin;
            Previous = origin;
        }
    }
}
