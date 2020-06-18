using MooTUI.Widgets.Primitives;
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
        public static Window GenerateWindowFromWidget(Widget w)
        {
            Window newWindow = new Window();
            WPFMooViewer viewer = new WPFMooViewer(w.Width, w.Height, Colors.White);
            newWindow.Content = viewer;

            MooInterface @interface = new MooInterface(viewer, w);

            return newWindow;
        }
    }
}
