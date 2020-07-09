using MooTUI.OldWidgets.Primitives;
using MooTUI.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows.Input;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using MooTUI.IO;

namespace MooTUI.OldWidgets
{
    /// <summary>
    /// Automatically creates ScrollBars - make sure the specified width and height
    /// is one greater than intended viewport size.
    /// </summary>
    public class ScrollViewer : MonoContainer
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
        public ScrollBarVisibility VScrollBarVisibility { get; private set; }
        public ScrollBarVisibility HScrollBarVisibility { get; private set; }

        public int HorizontalStart { get; private set; }
        public int VerticalStart { get; private set; }

        private ScrollBar HScrollBar { get; set; }
        private ScrollBar VScrollBar { get; set; }

        private int ViewportWidth => 
            (VScrollBarVisibility == ScrollBarVisibility.DISABLED) ? Width : Width - 1;
        private int ViewportHeight =>
            (HScrollBarVisibility == ScrollBarVisibility.DISABLED) ? Height : Height - 1;

        public ScrollViewer(int width, int height, Widget content, 
            ScrollBarVisibility hScrollBarVisibility, ScrollBarVisibility vScrollBarVisibility) : base(width, height)
        {
            SetContent(content);

            if (Content is IEnsureCursorVisible i)
            {
                i.EnsureCursorVisible += Content_EnsureCursorVisible;
            }

            HScrollBarVisibility = hScrollBarVisibility;
            VScrollBarVisibility = vScrollBarVisibility;

            if (Width <= 3 && HScrollBarVisibility == ScrollBarVisibility.AUTO)
                HScrollBarVisibility = ScrollBarVisibility.DISABLED;
            if (Height <= 3 && VScrollBarVisibility == ScrollBarVisibility.AUTO)
                VScrollBarVisibility = ScrollBarVisibility.DISABLED;

            if (HScrollBarVisibility != ScrollBarVisibility.DISABLED)
            {
                HScrollBar = ScrollBar.Factory(ScrollBar.ScrollBarOrientation.HORIZONTAL, ViewportWidth, this);
                LinkChild(HScrollBar);
            }
            if (VScrollBarVisibility != ScrollBarVisibility.DISABLED)
            {
                VScrollBar = ScrollBar.Factory(ScrollBar.ScrollBarOrientation.VERTICAL, ViewportHeight, this);
                LinkChild(VScrollBar);
            }

            CalculateScrollInfo();
        }

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

        protected override void OnChildResize()
        {
            CalculateScrollInfo();

            MinScroll();

            Render();
        }

        /// <summary>
        /// Gives preference to top right corner if region is too large.
        /// </summary>
        private void Content_EnsureCursorVisible(object sender, CursorRegionEventArgs e)
        {
            if (e.Width > 1 && e.Height > 1)
                ScrollToPoint(e.XStart + e.Width - 1, e.YStart + e.Height - 1);

            ScrollToPoint(e.XStart, e.YStart);
        }

        private bool IsHScrollBarVisible() =>
            (HScrollBar != null) &&
            ((HScrollBarVisibility == ScrollBarVisibility.VISIBLE) || 
            (HScrollBarVisibility == ScrollBarVisibility.AUTO && Content.Width > HScrollBar.Width));
        private bool IsVScrollBarVisible() =>
            (VScrollBar != null) &&
            (VScrollBarVisibility == ScrollBarVisibility.VISIBLE ||
            (VScrollBarVisibility == ScrollBarVisibility.AUTO && Content.Height > VScrollBar.Height));

