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

            WidthData.ActualSizeChanged += BubbleSizeChanged;
            HeightData.ActualSizeChanged += BubbleSizeChanged;
        }
        public LayoutRect(int width, int height) 
            : this(new Size(width), new Size(height)) { }

        public event EventHandler SizeChanged;

        public LayoutRect Clone() => new LayoutRect(WidthData.Clone(), HeightData.Clone());

        public LayoutRect WithRelativeSize(int widthDiff, int heightDiff) =>
            new LayoutRect(WidthData.WithRelativeSize(widthDiff), HeightData.WithRelativeSize(heightDiff));

        public void TryResize(int width, int height)
        {
            if (WidthData is FlexSize f)
                f.TryResize(width);

            if (HeightData is FlexSize g)
                g.TryResize(height);
        }

        public void AssertMinSize(int width, int height)
        {
            if (Width < width || Height < height)
                throw new SizeException("LayoutRect is too small!");
        }

        private void BubbleSizeChanged(object sender, EventArgs e)
        {
            EventHandler handler = SizeChanged;
            handler?.Invoke(sender, e);
        }
    }
}
