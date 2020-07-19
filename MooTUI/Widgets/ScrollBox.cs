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

        public ScrollBarVisibility HScrollBarVisibility { get; private set; }
        public ScrollBarVisibility VScrollBarVisibility { get; private set; }

        public int HorizontalOffset { get; private set; }
        public int VerticalOffset { get; private set; }

        private ScrollBar HScrollBar { get; set; }
        private ScrollBar VScrollBar { get; set; }

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
            bounds.AssertMinSize(5, 5);

            HScrollBarVisibility = hScrollbarVisibility;
            VScrollBarVisibility = vScrollbarVisibility;

            HScrollBar = ScrollBar.Factory(Orientation.Horizontal, ViewportWidth, this);
            VScrollBar = ScrollBar.Factory(Orientation.Vertical, ViewportHeight, this);

            LinkChild(HScrollBar);
            LinkChild(VScrollBar);

            CalculateScrollInfo();
        }

        #region SCROLLING

        private void CalculateScrollInfo()
        {
            if ((VScrollBarVisibility == ScrollBarVisibility.DISABLED && Content.Height > Height) ||
                (HScrollBarVisibility == ScrollBarVisibility.DISABLED && Content.Width > Width))
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

        public override Widget GetHoveredWidget(MouseContext m)
        {
            (int x, int y) = m.Mouse;

            if (x > 0 && x < 1 + ViewportWidth && y > 0 && y < 1 + ViewportHeight)
            {
                m.SetRelativeMouse(HorizontalOffset - 1, VerticalOffset - 1);
                return Content;
            }
            else if (IsHScrollBarVisible && y == Height - 1 && x > 0 && x < 1 + ViewportWidth)
            {
                m.SetRelativeMouse(-1, -(Height - 1));
                return HScrollBar;
            }
            else if (IsVScrollBarVisible && x == Width - 1 && y > 0 && y < 1 + ViewportHeight)
            {
                m.SetRelativeMouse(-(Width - 1), -1);
                return VScrollBar;
            }

            return this;
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

            Render();
        }

        protected override void Resize()
        {
            Lock = true;

            HScrollBar.Bounds.TryResize(Width - 2, 1);
            VScrollBar.Bounds.TryResize(1, Height - 2);

            Lock = false;
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

            switch (e.InputType)
            {
                case InputTypes.KEY_DOWN:
                    OnKeyDown(e);
                    break;
                case InputTypes.SCROLL:
                    OnScroll(e);
                    break;
                default:
                    // ok
                    break;
            }
        }

        private void OnScroll(InputEventArgs e)
        {
            int delta = e.Mouse.ScrollDelta;

            if (e.Keyboard.Shift || !IsVScrollBarVisible)
            {
                if (delta < 0)
                    e.Handled = ScrollX(-1);
                else
                    e.Handled = ScrollX(1);
            }
            else
            {
                if (delta < 0)
                    e.Handled = ScrollY(1);
                else
                    e.Handled = ScrollY(-1);
            }
        }

        private void OnKeyDown(InputEventArgs e)
        {
            switch (e.Keyboard.LastKeyPressed)
            {
                case Keys.Key.PageUp:
                    e.Handled = PageUp();
                    return;
                case Keys.Key.PageDown:
                    e.Handled = PageDown();
                    return;
                case Keys.Key.Home:
                    e.Handled = MinScroll();
                    return;
                case Keys.Key.End:
                    e.Handled = MaxScroll();
                    return;
                case Keys.Key.Left:
                    e.Handled = ScrollX(-1);
                    return;
                case Keys.Key.Right:
                    e.Handled = ScrollX(1);
                    return;
                case Keys.Key.Up:
                    e.Handled = ScrollY(-1);
                    return;
                case Keys.Key.Down:
                    e.Handled = ScrollY(1);
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
            private ScrollBar(int width, int height) : base(new LayoutRect(width, height))
            {
                Region = HoverRegion.NONE;
            }

            public static ScrollBar Factory(Orientation orientation, int length, ScrollBox parent)
            {
                if (length < 3)
                    throw new SizeException("A ScrollBar must be at least 3 cells long!");

                ScrollBar s;

                if (orientation == Orientation.Horizontal)
                    s = new ScrollBar(length, 1);
                else if (orientation == Orientation.Vertical)
                    s = new ScrollBar(1, length);
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
                        break;
                    case HoverRegion.GRIP:
                        if (Orientation == Orientation.Horizontal)
                            Visual.FillColors(Style.GetColorPair("Hover"), GripStart, 0, GripLength, 1);
                        else if (Orientation == Orientation.Vertical)
                            Visual.FillColors(Style.GetColorPair("Hover"), 0, GripStart, 1, GripLength);
                        break;
                    case HoverRegion.MORE_TRACK:
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
                switch (e.InputType)
                {
                    case InputTypes.MOUSE_MOVE:
                        OnMouseMove(e);
                        break;
                    case InputTypes.MOUSE_LEAVE:
                        OnMouseLeave();
                        break;
                    case InputTypes.LEFT_CLICK:
                        OnLeftClick(e);
                        break;
                    case InputTypes.SCROLL:
                        OnScroll(e);
                        break;
                    default:
                        // ok
                        break;
                }
            }

            private void OnMouseMove(InputEventArgs e)
            {
                int index = Orientation switch
                {
                    Orientation.Horizontal => e.Mouse.Mouse.X,
                    Orientation.Vertical => e.Mouse.Mouse.Y,
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

            private void OnLeftClick(InputEventArgs e)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (Region == HoverRegion.LESS_BUTTON)
                        e.Handled = Viewer.ScrollX(-1);
                    else if (Region == HoverRegion.MORE_BUTTON)
                        e.Handled = Viewer.ScrollX(1);
                    else if (Region == HoverRegion.LESS_TRACK)
                        e.Handled = Viewer.PageLeft();
                    else if (Region == HoverRegion.MORE_TRACK)
                        e.Handled = Viewer.PageRight();
                }
                else if (Orientation == Orientation.Vertical)
                {
                    if (Region == HoverRegion.LESS_BUTTON)
                        e.Handled = Viewer.ScrollY(-1);
                    else if (Region == HoverRegion.MORE_BUTTON)
                        e.Handled = Viewer.ScrollY(1);
                    else if (Region == HoverRegion.LESS_TRACK)
                        e.Handled = Viewer.PageUp();
                    else if (Region == HoverRegion.MORE_TRACK)
                        e.Handled = Viewer.PageDown();
                }
            }

            private void OnScroll(InputEventArgs e)
            {
                int delta = e.Mouse.ScrollDelta;

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

                e.Handled = true;
            }
        }
    }
}
