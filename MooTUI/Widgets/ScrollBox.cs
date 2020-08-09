using MooTUI.Core;
using MooTUI.Input;
using MooTUI.Layout;
using MooTUI.Text;
using MooTUI.Widgets.Primitives;
using Keys = System.Windows.Input;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using MooTUI.IO;
using System;
using System.ComponentModel;

namespace MooTUI.Widgets
{
    public class ScrollBox : Box
    {
        public enum ScrollBarVisibility
        {
            /// <summary>
            /// Will throw an exception if the content is too large in this direction.
            /// </summary>
            DISABLED,

            /// <summary>
            /// If the content is larger than the viewer, the scrollbar will be visible.
            /// </summary>
            AUTO,

            /// <summary>
            /// The scrollbar will always be visible, even if the content is smaller than the viewer.
            /// </summary>
            VISIBLE,

            /// <summary>
            /// The scrollbar will never be visible, even if the content is larger than the viewer.
            /// </summary>
            HIDDEN
        }

        public ScrollBarVisibility HScrollBarVisibility { get; }
        public ScrollBarVisibility VScrollBarVisibility { get; }

        public int HorizontalOffset { get; private set; }
        public int VerticalOffset { get; private set; }

        private ScrollBar HScrollBar { get; }
        private ScrollBar VScrollBar { get; }

        private int ViewportWidth => Width - 2;
        private int ViewportHeight => Height - 2;

        private bool IsHScrollBarVisible => HScrollBarVisibility == ScrollBarVisibility.VISIBLE
            || (HScrollBarVisibility == ScrollBarVisibility.AUTO && Content.Width > ViewportWidth);
        private bool IsVScrollBarVisible => VScrollBarVisibility == ScrollBarVisibility.VISIBLE
            || (VScrollBarVisibility == ScrollBarVisibility.AUTO && Content.Height > ViewportHeight);

        private bool IsAtLeft => HorizontalOffset == 0;
        private bool IsAtRight => HorizontalOffset + ViewportWidth == Content.Width;
        private bool IsAtTop => VerticalOffset == 0;
        private bool IsAtBottom => VerticalOffset + ViewportHeight == Content.Height;

        public ScrollBox(LayoutRect bounds, Widget w,
            ScrollBarVisibility hScrollbarVisibility = ScrollBarVisibility.AUTO,
            ScrollBarVisibility vScrollbarVisibility = ScrollBarVisibility.AUTO,
            string text = "", BoxDrawing lineStyle = null)
            : base(bounds, w, text, lineStyle)
        {
            HScrollBarVisibility = hScrollbarVisibility;
            VScrollBarVisibility = vScrollbarVisibility;

            bounds.AssertMinSize(
                HScrollBarVisibility == ScrollBarVisibility.DISABLED ? 3 : 5,
                VScrollBarVisibility == ScrollBarVisibility.DISABLED ? 3 : 5);

            if (HScrollBarVisibility != ScrollBarVisibility.DISABLED)
            {
                HScrollBar = ScrollBar.Factory(Orientation.Horizontal, ViewportWidth, this);
                LinkChild(HScrollBar);
            }
            if (VScrollBarVisibility != ScrollBarVisibility.DISABLED)
            {
                VScrollBar = ScrollBar.Factory(Orientation.Vertical, ViewportHeight, this);
                LinkChild(VScrollBar);
            }

            TryStretchContent();

            CalculateScrollInfo();
        }

        #region SCROLLING

        private void CalculateScrollInfo()
        {
            if ((VScrollBarVisibility == ScrollBarVisibility.DISABLED && Content.Height > ViewportHeight) ||
                (HScrollBarVisibility == ScrollBarVisibility.DISABLED && Content.Width > ViewportWidth))
                throw new SizeException(
                    "If the scrollbar is disabled in a certain dimension, content cannot be larger than the viewer!");

            RefreshHScrollInfo();
            RefreshVScrollInfo();
        }
        private void RefreshHScrollInfo()
        {
            if (IsHScrollBarVisible)
                HScrollBar.LoadScrollInfo(HorizontalOffset, Content.Width, ViewportWidth);
        }
        private void RefreshVScrollInfo()
        {
            if (IsVScrollBarVisible)
                VScrollBar.LoadScrollInfo(VerticalOffset, Content.Height, ViewportHeight);
        }

