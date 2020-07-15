﻿using System;
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

        public Size GetSizeInMainAxis(Orientation orientation) => orientation switch
        {
            Orientation.Horizontal => WidthData,
            Orientation.Vertical => HeightData,
            _ => throw new InvalidEnumArgumentException(),
        };
        public Size GetSizeInCrossAxis(Orientation orientation) => orientation switch
        {
            Orientation.Horizontal => HeightData,
            Orientation.Vertical => WidthData,
            _ => throw new InvalidEnumArgumentException(),
        };

        public LayoutRect WithSize(Orientation orientation, Size s) => orientation switch
        {
            Orientation.Horizontal => new LayoutRect(s, HeightData),
            Orientation.Vertical => new LayoutRect(WidthData, s),
            _ => throw new InvalidEnumArgumentException(),
        };

        public void TryResize(int width, int height)
        {
            if (WidthData is FlexSize f)
                f.ActualSize = width;

            if (HeightData is FlexSize g)
                g.ActualSize = height;
        }

        private void BubbleSizeChanged(object sender, EventArgs e)
        {
            EventHandler handler = SizeChanged;
            handler?.Invoke(sender, e);
        }
    }
}
