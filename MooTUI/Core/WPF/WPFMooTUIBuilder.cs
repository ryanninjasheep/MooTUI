using MooTUI.OldWidgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MooTUI.IO;
using System.Windows.Media;

namespace MooTUI.Core.WPF
{
    public static class WPFMooTUIBuilder
    {
        public static Window GenerateWindowFromWidget(Widget w, Theme t)
        {
            Window newWindow = new Window();
            WPFMooViewer viewer = new WPFMooViewer(w.Width, w.Height, t);
            newWindow.Content = viewer;
            newWindow.Background = new SolidColorBrush(t.Palette[Color.Base03]);

            MooInterface @interface = new MooInterface(viewer, w);

            return newWindow;
        }
    }
}