        public bool ScrollX(int amount)
        {
            if (amount < 0 && IsAtLeft || amount > 0 && IsAtRight)
                return false;

            SetHOffset(HorizontalOffset + amount);

            RefreshHScrollInfo();

            DrawChild(Content);
            Render();

            return true;
        }
        public bool ScrollY(int amount)
        {
            if (amount < 0 && IsAtTop || amount > 0 && IsAtBottom)
                return false;

            SetVOffset(VerticalOffset + amount);

            RefreshVScrollInfo();

            DrawChild(Content);
            Render();

            return true;
        }

        public bool MinScroll()
        {
            if (IsAtLeft && IsAtTop)
                return false;

            HorizontalOffset = 0;
            VerticalOffset = 0;

            RefreshHScrollInfo();
            RefreshVScrollInfo();

            DrawChild(Content);
            Render();

            return true;
        }
        public bool MaxScroll()
        {
            if (IsAtRight && IsAtBottom)
                return false;

            HorizontalOffset = Content.Width - ViewportWidth;
            if (HorizontalOffset < 0)
                HorizontalOffset = 0;

            VerticalOffset = Content.Height - ViewportHeight;
            if (VerticalOffset < 0)
                VerticalOffset = 0;

            RefreshHScrollInfo();
            RefreshVScrollInfo();

            DrawChild(Content);
            Render();

            return true;
        }

        public bool PageUp() => ScrollY(-Height);
        public bool PageDown() => ScrollY(Height);
        public bool PageLeft() => ScrollX(-Width);
        public bool PageRight() => ScrollX(Width);

        /// <summary>
        /// Returns false if point is already within view.
        /// </summary>
        public bool ScrollToPoint(int x, int y)
        {
            if ((x < 0 || x > Content.Width) || (y < 0 || y > Content.Height))
                throw new ArgumentOutOfRangeException("The given point is outside the bounds of the content");

            if (IsPointInView(x, y))
                return false;

            if (x < HorizontalOffset)
                SetHOffset(x);
            else if (x >= HorizontalOffset + ViewportWidth)
                SetHOffset(x - ViewportWidth + 1);

            if (y < VerticalOffset)
                SetVOffset(y);
            else if (y >= VerticalOffset + ViewportHeight)
                SetVOffset(y - ViewportHeight + 1);

            RefreshHScrollInfo();
            RefreshVScrollInfo();

            DrawChild(Content);
            Render();

            return true;
        }

        private bool IsPointInView(int x, int y) =>
            (x >= HorizontalOffset && x < HorizontalOffset + ViewportWidth) &&
            (y >= VerticalOffset && y < VerticalOffset + ViewportHeight);

        private void SetHOffset(int x)
        {
            HorizontalOffset = x;

            if (HorizontalOffset + ViewportWidth > Content.Width)
                HorizontalOffset = Content.Width - ViewportWidth;

            if (HorizontalOffset < 0)
                HorizontalOffset = 0;
        }
        private void SetVOffset(int y)
        {
            VerticalOffset = y;

            if (VerticalOffset + ViewportHeight > Content.Height)
                VerticalOffset = Content.Height - ViewportHeight;

            if (VerticalOffset < 0)
                VerticalOffset = 0;
        }

        #endregion SCROLLING

        protected override IEnumerable<Widget> GetLogicalChildren()
        {
            List<Widget> toReturn = new List<Widget>();

            if (Content != null)
                toReturn.Add(Content);
            if (HScrollBar != null)
                toReturn.Add(HScrollBar);
            if (VScrollBar != null)
                toReturn.Add(VScrollBar);

            return toReturn;
        }

        public override Widget GetHoveredWidget((int x, int y) relativeMouseLocation)
        {
            (int x, int y) = relativeMouseLocation;

            if (x > 0 && x < 1 + ViewportWidth && y > 0 && y < 1 + ViewportHeight
                && Content.HitTest(x + HorizontalOffset - 1, y + VerticalOffset - 1))
            {
                return Content;
            }
            else if (IsHScrollBarVisible && y == Height - 1 && x > 0 && x < 1 + ViewportWidth)
            {
                return HScrollBar;
            }
            else if (IsVScrollBarVisible && x == Width - 1 && y > 0 && y < 1 + ViewportHeight)
            {
                return VScrollBar;
            }

            return this;
        }

