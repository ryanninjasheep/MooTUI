using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.IO.EventArgs
{
    public class RegionEventArgs : System.EventArgs
    {
        public int XStart { get; }
        public int YStart { get; }
        public int Width { get; }
        public int Height { get; }

        public RegionEventArgs(int xStart, int yStart, int width, int height)
        {
            XStart = xStart;
            YStart = yStart;
            Width = width;
            Height = height;
        }
        public RegionEventArgs(int x, int y) : this(x, y, 1, 1) { }
    }
}
