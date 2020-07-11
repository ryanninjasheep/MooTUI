using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public Size GetSize(Orientation orientation) => orientation switch
        {
            Orientation.Horizontal => WidthData,
            Orientation.Vertical => HeightData,
            _ => throw new InvalidEnumArgumentException(),
        };

        public LayoutRect WithSize(Orientation orientation, Size s) => orientation switch
        {
            Orientation.Horizontal => new LayoutRect(s, HeightData),
            Orientation.Vertical => new LayoutRect(WidthData, s),
            _ => throw new InvalidEnumArgumentException(),
        };

        public LayoutRect TryResize(int width, int height)
        {
            Size wi = WidthData is FlexSize h ? h.WithActualSize(width) : WidthData;
            Size he = HeightData is FlexSize v ? v.WithActualSize(height) : HeightData;

            return new LayoutRect(wi, he);
        }
    }
}