        protected internal override (int xOffset, int yOffset) GetChildOffset(Widget child)
        {
            if (child == Content)
            {
                return (1 - HorizontalOffset, 1 - VerticalOffset);
            }
            else if (child == HScrollBar)
            {
                return (1, Height - 1);
            }
            else if (child == VScrollBar)
            {
                return (Width - 1, 1);
            }
            throw new ArgumentException("Not a child of this container", nameof(child));
        }

        protected override void DrawChild(Widget child)
        {
            if (child == Content)
                Visual.Merge(child.Visual, 1, 1, HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);
            else if (child == HScrollBar)
                Visual.Merge(child.Visual, 1, Height - 1);
            else if (child == VScrollBar)
                Visual.Merge(child.Visual, Width - 1, 1);
        }

        protected override void OnChildResized(Widget child)
        {
            CalculateScrollInfo();

            MinScroll();

            RefreshVisual();
            Render();
        }

        protected override void EnsureRegionVisible(int x, int y, int width = 1, int height = 1)
        {
            if (width > 1 || height > 1)
                ScrollToPoint(x + HorizontalOffset + width - 2, y + VerticalOffset + height - 2);

            ScrollToPoint(x + HorizontalOffset - 1, y + VerticalOffset - 1);

            base.EnsureRegionVisible(x, y, width, height);
        }

        protected override void Resize()
        {
            Lock = true;

            if (HScrollBar != null && HScrollBar.Bounds.WidthData is FlexSize w)
                w.TryResize(Width - 2);
            if (VScrollBar != null && VScrollBar.Bounds.HeightData is FlexSize h)
                h.TryResize(Height - 2);

            TryStretchContent();

            Lock = false;

            CalculateScrollInfo();
        }

        protected override void RefreshVisual()
        {
            Visual.FillCell(new Cell(' ', Style.GetColorPair("Default")));

            DrawChild(Content);

            DrawOutline();
            DrawText();
            DrawScrollBars();
        }

        protected override void Input(InputEventArgs e)
        {
            if (e.Handled)
                return;

            switch (e)
            {
                case KeyboardInputEventArgs k:
                    OnKeyDown(k);
                    break;
                case ScrollInputEventArgs s:
                    OnScroll(s);
                    break;
                default:
                    // ok
                    break;
            }
        }

        private void OnScroll(ScrollInputEventArgs s)
        {
            int delta = s.Delta;

            if (s.Shift || !IsVScrollBarVisible)
            {
                if (delta < 0)
                    s.Handled = ScrollX(s.Ctrl ? -5 : -1);
                else
                    s.Handled = ScrollX(s.Ctrl ? 5 : 1);
            }
            else
            {
                if (delta < 0)
                    s.Handled = ScrollY(s.Ctrl ? 5 : 1);
                else
                    s.Handled = ScrollY(s.Ctrl ? -5 : -1);
            }
        }

        private void OnKeyDown(KeyboardInputEventArgs k)
        {
            switch (k.Key)
            {
                case Keys.Key.PageUp:
                    k.Handled = PageUp();
                    return;
                case Keys.Key.PageDown:
                    k.Handled = PageDown();
                    return;
                case Keys.Key.Home:
                    k.Handled = MinScroll();
                    return;
                case Keys.Key.End:
                    k.Handled = MaxScroll();
                    return;
                case Keys.Key.Left:
                    k.Handled = ScrollX(-1);
                    return;
                case Keys.Key.Right:
                    k.Handled = ScrollX(1);
                    return;
                case Keys.Key.Up:
                    k.Handled = ScrollY(-1);
                    return;
                case Keys.Key.Down:
                    k.Handled = ScrollY(1);
                    return;
                default:
                    return;
            }
        }

        private void DrawOutline()
        {
            LineStyle.DrawBox(Visual, Width, Height);
        }

        private void DrawText()
        {
            if (Text.Text.Length > 0)
                Visual.Merge(Enclosure.DrawEnclosure(Text),
                    1, 0,
                    0, 0,
                    Width, 1);
        }

        private void DrawScrollBars()
        {
            if (IsHScrollBarVisible)
                DrawChild(HScrollBar);
            if (IsVScrollBarVisible)
                DrawChild(VScrollBar);
        }

        private void TryStretchContent()
        {
            Content.Bounds.TryResize(ViewportWidth, ViewportHeight);
        }

