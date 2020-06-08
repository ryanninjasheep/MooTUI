using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.IO
{
    public interface IEnsureCursorVisible
    {
        public event EventHandler<CursorRegionEventArgs> EnsureCursorVisible;
    }

    public class CursorRegionEventArgs : EventArgs
    {
        public int XStart { get; }
        public int YStart { get; }
        public int Width { get; }
        public int Height { get; }

        public CursorRegionEventArgs(int xStart, int yStart, int width, int height)
        {
            XStart = xStart;
            YStart = yStart;
            Width = width;
            Height = height;
        }
        public CursorRegionEventArgs(int x, int y) : this(x, y, 1, 1) { }
    }
}
