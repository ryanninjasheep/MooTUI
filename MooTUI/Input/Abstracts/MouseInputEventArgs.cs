using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Input
{
    public abstract class MouseInputEventArgs : InputEventArgs
    {
        public (int X, int Y) AbsoluteLocation { get; }
        public (int X, int Y) Location { get; private set; }

        public MouseInputEventArgs((int x, int y) absoluteLocation)
        {
            AbsoluteLocation = absoluteLocation;
            Location = absoluteLocation;
        }

        public void SetRelativeMouse((int x, int y) offset)
        {
            Location = (Location.X + offset.x, Location.Y + offset.y);
        }
    }
}