        /// <summary>
        /// A ScrollBar has no knowledge of the actual size of the content; it merely knows 
        /// its own size and stuff for rendering.  These things must be updated any time the
        /// ScrollViewer's view changes.
        /// </summary>
        private class ScrollBar : Widget
        {
            private Orientation Orientation { get; set; }
            private int Length { get; set; }

            private enum HoverRegion { NONE, LESS_BUTTON, LESS_TRACK, GRIP, MORE_TRACK, MORE_BUTTON }
            private HoverRegion Region { get; set; }

            private int TrackLength { get; set; }

            private int GripStart { get; set; }
            private int GripLength { get; set; }

            // Yes, I know this is a circular reference but it's okay because the existence of a scrollbar
            // is very controlled.  They will exist only in the context of a scrollviewer.
            private ScrollBox Viewer { get; set; }

            /// <summary>
            /// Only called by factory!
            /// </summary>
            private ScrollBar(LayoutRect bounds) : base(bounds)
            {
                Region = HoverRegion.NONE;
            }

            public static ScrollBar Factory(Orientation orientation, int length, ScrollBox parent)
            {
                if (length < 3)
                    throw new SizeException("A ScrollBar must be at least 3 cells long!");

                ScrollBar s;

                if (orientation == Orientation.Horizontal)
                    s = new ScrollBar(
                            new LayoutRect(
                                new FlexSize(3, length),
                                new Size(1)));
                else if (orientation == Orientation.Vertical)
                    s = new ScrollBar(
                            new LayoutRect(
                                new Size(1),
                                new FlexSize(3, length)));
                else
                    throw new InvalidEnumArgumentException();

                s.Orientation = orientation;
                s.Length = length;
                s.Viewer = parent;

                return s;
            }

            protected override void Resize()
            {
                Length = Math.Max(Width, Height);
            }

            public void LoadScrollInfo(int offset, int contentLength, int viewerLength)
            {
                TrackLength = Length - 2;

                float windowContentRatio = (float)Length / contentLength;

                GripLength = (int)(TrackLength * windowContentRatio);

                if (GripLength < 1)
                    GripLength = 1;
                if (GripLength > TrackLength)
                    GripLength = TrackLength;

                int scrollAreaSize = contentLength - viewerLength;
                float windowPositionRatio = (float)offset / scrollAreaSize;
                int trackScrollAreaSize = TrackLength - GripLength;
                int gripPositionOnTrack = (int)(trackScrollAreaSize * windowPositionRatio);

                GripStart = Math.Max(gripPositionOnTrack + 1, 1);

                RefreshVisual();
                Render();
            }

            #region DISPLAY CONSTANTS
            private const char HTrackChar = '─';
            private const char VTrackChar = '│';
            private const char HGripChar = '█';
            private const char VGripChar = '█';
            private const char HLessChar = '˂';
            private const char HMoreChar = '˃';
            private const char VLessChar = '˄';
            private const char VMoreChar = '˅';
            #endregion DISPLAY CONSTANTS

            protected override void RefreshVisual()
            {
                DrawTrack();
                DrawGrip();
            }

