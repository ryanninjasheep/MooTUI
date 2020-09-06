using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Control
{
    public class ClaimFocusEventArgs : BubblingEventArgs
    {
        public Widget Sender { get; }

        public ClaimFocusEventArgs(Widget sender)
        {
            Sender = sender;
        }
    }
}
