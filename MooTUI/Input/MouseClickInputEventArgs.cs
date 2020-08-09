using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Input
{
    public class MouseClickInputEventArgs : MouseInputEventArgs
    {
        public enum MouseButton { LEFT, RIGHT, MIDDLE }

        public MouseButton Button { get; }

        public MouseClickInputEventArgs((int x, int y) absoluteLocation, MouseButton button)
            : base(absoluteLocation)
        {
            Button = button;
        }
    }
}
