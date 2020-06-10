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
        // that children cannot interact with parents except under very specific circumstances.
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

        /// <summary>
        /// THIS SHOULD ONLY BE CALLED BY PARENT CONTAINER!!!
        /// </summary>
        internal void LinkParent(Container parent)
        {
            if (Parent != null)
                throw new InvalidOperationException("Child already has a parent.");

            Parent = parent;
        }
        /// <summary>
        /// THIS SHOULD ONLY BE CALLED BY PARENT CONTAINER!!!
        /// </summary>
        internal void UnlinkParent() => Parent = null;

        /// <summary>
        /// Adjusts the width and height of this Widget and then renders it.
        /// </summary>
        /// <remarks>
        /// If any internal changes need to take place before rendering, override OnResize().
        /// </remarks>
        public void Resize(int width, int height)
        {
            if (Width == width && Height == height)
                return;

            View = new View(width, height);

            Parent?.OnChildResize(this);

            Resized();

            Render();
        }

        /// <remarks>
        /// Override if any additional behavior is needed when resizing.
        /// </remarks>
        protected virtual void Resized() { }

        #region RENDERING and STYLE

        /// <summary>
        /// Contains all logic for manipulating the View.  In general, assume that it should be completely
        /// recreated every time this Widget is rendered.  By default, clears all chars.
        /// </summary>
        protected virtual void Draw() 
        {
            View.FillChar(' ');
        }

        /// <summary>
        /// Redraws View and bubbles, making parents render, too.
        /// </summary>
        public View Render()
        {
            if (!IsVisible)
                return new View(Width, Height);

            Draw();

            Parent?.OnChildRender(this);

            OnRendered(EventArgs.Empty);

            return View;
        }

        /// <summary>
        /// Raised whenever this Widget is rendered.
        /// </summary>
        public event EventHandler Rendered;
        protected void OnRendered(EventArgs e)
        {
            EventHandler handler = Rendered;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Sets this Widget's style unless already overridden.
        /// </summary>
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

        /// <summary>
        /// Bubbles to find the root of the logical tree.
        /// </summary>
        protected Widget GetLogicalRoot() => Parent?.GetLogicalRoot() ?? this;

        #region MESSAGING and MODALS

        /// <summary>
        /// Attempts to bubble the given message up the logical tree until it
        /// reaches a Widget that is capable of displaying it.
        /// </summary>
        public void BubbleMessage(Message m)
        {
            if (this is IPushMessage pushMessage)
                pushMessage.PushMessage(m);
            else
                Parent?.BubbleMessage(m);
        }

        //public void BubbleModal(Modal m)
        //{
        //    if (this is IPushModal pushModal)
        //        pushModal.PushModal(m);
        //    else
        //        Parent?.BubbleModal(m);
        //}

        #endregion

        #region INPUT

        /// <summary>
        /// Attempts to handle the given InputEventArgs.  Bubbles up to parent is this Widget has one.
        /// </summary>
        public void HandleInput(InputEventArgs e)
        {
            if (!IsEnabled)
                return;

            Input(e);

            OnInput(e);

            Parent?.OnChildInput(this, e);
        }

        /// <summary>
        /// Override if this Widget responds to any input.
        /// </summary>
        protected virtual void Input(InputEventArgs e) { }

        /// <summary>
        /// Raised after this Widget handles input, but before it bubbles the input
        /// to its parent.
        /// </summary>
        public event EventHandler<InputEventArgs> InputEventHandler;
        protected void OnInput(InputEventArgs e)
        {
            EventHandler<InputEventArgs> handler = InputEventHandler;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// JANK!  Only exists for MooInterface.  Maybe come up with something better later?????
        /// </summary>
        internal event EventHandler<FocusEventArgs> ClaimFocus;
        protected void OnClaimFocus(FocusEventArgs e)
        {
            // THIS IS JANKY :(((

            if (Parent != null)
            {
                Parent?.OnClaimFocus(e);
            }
            else
            {
                EventHandler<FocusEventArgs> handler = ClaimFocus;
                handler?.Invoke(this, e);
            }
        }

        #endregion

        /// <summary>
        /// Returns true if a point is within bounds.
        /// </summary>
        public bool HitTest(int x, int y) => 
            (x >= 0 && x < Width) && (y >= 0 && y < Height);
    }
}