            private void DrawTrack()
            {
                if (Orientation == Orientation.Horizontal)
                {
                    Visual.FillChar(HTrackChar);

                    Visual.SetChar(0, 0, HLessChar);
                    Visual.SetChar(Width - 1, Height - 1, HMoreChar);
                }
                else if (Orientation == Orientation.Vertical)
                {
                    Visual.FillChar(VTrackChar);

                    Visual.SetChar(0, 0, VLessChar);
                    Visual.SetChar(Width - 1, Height - 1, VMoreChar);
                }
            }
            private void DrawGrip()
            {
                if (Orientation == Orientation.Horizontal)
                {
                    Visual.FillChar(HGripChar, GripStart, 0, GripLength, 1);
                }
                else if (Orientation == Orientation.Vertical)
                {
                    Visual.FillChar(VGripChar, 0, GripStart, 1, GripLength);
                }
            }
            private void Colorize()
            {
                Visual.FillColors(Style.GetColorPair("Default"));

                switch (Region)
                {
                    case HoverRegion.LESS_BUTTON:
                        Visual.SetColors(0, 0, Style.GetColorPair("Hover"));
                        break;
                    case HoverRegion.LESS_TRACK:
                        if (Orientation == Orientation.Horizontal)
                            Visual.FillColors(Style.GetColorPair("Hover"), 1, 0, GripStart - 1, 1);
                        else if (Orientation == Orientation.Vertical)
                            Visual.FillColors(Style.GetColorPair("Hover"), 0, 1, 1, GripStart - 1);
                        break;
                    case HoverRegion.GRIP:
                        if (Orientation == Orientation.Horizontal)
                            Visual.FillColors(Style.GetColorPair("Hover"), GripStart, 0, GripLength, 1);
                        else if (Orientation == Orientation.Vertical)
                            Visual.FillColors(Style.GetColorPair("Hover"), 0, GripStart, 1, GripLength);
                        break;
                    case HoverRegion.MORE_TRACK:
                        if (Orientation == Orientation.Horizontal)
                            Visual.FillColors(Style.GetColorPair("Hover"), GripStart + GripLength, 0, 
                                Width - GripStart - GripLength - 1, 1);
                        else if (Orientation == Orientation.Vertical)
                            Visual.FillColors(Style.GetColorPair("Hover"), 0, GripStart + GripLength,
                                1, Height - GripStart - GripLength - 1);
                        break;
                    case HoverRegion.MORE_BUTTON:
                        Visual.SetColors(Width - 1, Height - 1, Style.GetColorPair("Hover"));
                        break;
                    case HoverRegion.NONE:
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }

            protected override void Input(InputEventArgs e)
            {
                switch (e)
                {
                    case MouseMoveInputEventArgs m:
                        OnMouseMove(m);
                        break;
                    case MouseLeaveInputEventArgs _:
                        OnMouseLeave();
                        break;
                    case MouseClickInputEventArgs c:
                        if (c.Button == MouseClickInputEventArgs.MouseButton.LEFT)
                            OnLeftClick(c);
                        break;
                    case ScrollInputEventArgs s:
                        OnScroll(s);
                        break;
                    default:
                        // ok
                        break;
                }
            }

            private void OnMouseMove(MouseMoveInputEventArgs m)
            {
                int index = Orientation switch
                {
                    Orientation.Horizontal => m.Location.X,
                    Orientation.Vertical => m.Location.Y,
                    _ => throw new InvalidEnumArgumentException(),
                };

                if (index == 0)
                {
                    Region = HoverRegion.LESS_BUTTON;
                }
                else if (index == Length - 1)
                {
                    Region = HoverRegion.MORE_BUTTON;
                }
                else if (index < GripStart)
                {
                    Region = HoverRegion.LESS_TRACK;
                }
                else if (index > GripStart + GripLength - 1)
                {
                    Region = HoverRegion.MORE_TRACK;
                }
                else if (index >= GripStart && index <= GripStart + GripLength - 1)
                {
                    Region = HoverRegion.GRIP;
                }
                else
                {
                    Region = HoverRegion.NONE;
                }

                Colorize();
                Render();
            }

            private void OnMouseLeave()
            {
                Region = HoverRegion.NONE;

                Colorize();
                Render();
            }

            private void OnLeftClick(MouseClickInputEventArgs c)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (Region == HoverRegion.LESS_BUTTON)
                        c.Handled = Viewer.ScrollX(-1);
                    else if (Region == HoverRegion.MORE_BUTTON)
                        c.Handled = Viewer.ScrollX(1);
                    else if (Region == HoverRegion.LESS_TRACK)
                        c.Handled = Viewer.PageLeft();
                    else if (Region == HoverRegion.MORE_TRACK)
                        c.Handled = Viewer.PageRight();
                }
                else if (Orientation == Orientation.Vertical)
                {
                    if (Region == HoverRegion.LESS_BUTTON)
                        c.Handled = Viewer.ScrollY(-1);
                    else if (Region == HoverRegion.MORE_BUTTON)
                        c.Handled = Viewer.ScrollY(1);
                    else if (Region == HoverRegion.LESS_TRACK)
                        c.Handled = Viewer.PageUp();
                    else if (Region == HoverRegion.MORE_TRACK)
                        c.Handled = Viewer.PageDown();
                }
            }

            private void OnScroll(ScrollInputEventArgs s)
            {
                int delta = s.Delta;

                if (Orientation == Orientation.Horizontal)
                {
                    if (delta < 0)
                        Viewer.ScrollX(-1);
                    else
                        Viewer.ScrollX(1);
                }
                else if (Orientation == Orientation.Vertical)
                {
                    if (delta < 0)
                        Viewer.ScrollY(1);
                    else
                        Viewer.ScrollY(-1);
                }

                s.Handled = true;
            }
        }
    }
}
