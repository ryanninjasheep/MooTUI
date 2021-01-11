using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Widgets.Primitives;

namespace MooTUI.Control
{
    public class EnsureRegionVisibleEventArgs : BubblingEventArgs
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public EnusreRegionVisibleEventArgs(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
