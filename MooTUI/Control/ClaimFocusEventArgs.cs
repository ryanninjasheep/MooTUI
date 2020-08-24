using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Input
{
    public class ClaimFocusEventArgs : EventArgs
    {
        public Widget Sender { get; }

        public ClaimFocusEventArgs(Widget sender)
        {
            Sender = sender;
        }
    }
}
