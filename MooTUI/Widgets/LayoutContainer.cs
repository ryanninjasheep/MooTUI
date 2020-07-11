using MooTUI.Input;
using MooTUI.Layout;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MooTUI.Widgets
{
    public class LayoutContainer : Container
    {
        public List<Widget> Children { get; private set; }
        public Orientation Orientation { get; }

        private int OrientationSize => Bounds.GetSize(Orientation).ActualSize;

        public LayoutContainer(LayoutRect bounds, Orientation oritentation) : base(bounds)
        {
            Orientation = oritentation;
            Children = new List<Widget>();
        }

        public void AddChild(Widget w)
        {
            LinkChild(w);
            Children.Add(w);
            CalculateLayout();
            w.Render();
        }

        public void CalculateLayout()
        {
            AssertMinSizesFit();

            LockRendering();

            List<(Widget, FlexSize)> flexible = Children
                .Where((w) => w.Bounds.GetSize(Orientation) is FlexSize)
                .Select((w) => (w, w.Bounds.GetSize(Orientation) as FlexSize))
                .ToList();

            int freeSpace = OrientationSize - Children.Sum((s) => s.Bounds.GetSize(Orientation).ActualSize);

            while (freeSpace != 0 && flexible.Count > 0)
            {
                if (freeSpace < 0)
                    flexible = flexible
                        .Where(((Widget w, FlexSize f) pair) => pair.f.ActualSize > pair.f.Min)
                        .ToList();

                foreach ((Widget w, FlexSize f) in flexible)
                {
                    float totalPreferred = flexible.Sum(((Widget w, FlexSize f) pair) => pair.f.PreferredSize);

                    float sizeRatio = f.PreferredSize / totalPreferred;
                    float growth = sizeRatio * freeSpace;
                    int newSize = Math.Max(f.PreferredSize + (int)Math.Round(growth), f.Min);
                    switch (Orientation)
                    {
                        case Orientation.Horizontal:
                            w.Resize(w.Bounds.TryResize(newSize, Height));
                            break;
                        case Orientation.Vertical:
                            w.Resize(w.Bounds.TryResize(Width, newSize));
                            break;
                        default:
                            throw new System.ComponentModel.InvalidEnumArgumentException();
                    }
                }

                freeSpace = OrientationSize - Children.Sum((s) => s.Bounds.GetSize(Orientation).ActualSize);
            }

            UnlockRendering();

            RefreshVisual();
        }

        public override Widget GetHoveredWidget(MouseContext m)
        {
            (int x, int y) = m.Mouse;

            int index = 0;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    foreach (Widget w in Children)
                    {
                        if (index + w.Width > x)
                        {
                            if (y < w.Height)
                            {
                                m.SetRelativeMouse(-index, 0);

                                return w;
                            }
                            break;
                        }

                        index += w.Width;
                    }
                    break;
                case Orientation.Vertical:
                    foreach (Widget w in Children)
                    {
                        if (index + w.Height > y)
                        {
                            if (x < w.Width)
                            {
                                m.SetRelativeMouse(0, -index);

                                return w;
                            }
                            break;
                        }

                        index += w.Height;
                    }
                    break;
            }

            // if nothing is hovered over
            return this;
        }

        protected override void RefreshVisual()
        {
            View.ClearText();
            View.FillColorScheme(new Core.ColorPair());

            int index = 0;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    foreach (Widget w in Children)
                    {
                        View.Merge(w.View, index, 0);

                        index += w.Width;
                        if (index > Width)
                            return;
                    }
                    return;
                case Orientation.Vertical:
                    foreach (Widget w in Children)
                    {
                        View.Merge(w.View, 0, index);

                        index += w.Height;
                        if (index > Height)
                            return;
                    }
                    return;
            }
        }

        protected override void DrawChild(Widget child)
        {
            int index = 0;
            foreach (Widget w in Children)
            {
                if (w == child)
                {
                    switch (Orientation)
                    {
                        case Orientation.Horizontal:
                            View.Merge(w.View, index, 0);
                            break;
                        case Orientation.Vertical:
                            View.Merge(w.View, 0, index);
                            break;
                        default:
                            throw new System.ComponentModel.InvalidEnumArgumentException();
                    }
                }
                else
                {
                    index += w.Bounds.GetSize(Orientation).ActualSize;
                }
            }
        }

        protected override void Resize() => CalculateLayout();

        protected override IEnumerable<Widget> GetLogicalChildren() => Children;

        protected override void Input(InputEventArgs e) { }

        private void AssertMinSizesFit()
        {
            int min = 0;
            foreach (Widget w in Children)
            {
                Size s = w.Bounds.GetSize(Orientation);
                if (s is FlexSize f)
                    min += f.Min;
                else
                    min += s.ActualSize;
            }

            if (min > OrientationSize)
                throw new InvalidOperationException("The given objects cannot fit!");
        }
    }
}
