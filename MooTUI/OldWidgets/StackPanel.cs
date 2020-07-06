using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.OldWidgets.Primitives;
using MooTUI.Input;
using MooTUI.IO;

namespace MooTUI.OldWidgets
{
    public class StackPanel : Container
    {
        public List<Widget> Children { get; private set; }

        public StackPanel(int width, int height) : base(width, height)
        {
            Children = new List<Widget>();
        }

        public void AddChild(Widget w)
        {
            LinkChild(w);
            Children.Add(w);
            w.Render();
        }
        public void RemoveChild(Widget w)
        {
            if (Children.Remove(w))
            {
                UnlinkChild(w);
                Render();
            }
        }

        protected override void OnChildResize()
        {
            Render();
        }

        protected override IEnumerable<Widget> GetLogicalChildren() => Children;

        protected override void Draw()
        {
            base.Draw();

            int yIndex = 0;
            foreach (Widget w in Children)
            {
                View.Merge(w.View, 0, yIndex);

                yIndex += w.Height;
                if (yIndex > Height)
                    break;
            }
        }

        #region INPUT

        public override Widget GetHoveredWidget(MouseContext m)
        {
            (int x, int y) = m.Mouse; 
            
            int yIndex = 0;
            foreach (Widget w in Children)
            {
                if (yIndex + w.Height > y)
                {
                    if (x < w.Width)
                    {
                        m.SetRelativeMouse(0, -yIndex);

                        return w;
                    }
                    break;
                }

                yIndex += w.Height;
            }

            // if nothing is hovered over
            return this;
        }

        #endregion
    }
}
