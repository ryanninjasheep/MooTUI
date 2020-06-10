using MooTUI.Core;
using MooTUI.IO;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MooTUI.Widgets
{
    public class Canvas : Container
    {
        private List<WidgetWithLocation> Children { get; set; }

        public Canvas(int width, int height) : base(width, height)
        {
            Children = new List<WidgetWithLocation>();
        }

        public void AddChild(Widget w, int x, int y)
        {
            Children.Add(new WidgetWithLocation(w, x, y));
            LinkChild(w);
            w.Render();
        }
        public void RemoveChild(Widget w)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Widget == w)
                {
                    UnlinkChild(w);
                    Children.RemoveAt(i);

                    Render();
                    return;
                }
            }

            throw new InvalidOperationException("Parent does not contain child.");
        }

        protected override IEnumerable<Widget> GetLogicalChildren()
        {
            List<Widget> toReturn = new List<Widget>();

            foreach(WidgetWithLocation w in Children)
            {
                toReturn.Add(w.Widget);
            }

            return toReturn;
        }

        protected override void OnChildResize()
        {
            Render();
        }

        protected override void Draw()
        {
            base.Draw();

            View.FillColorScheme(Style.GetColorScheme("Default"));

            foreach (WidgetWithLocation w in Children)
            {
                View.Merge(w.Widget.View, w.X, w.Y);
            }
        }

        public override Widget GetHoveredWidget(MouseContext m)
        {
            (int x, int y) = m.Mouse;

            foreach (WidgetWithLocation w in Children)
            {
                if (x >= w.X && y >= w.Y && w.Widget.HitTest(x - w.X, y - w.Y))
                {
                    m.SetRelativeMouse(-w.X, -w.Y);
                    return w.Widget;
                }
            }

            return this;
        }

        private class WidgetWithLocation
        {
            public Widget Widget { get; set; }

            public int X { get; set; }
            public int Y { get; set; }

            public WidgetWithLocation(Widget widget, int x, int y)
            {
                Widget = widget;
                X = x;
                Y = y;
            }
        }
    }
}
