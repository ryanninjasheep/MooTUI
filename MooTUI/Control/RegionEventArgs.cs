using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Widgets.Primitives;

namespace MooTUI.Control
{
    public class RegionEventArgs : BubblingEventArgs
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public RegionEventArgs(Widget origin, int x, int y, int width, int height) : base(origin)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
