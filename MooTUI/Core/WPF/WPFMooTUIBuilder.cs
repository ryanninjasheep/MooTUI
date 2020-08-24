using MooTUI.Widgets.Primitives;
using MooTUI.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace MooTUI.Core.WPF
{
    public static class WPFMooTUIBuilder
    {
        public static void GenerateViewer(Window window, Widget widget, Theme t)
        {
            WPFMooViewer viewer = new WPFMooViewer(widget.Width, widget.Height, t);
            window.Content = viewer;
            window.Background = new SolidColorBrush(t.Palette[MooTUI.Drawing.Color.Base03]);

            new MooInterface(viewer, widget);
        }
    }
}
