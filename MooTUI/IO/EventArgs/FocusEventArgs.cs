using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.IO
{
    public class FocusEventArgs : EventArgs
    {
        public Widget Sender { get; }

        public FocusEventArgs(Widget sender)
        {
            Sender = sender;
        }
    }
}
