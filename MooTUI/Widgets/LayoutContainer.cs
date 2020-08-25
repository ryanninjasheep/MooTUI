using MooTUI.Drawing;
using MooTUI.Input;
using MooTUI.Layout;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MooTUI.Widgets
{
    public class LayoutContainer : Container
    {
        public enum MainAxisJustification { START, CENTER, END, STRETCH, FIT }
        public enum CrossAxisJustification { START, CENTER, END, STRETCH }

        public List<Widget> Children { get; private set; }
        private List<WidgetWithLocation> ChildrenWithLocation { get; set; }

        public Orientation Orientation { get; }

        private MainAxisJustification _mainJustification;
        private CrossAxisJustification _crossJustification;

        public MainAxisJustification MainJustification
        {
            get => _mainJustification;
            set
            {
                _mainJustification = value;
                CalculateLayout();
            }
        }
        public CrossAxisJustification CrossJustification
        {
            get => _crossJustification;
            set
            {
                _crossJustification = value;
                CalculateLayout();
            }
        }

        private int OrientationSize => Bounds.GetSizeInMainAxis(Orientation).ActualSize;

        public LayoutContainer(LayoutRect bounds, Orientation oritentation,
            MainAxisJustification mainJustification = MainAxisJustification.STRETCH,
            CrossAxisJustification crossJustification = CrossAxisJustification.CENTER) 
            : base(bounds)
        {
            Orientation = oritentation;
            Children = new List<Widget>();
            ChildrenWithLocation = new List<WidgetWithLocation>();
            _mainJustification = mainJustification;
            _crossJustification = crossJustification;
        }

        public void AddChild(Widget w)
        {
            LinkChild(w);
            Children.Add(w);
            CalculateLayout();

            w.Render();
        }
        public void InsertChild(Widget w, int index)
        {
            LinkChild(w);
            Children.Insert(index, w);
            CalculateLayout();

            w.Render();
        }
        public void RemoveChild(Widget w)
        {
            if (!Children.Contains(w))
                return;

            UnlinkChild(w);
            Children.Remove(w);
            CalculateLayout();

            Render();
        }

        public void CalculateLayout()
        {
            if (Lock)
                return;

            int totalSpace = GetMinContentSize();

            if (MainJustification == MainAxisJustification.FIT
                && Bounds.GetSizeInMainAxis(Orientation) is FlexSize size)
            {
                Lock = true;
                size.SetMin(Math.Max(totalSpace, 1));
            }

            AssertMinSizesFit();

            Lock = true;

            List<WidgetWithLocation> widgets = Children.Select((w) => new WidgetWithLocation(w)).ToList();

            foreach (WidgetWithLocation w in widgets)
            {
                SetIndividualCrossSize(w);
            }

            if (MainJustification == MainAxisJustification.START 
                || MainJustification == MainAxisJustification.FIT
                || MainJustification == MainAxisJustification.CENTER 
                || MainJustification == MainAxisJustification.END 
                || MainJustification == MainAxisJustification.STRETCH)
            {
                if (MainJustification == MainAxisJustification.STRETCH &&
                    totalSpace < OrientationSize)
                    UpdateSizesToFit(Children, OrientationSize, Orientation);

                int index = MainJustification switch
                {
                    MainAxisJustification.START => HJustification.LEFT.GetOffset(totalSpace, OrientationSize),
                    MainAxisJustification.FIT => HJustification.LEFT.GetOffset(totalSpace, OrientationSize),
                    MainAxisJustification.CENTER => HJustification.CENTER.GetOffset(totalSpace, OrientationSize),
                    MainAxisJustification.END => HJustification.RIGHT.GetOffset(totalSpace, OrientationSize),
                    MainAxisJustification.STRETCH => 0,
                    _ => throw new NotImplementedException(),
                };

                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        foreach (WidgetWithLocation w in widgets)
                        {
                            w.X = index;
                            index += w.Widget.Width;
                        }
                        break;
                    case Orientation.Vertical:
                        foreach (WidgetWithLocation w in widgets)
                        {
                            w.Y = index;
                            index += w.Widget.Height;
                        }
                        break;
                    default:
                        throw new System.ComponentModel.InvalidEnumArgumentException();
                }
            }
            else
            {
                throw new System.ComponentModel.InvalidEnumArgumentException();
            }

            ChildrenWithLocation = widgets;

            Lock = false;

            OnLayoutUpdated(EventArgs.Empty);

            RefreshVisual();
        }

        public override Widget GetHoveredWidget((int x, int y) relativeMouseLocation)
        {
            (int x, int y) = relativeMouseLocation;

            foreach (WidgetWithLocation w in ChildrenWithLocation)
            {
                if (x >= w.X && x < w.X + w.Widget.Width
                    && y >= w.Y && y < w.Y + w.Widget.Height)
                {
                    return w.Widget;
                }
            }

            // if nothing is hovered over
            return this;
        }

        protected override void RefreshVisual()
        {
            Visual.FillCell(new Cell(' ', Style.GetColorPair("Default")));

            foreach (WidgetWithLocation w in ChildrenWithLocation)
            {
                Visual.Merge(w.Widget.Visual, w.X, w.Y);
            }
        }

        protected override void Resize() => CalculateLayout();

        protected override IEnumerable<Widget> GetLogicalChildren() => Children;

        protected override void DrawChild(Widget child)
        {
            foreach (WidgetWithLocation w in ChildrenWithLocation)
            {
                if (w.Widget == child)
                    Visual.Merge(w.Widget.Visual, w.X, w.Y);
            }
        }

        protected override void OnChildResized(Widget child) => CalculateLayout();

        protected internal override (int xOffset, int yOffset) GetChildOffset(Widget child)
        {
            foreach(WidgetWithLocation w in ChildrenWithLocation)
            {
                if (w.Widget == child)
                    return (w.X, w.Y);
            }

            throw new ArgumentException("Not a child of this container!");
        }

        protected override void Input(InputEventArgs e) { }

        private void AssertMinSizesFit()
        {
            if (GetMinContentSize() > OrientationSize)
                throw new SizeException("The given Widgets will not fit in this LayoutContainer.");
        }

        private int GetMinContentSize()
        {
            int min = 0;
            foreach (Widget w in Children)
            {
                Size m = w.Bounds.GetSizeInMainAxis(Orientation);
                if (m is FlexSize f)
                    min += f.Min;
                else
                    min += m.ActualSize;

                Size c = w.Bounds.GetSizeInCrossAxis(Orientation);

                if ((c is FlexSize g && g.Min > Bounds.GetSizeInCrossAxis(Orientation).ActualSize) ||
                    (!(c is FlexSize) && c.ActualSize > Bounds.GetSizeInCrossAxis(Orientation).ActualSize))
                    throw new SizeException("The given Widgets will not fit in this LayoutContainer.");
            }

            return min;
        }

        private void SetIndividualCrossSize(WidgetWithLocation w)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    if (w.Widget.Bounds.HeightData is FlexSize g)
                    {
                        g.TryResize(Height);
                        w.Y = 0;
                    }
                    else
                    {
                        w.Y = CrossJustification switch
                        {
                            CrossAxisJustification.START =>
                                HJustification.LEFT.GetOffset(w.Widget.Height, Height),
                            CrossAxisJustification.CENTER =>
                                HJustification.CENTER.GetOffset(w.Widget.Height, Height),
                            CrossAxisJustification.END =>
                                HJustification.RIGHT.GetOffset(w.Widget.Height, Height),
                            _ => throw new System.ComponentModel.InvalidEnumArgumentException(),
                        };
                    }
                    break;
                case Orientation.Vertical:
                    if (w.Widget.Bounds.WidthData is FlexSize f)
                    {
                        f.TryResize(Width);
                        w.X = 0;
                    }
                    else
                    {
                        w.X = CrossJustification switch
                        {
                            CrossAxisJustification.START =>
                                HJustification.LEFT.GetOffset(w.Widget.Width, Width),
                            CrossAxisJustification.CENTER =>
                                HJustification.CENTER.GetOffset(w.Widget.Width, Width),
                            CrossAxisJustification.END =>
                                HJustification.RIGHT.GetOffset(w.Widget.Width, Width),
                            _ => throw new System.ComponentModel.InvalidEnumArgumentException(),
                        };
                    }
                    break;
                default:
                    throw new System.ComponentModel.InvalidEnumArgumentException();
            }
        }

        private static void UpdateSizesToFit(List<Widget> toResize, int totalSpace, Orientation orientation)
        {
            // Step 1: setup data
            int freeSpace = totalSpace;
            int totalMin = 0;
            List<SizeHelper> flexible = new List<SizeHelper>();
            foreach (Widget w in toResize)
            {
                Size s = w.Bounds.GetSizeInMainAxis(orientation);
                if (s is FlexSize f)
                {
                    flexible.Add(new SizeHelper(f));
                    freeSpace -= f.Min;
                    totalMin += f.Min;
                }
                else
                {
                    freeSpace -= s.ActualSize;
                }
            }
            if (flexible.Count == 0)
                return;
            flexible = flexible.OrderBy((s) => s.Min).ToList();

            // Step 2: floor function
            int totalGrowth = 0;
            foreach (SizeHelper s in flexible)
            {
                float sizeRatio = (float)s.Min / totalMin;
                float growth = sizeRatio * freeSpace; // should be positive bc freeSpace is positive
                int intGrowth = (int)Math.Floor(growth);

                s.Curr += intGrowth;
                totalGrowth += intGrowth;
            }
            freeSpace -= totalGrowth;

            // Step 3: distribute remaining space
            for (int i = 0; i < freeSpace; i++)
            {
                // Because flexible was already sorted, this will be distributed to the largest ones first
                flexible[i % flexible.Count].Curr += 1;
            }

            // Step 4: zip
            foreach (SizeHelper s in flexible)
            {
                s.Zip();
            }
        }

        private class WidgetWithLocation
        {
            public Widget Widget { get; }

            public int X { get; set; }
            public int Y { get; set; }

            public WidgetWithLocation(Widget w)
            {
                Widget = w;
            }
        }
        private class SizeHelper
        {
            private FlexSize size;

            public int Min { get; }
            public int Curr { get; set; }

            public SizeHelper(FlexSize f)
            {
                size = f;
                Min = f.Min;
                Curr = f.Min;
            }

            public void Zip() => size.TryResize(Curr);
        }
    }

    public static class LayoutExtensions
    {
        public static Size GetSizeInMainAxis(this LayoutRect r, Orientation orientation) => orientation switch
        {
            Orientation.Horizontal => r.WidthData,
            Orientation.Vertical => r.HeightData,
            _ => throw new System.ComponentModel.InvalidEnumArgumentException(),
        };
        public static Size GetSizeInCrossAxis(this LayoutRect r, Orientation orientation) => orientation switch
        {
            Orientation.Horizontal => r.HeightData,
            Orientation.Vertical => r.WidthData,
            _ => throw new System.ComponentModel.InvalidEnumArgumentException(),
        };
    }
}
