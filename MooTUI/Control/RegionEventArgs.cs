using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Control
{
    public class RegionEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public RegionEventArgs(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
