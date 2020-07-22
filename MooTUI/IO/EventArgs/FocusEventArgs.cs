using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.IO.EventArgs
{
    public class FocusEventArgs : System.EventArgs
    {
        public Widget Sender { get; }

        public FocusEventArgs(Widget sender)
        {
            Sender = sender;
        }
    }
}
