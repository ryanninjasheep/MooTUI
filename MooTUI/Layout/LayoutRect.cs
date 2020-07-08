using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Layout
{
    public class LayoutRect
    {
        public Size WidthData { get; private set; }
        public Size HeightData { get; private set; }

        public int Width { get => WidthData.ActualSize; }
        public int Height { get => HeightData.ActualSize; }

        public LayoutRect(Size widthData, Size heightData)
        {
            WidthData = widthData;
            HeightData = heightData;
        }
        public LayoutRect(int width, int height) 
            : this(new Size(width), new Size(height)) { }
    }
}
