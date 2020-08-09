using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Input
{
    public class MouseMoveInputEventArgs : MouseInputEventArgs
    {
        public MouseMoveInputEventArgs((int x, int y) absoluteLocation)
            : base(absoluteLocation) { }
    }
}
