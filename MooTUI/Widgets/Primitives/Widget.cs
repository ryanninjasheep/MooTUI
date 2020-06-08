using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using MooTUI.Core;
using MooTUI.IO;

namespace MooTUI.Widgets.Primitives
{
    /// <summary>
    /// The base class for all Widgets.
    /// </summary>
    public abstract class Widget
    {
        public View View { get; private set; }
        public int Width { get => View.Width; }
        public int Height { get => View.Height; }

        public bool IsEnabled { get; protected set; }
        public bool IsVisible { get; protected set; }

        protected Style Style { get; private set; }
        private bool IsDefaultStyle { get; set; }

        // Think VERY HARD before you attempt to make this protected.  Marking it as private ensures
        // that children cannot interact with parents except in very specific circumstances.
        // ALSO: Don't set internally -- use SetParent() instead.
        private Container Parent { get; set; }

        public Widget(int width, int height)
        {
            View = new View(width, height);

            IsDefaultStyle = true;
            Style = Style.Default;

            IsEnabled = true;
            IsVisible = true;
        }

        public virtual void Resize(int width, int height)
        {
            if (Width == width && Height == height)
                return;

            View = new View(width, height);

            OnResize(EventArgs.Empty);

            Render();
        }

        public event EventHandler ResizeEventHandler;
        private void OnResize(EventArgs e)
        {
            EventHandler handler = ResizeEventHandler;
            handler?.Invoke(this, e);
        }

        #region RENDERING and STYLE

        protected virtual void Draw() 
        {
            View.FillChar(' ');
        }

        public View Render()
        {
            if (!IsVisible)
                return new View(Width, Height);

            Draw();
            OnRender(EventArgs.Empty);
            return View;
        }

        public event EventHandler RenderEventHandler;
        private void OnRender(EventArgs e)
        {
            EventHandler handler = RenderEventHandler;
            handler?.Invoke(this, e);
        }

        public virtual void SetStyle(Style style, bool overrideDefault)
        {
            if (overrideDefault)
            {
                IsDefaultStyle = false;
                Style = style;

                Render();
            }
            else if (IsDefaultStyle)
            {
                Style = style;

                Render();
            }
        }

        #endregion

        #region LOGICAL TREE MANIPULATION

        /// <summary>
        /// Finds the root of the logical tree.
        /// </summary>
        public event EventHandler<LogicalBubbleEventArgs> LogicalBubble;

        protected void BubbleLogicalRoot(LogicalBubbleEventArgs e)
        {
            e.Root = this;

            EventHandler<LogicalBubbleEventArgs> handler = LogicalBubble;
            handler?.Invoke(this, e);
        }

        protected Widget GetLogicalRoot()
        {
            LogicalBubbleEventArgs e = new LogicalBubbleEventArgs();

            BubbleLogicalRoot(e);

            return e.Root;
        }

        protected Window GetWindow()
        {
            if (GetLogicalRoot() is Window w)
                return w;

            return null;
        }

        #endregion

        #region MESSAGING and MODALS

        public void BubbleMessage(Message m)
        {
            GetWindow()?.ReceiveMessage(m);
        }

        // Not yet implemented
        //public void BubbleModal(Modal m)
        //{
        //    if (GetLogicalRoot() is Window w)
        //    {
        //        w.PushModal(m);
        //    }
        //}

        #endregion

        #region INPUT

        public void HandleInput(InputEventArgs e)
        {
            if (!IsEnabled)
                return;

            Input(e);
            OnInput(e);
        }

        protected virtual void Input(InputEventArgs e) { }

        public event EventHandler<InputEventArgs> InputEventHandler;
        private void OnInput(InputEventArgs e)
        {
            EventHandler<InputEventArgs> handler = InputEventHandler;
            handler?.Invoke(this, e);
        }

        public event EventHandler<FocusEventArgs> ClaimFocus;
        protected void OnClaimFocus(FocusEventArgs e)
        {
            EventHandler<FocusEventArgs> handler = ClaimFocus;
            handler?.Invoke(this, e);
        }

        #endregion

        /// <summary>
        /// Returns true if a point is within bounds.
        /// </summary>
        public bool HitTest(int x, int y) =>
            (x >= 0 && x < Width) && (y >= 0 && y < Height);
    }
}