        #region SCROLL

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
            if (IsHScrollBarVisible())
                HScrollBar.LoadScrollInfo(new ScrollInfo(HorizontalStart, Content.Width, ViewportWidth));
        }
        private void RefreshVScrollInfo()
        {
            if (IsVScrollBarVisible())
                VScrollBar.LoadScrollInfo(new ScrollInfo(VerticalStart, Content.Height, ViewportHeight));
        }

        public void SetHorizontalStart(int x)
        {
            HorizontalStart = x;

            if (Content != null && HorizontalStart + ViewportWidth > Content.Width)
            {
                HorizontalStart = Content.Width - ViewportWidth;
            }
            if (HorizontalStart < 0)
            {
                HorizontalStart = 0;
            }
        }
        public void SetVerticalStart(int y)
        {
            VerticalStart = y;

            if (Content != null && VerticalStart + ViewportHeight > Content.Height)
            {
                VerticalStart = Content.Height - ViewportHeight;
            }
            if (VerticalStart < 0)
            {
                VerticalStart = 0;
            }
        }
        public bool ScrollX(int amount)
        {
            if (amount < 0 && AtBeginningX() || amount > 0 && AtEndX())
                return false;

            SetHorizontalStart(HorizontalStart + amount);

            RefreshHScrollInfo();

            Render();

            return true;
        }
        public bool ScrollY(int amount)
        {
            if (amount < 0 && AtBeginningY() || amount > 0 && AtEndY())
                return false;

            SetVerticalStart(VerticalStart + amount);

            RefreshVScrollInfo();

            Render();

            return true;
        }

        public bool MinScroll()
        {
            if (AtBeginningX() && AtBeginningY())
                return false;

            HorizontalStart = 0;
            VerticalStart = 0;

            RefreshHScrollInfo();
            RefreshVScrollInfo();

            Render();

            return true;
        }
        public bool MaxScroll()
        {
            if (AtEndX() && AtEndY())
                return false;

            HorizontalStart = Content.Width - ViewportWidth;
            if (HorizontalStart < 0)
            {
                HorizontalStart = 0;
            }
            VerticalStart = Content.Height - ViewportHeight;
            if (VerticalStart < 0)
            {
                VerticalStart = 0;
            }

            RefreshHScrollInfo();
            RefreshVScrollInfo();

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

            if (x < HorizontalStart)
            {
                SetHorizontalStart(x);
            }
            else if (x >= HorizontalStart + ViewportWidth)
            {
                SetHorizontalStart(x - ViewportWidth + 1);
            }

            if (y < VerticalStart)
            {
                SetVerticalStart(y);
            }
            else if (y >= VerticalStart + ViewportHeight)
            {
                SetVerticalStart(y - ViewportHeight + 1);
            }

            RefreshHScrollInfo();
            RefreshVScrollInfo();

            Render();

            return true;
        }

        private bool IsPointInView(int x, int y) =>
            (x >= HorizontalStart && x < HorizontalStart + ViewportWidth) && 
            (y >= VerticalStart && y < VerticalStart + ViewportHeight);

        private bool AtBeginningX() => HorizontalStart == 0;
        private bool AtEndX() => HorizontalStart + ViewportWidth == Content.Width;
        private bool AtBeginningY() => VerticalStart == 0;
        private bool AtEndY() => VerticalStart + ViewportHeight == Content.Height;

        #endregion

        protected override void Draw()
        {
            base.Draw();

            if (Content != null)
                View.Merge(Content.View, 0, 0, HorizontalStart, VerticalStart, ViewportWidth, ViewportHeight);

            if (IsHScrollBarVisible())
                View.Merge(HScrollBar.View, 0, Height - 1);
            if (IsVScrollBarVisible())
                View.Merge(VScrollBar.View, Width - 1, 0);
        }

        #region INPUT

        public override Widget GetHoveredWidget(MouseContext m)
        {
            (int x, int y) = m.Mouse;

            if (x == ViewportWidth && y == ViewportHeight)
            {
                return this;
            }
            else if (IsVScrollBarVisible() && x == ViewportWidth)
            {
                return VScrollBar;
            }
            else if (IsHScrollBarVisible() && y == ViewportHeight)
            {
                return HScrollBar;
            }
            else
            {
                m.SetRelativeMouse(HorizontalStart, VerticalStart);
                return Content;
            }
        }

        protected override void Input(Input.InputEventArgs e)
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

        private void OnScroll(Input.InputEventArgs e)
        {
            int delta = e.Mouse.ScrollDelta;

            if (e.Keyboard.Shift || !IsVScrollBarVisible())
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

        private void OnKeyDown(Input.InputEventArgs e)
        {
            switch (e.Keyboard.LastKeyPressed)
            {
                case Key.PageUp:
                    e.Handled = PageUp();
                    return;
                case Key.PageDown:
                    e.Handled = PageDown();
                    return;
                case Key.Home:
                    e.Handled = MinScroll();
                    return;
                case Key.End:
                    e.Handled = MaxScroll();
                    return;
                case Key.Left:
                    e.Handled = ScrollX(-1);
                    return;
                case Key.Right:
                    e.Handled = ScrollX(1);
                    return;
                case Key.Up:
                    e.Handled = ScrollY(-1);
                    return;
                case Key.Down:
                    e.Handled = ScrollY(1);
                    return;
                default:
                    return;
            }
        }

        #endregion

        /// <summary>
        /// A ScrollBar has no knowledge of the actual size of the content; it merely knows 
        /// its own size and stuff for rendering.  These things must be updated any time the
        /// ScrollViewer's view changes.
        /// </summary>
        private class ScrollBar : Widget
        {
            public enum ScrollBarOrientation { HORIZONTAL, VERTICAL }
            private ScrollBarOrientation Orientation { get; set; }
            private int Length { get; set; }

            private enum HoverRegion { LESS_BUTTON, LESS_TRACK, GRIP, MORE_TRACK, MORE_BUTTON, NONE }
            private HoverRegion Region { get; set; }

            private int TrackLength { get; set; }

            private int GripStart { get; set; }
            private int GripLength { get; set; }

            // Yes, I know this is a circular reference but it's okay because the existence of a scrollbar
            // is very controlled.  They will exist only in the context of a scrollviewer.
            private ScrollViewer Viewer { get; set; }

            /// <summary>
            /// Only called by factory!
            /// </summary>
            private ScrollBar(int width, int height) : base(width, height)
            {
                Region = HoverRegion.NONE;
            }

            public static ScrollBar Factory(ScrollBarOrientation orientation, int length, ScrollViewer parent)
            {
                if (length < 3)
                    throw new ArgumentOutOfRangeException("A ScrollBar must be at least 3 cells long!");

                ScrollBar s;

                if (orientation == ScrollBarOrientation.HORIZONTAL)
                    s = new ScrollBar(length, 1);
                else if (orientation == ScrollBarOrientation.VERTICAL)
                    s = new ScrollBar(1, length);
                else
                    throw new InvalidEnumArgumentException();

                s.Orientation = orientation;
                s.Length = length;
                s.Viewer = parent;

                return s;
            }

            protected override void AdjustResize()
            {
                Length = Math.Max(Width, Height);
            }

            public void LoadScrollInfo(ScrollInfo s)
            {
                TrackLength = Length - 2;

                float windowContentRatio = (float)Length / s.ContentLength;

                GripLength = (int)(TrackLength * windowContentRatio);

                if (GripLength < 1)
                    GripLength = 1;
                if (GripLength > TrackLength)
                    GripLength = TrackLength;

                int scrollAreaSize = s.ContentLength - s.ViewerLength;
                float windowPositionRatio = (float)s.Start / scrollAreaSize;
                int trackScrollAreaSize = TrackLength - GripLength;
                int gripPositionOnTrack = (int)(trackScrollAreaSize * windowPositionRatio);

                GripStart = Math.Max(gripPositionOnTrack + 1, 1);

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
            private const string TrackScheme = "Default";
            private const string GripScheme = "Default";

            private static readonly ColorFamily Base = new ColorFamily() { "ScrollBar_Default", "Default" };
            private static readonly ColorFamily Hover = new ColorFamily() { "ScrollBar_Hover", "Hover" };
            #endregion DISPLAY CONSTANTS

            protected override void Draw()
            {
                base.Draw();

                View.FillColorScheme(Style.GetColorScheme(Base));

                DrawTrack();
                DrawGrip();
                Colorize();
            }

            private void DrawTrack()
            {
                View.FillColorScheme(Style.GetColorPair(TrackScheme));

                if (Orientation == ScrollBarOrientation.HORIZONTAL)
                {
                    View.FillChar(HTrackChar);

                    View.SetChar(0, 0, HLessChar);
                    View.SetChar(Width - 1, Height - 1, HMoreChar);
                }
                else if (Orientation == ScrollBarOrientation.VERTICAL)
                {
                    View.FillChar(VTrackChar);

                    View.SetChar(0, 0, VLessChar);
                    View.SetChar(Width - 1, Height - 1, VMoreChar);
                }
            }
            private void DrawGrip()
            {
                if (Orientation == ScrollBarOrientation.HORIZONTAL)
                {
                    View.FillColorScheme(Style.GetColorPair(GripScheme), GripStart, 0, GripLength, 1);
                    View.FillChar(HGripChar, GripStart, 0, GripLength, 1);
                }
                else if (Orientation == ScrollBarOrientation.VERTICAL)
                {
                    View.FillColorScheme(Style.GetColorPair(GripScheme), 0, GripStart, 1, GripLength);
                    View.FillChar(VGripChar, 0, GripStart, 1, GripLength);
                }
            }
            private void Colorize()
            {
                switch (Region)
                {
                    case HoverRegion.LESS_BUTTON:
                        View.SetColorScheme(0, 0, Style.GetColorScheme(Hover));
                        break;
                    case HoverRegion.LESS_TRACK:
                        break;
                    case HoverRegion.GRIP:
                        if (Orientation == ScrollBarOrientation.HORIZONTAL)
                            View.FillColorScheme(Style.GetColorScheme(Hover), GripStart, 0, GripLength, 1);
                        else if (Orientation == ScrollBarOrientation.VERTICAL)
                            View.FillColorScheme(Style.GetColorScheme(Hover), 0, GripStart, 1, GripLength);
                        break;
                    case HoverRegion.MORE_TRACK:
                        break;
                    case HoverRegion.MORE_BUTTON:
                        View.SetColorScheme(Width - 1, Height - 1, Style.GetColorScheme(Hover));
                        break;
                    case HoverRegion.NONE:
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }

            protected override void Input(Input.InputEventArgs e)
            {
                base.Input(e);

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

            private void OnMouseMove(Input.InputEventArgs e)
            {
                int index = Orientation switch
                {
                    ScrollBarOrientation.HORIZONTAL => e.Mouse.Mouse.X,
                    ScrollBarOrientation.VERTICAL => e.Mouse.Mouse.Y,
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

                Render();
            }

            private void OnMouseLeave()
            {
                Region = HoverRegion.NONE;

                Render();
            }

            private void OnLeftClick(Input.InputEventArgs e)
            {
                if (Orientation == ScrollBarOrientation.HORIZONTAL)
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
                else if (Orientation == ScrollBarOrientation.VERTICAL)
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

            private void OnScroll(Input.InputEventArgs e)
            {
                int delta = e.Mouse.ScrollDelta;

                if (Orientation == ScrollBarOrientation.HORIZONTAL)
                {
                    if (delta < 0)
                        Viewer.ScrollX(-1);
                    else
                        Viewer.ScrollX(1);
                }
                else if (Orientation == ScrollBarOrientation.VERTICAL)
                {
                    if (delta < 0)
                        Viewer.ScrollY(1);
                    else
                        Viewer.ScrollY(-1);
                }

                e.Handled = true;
            }
        }

        private struct ScrollInfo
        {
            public int Start { get; }
            public int ContentLength { get; }
            public int ViewerLength { get; }

            public ScrollInfo(int start, int contentLength, int viewerLength)
            {
                Start = start;
                ContentLength = contentLength;
                ViewerLength = viewerLength;
            }
        }
    }
}
