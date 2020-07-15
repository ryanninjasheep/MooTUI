﻿using MooTUI.Core;
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
        public enum MainAxisJustification { START, CENTER, END, STRETCH, SPACE_BETWEEN, SPACE_AROUND }
        public enum CrossAxisJustification { START, CENTER, END }

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

        public void CalculateLayout()
        {
            AssertMinSizesFit();

            Lock = true;

            List<WidgetWithLocation> widgets = Children.Select((w) => new WidgetWithLocation(w)).ToList();
            List<FlexSize> flexible = new List<FlexSize>();

            foreach (WidgetWithLocation w in widgets)
            {
                SetIndividualCrossSize(w);

                if (w.Widget.Bounds.GetSizeInMainAxis(Orientation) is FlexSize f)
                {
                    f.Reset();
                    flexible.Add(f);
                }
            }

            int totalSpace = Children.Sum((s) => s.Bounds.GetSizeInMainAxis(Orientation).ActualSize);

            if (MainJustification == MainAxisJustification.START ||
                MainJustification == MainAxisJustification.CENTER ||
                MainJustification == MainAxisJustification.END ||
                MainJustification == MainAxisJustification.STRETCH)
            {
                if (MainJustification == MainAxisJustification.STRETCH ||
                    totalSpace > OrientationSize)
                    UpdateFlexSizeToFit(flexible, OrientationSize - totalSpace);

                int index = MainJustification switch
                {
                    MainAxisJustification.START => HJustification.LEFT.GetOffset(totalSpace, OrientationSize),
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
            else if (MainJustification == MainAxisJustification.SPACE_AROUND)
            {
                //TODO
            }
            else if (MainJustification == MainAxisJustification.SPACE_BETWEEN)
            {
                //TODO
            }
            else
            {
                throw new System.ComponentModel.InvalidEnumArgumentException();
            }

            ChildrenWithLocation = widgets;

            Lock = false;

            RefreshVisual();
        }

        public override Widget GetHoveredWidget(MouseContext m)
        {
            (int x, int y) = m.Mouse;

            foreach(WidgetWithLocation w in ChildrenWithLocation)
            {
                if (x > w.X && x < w.X + w.Widget.Width
                    && y > w.Y && y < w.Y + w.Widget.Height)
                {
                    m.SetRelativeMouse(-w.X, -w.Y);

                    return w.Widget;
                }
            }

            // if nothing is hovered over
            return this;
        }

        protected override void RefreshVisual()
        {
            Visual.FillCell(new Cell(' ', new ColorPair()));

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

        protected override void Input(InputEventArgs e) { }

        private void AssertMinSizesFit()
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
                    throw new InvalidOperationException("The given objects cannot fit!");
            }

            if (min > OrientationSize)
                throw new InvalidOperationException("The given objects cannot fit!");
        }

        private void SetIndividualCrossSize(WidgetWithLocation w)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    if (w.Widget.Bounds.HeightData is FlexSize g)
                    {
                        g.ActualSize = Height;
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
                        f.ActualSize = Width;
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

        private void UpdateFlexSizeToFit(List<FlexSize> flexible, int freeSpace)
        {
            while (freeSpace != 0 && flexible.Count > 0)
            {
                if (freeSpace < 0)
                    flexible = flexible
                        .Where((f) => f.ActualSize > f.Min)
                        .ToList();

                foreach (FlexSize f in flexible)
                {
                    float totalPreferred = flexible.Sum((f) => f.PreferredSize);

                    float sizeRatio = f.PreferredSize / totalPreferred;
                    float growth = sizeRatio * freeSpace;
                    int newSize = Math.Max(f.ActualSize + (int)Math.Round(growth), f.Min);

                    f.ActualSize = newSize;
                }

                freeSpace = OrientationSize -
                    Children.Sum((s) => s.Bounds.GetSizeInMainAxis(Orientation).ActualSize);
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
    }
}