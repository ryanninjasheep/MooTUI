﻿using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Control
{
    public class ClaimFocusEventArgs : BubblingEventArgs
    {
        public ClaimFocusEventArgs(Widget origin) : base(origin) { }
    }
}
