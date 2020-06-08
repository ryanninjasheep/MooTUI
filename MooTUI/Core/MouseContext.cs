using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Core
{
    public class MouseContext
    {
        public (int X, int Y) AbsoluteMouse { get; private set; }
        public (int X, int Y) Mouse { get; private set; }

        public int ScrollDelta { get; private set; }

        public void SetAbsoluteMouse((int, int) pos)
        {
            AbsoluteMouse = pos;
            Mouse = pos;
        }
        public void SetAbsoluteMouse(int x, int y) => SetAbsoluteMouse((x, y));

        public void SetRelativeMouse(int xOffset, int yOffset)
        {
            Mouse = (Mouse.X + xOffset, Mouse.Y + yOffset);
        }

        public void SetScrollDelta(int delta) => ScrollDelta = delta;
    }
}
