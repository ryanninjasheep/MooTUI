using MooTUI.Core;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace MooTUI.Widgets
{
    public class Spacer : Widget
    {
        public Spacer(int width, int height) : base(width, height) 
        {
            IsEnabled = false;
        }
    }
}
