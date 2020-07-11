using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using MooTUI.Input;
using MooTUI.IO;

namespace MooTUI.OldWidgets.Primitives
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

        protected bool HasParent { get; private set; }

        protected Style Style { get; private set; }
        private bool IsDefaultStyle { get; set; }

        public Widget(int width, int height)
        {
            View = new View(width, height);

            IsDefaultStyle = true;
            Style = Style.Dark;

            IsEnabled = true;
            IsVisible = true;
        }

        /// <summary>
        /// Adjusts the width and height of this Widget and then renders it.
        /// </summary>
        /// <remarks>
        /// If any internal changes need to take place before rendering, override AdjustResize().
        /// </remarks>
        public void Resize(int width, int height)
        {
            if (Width == width && Height == height)
                return;

            View = new View(width, height);

            AdjustResize();

            OnResized(EventArgs.Empty);

            Render();
        }

        /// <remarks>
        /// Override if any additional behavior is needed when resizing.
        /// </remarks>
        protected virtual void AdjustResize() { }

        internal void ClaimAsChild()
        {
            if (HasParent)
                throw new InvalidOperationException("This Widget already has a parent!");

            HasParent = true;
        }
        internal void ReleaseAsChild() => HasParent = false;

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

            OnRendered(EventArgs.Empty);

            return View;
        }

        /// <summary>
        /// Sets this Widget's style unless it already isn't default.
        /// </summary>
        /// <remarks>
        /// Theoretically, this will only be overridden by Container.
        /// </remarks>
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
        /// Generate a message and begin bubbling it up the logical tree.
        /// </summary>
        public void GenerateMessage(Message.MessageType type, string message) =>
            BubbleMessage(new MessageEventArgs(new Message(type, message, this)));

        /// <remarks>
        /// Only marked internal so that Container has access.
        /// </remarks>
        internal void BubbleMessage(MessageEventArgs e)
        {
            if (this is IPushMessage pushMessage)
                pushMessage.PushMessage(e.Message);
            else
                OnMessageReceived(e);
        }

        #region INPUT

        /// <summary>
        /// Attempts to handle the given InputEventArgs.  Bubbles up to parent is this Widget has one.
        /// </summary>
        public void HandleInput(InputEventArgs e)
        {
            if (!IsEnabled)
                return;

            Input(e);

            OnInputReceived(e);

            OnBubbleInput(e);
        }

        /// <summary>
        /// Override if this Widget responds to any input.
        /// </summary>
        protected virtual void Input(InputEventArgs e) { }

        protected void ClaimFocus() => OnClaimFocus(new FocusEventArgs(this));

        #endregion

        /// <summary>
        /// Returns true if a point is within bounds.
        /// </summary>
        public bool HitTest(int x, int y) => 
            (x >= 0 && x < Width) && (y >= 0 && y < Height);

        #region EVENTS

        // The following events exist only in conjunction with 
        // Containers and MooInterfaces.  Do not observe them.

        internal event EventHandler Resized;
        internal event EventHandler<MessageEventArgs> MessageReceived;
        internal event EventHandler<InputEventArgs> BubbleInput;
        internal event EventHandler<FocusEventArgs> ClaimFocusEventHandler;

        // The following events are public, and can be observed 
        // if they are relevant

        /// <summary>
        /// Raised whenever this Widget is rendered.
        /// </summary>
        public event EventHandler Rendered;

        /// <summary>
        /// Raised after this Widget handles input, but before it bubbles the input
        /// to its parent.
        /// </summary>
        public event EventHandler<InputEventArgs> InputReceived;

        // The following methods are ways to raise events.  In some
        // cases, they are private; in other cases, they are protected.
        // This is based on whether they should be accessed from
        // derived classes.

        private void OnResized(EventArgs e)
        {
            EventHandler hander = Resized;
            hander?.Invoke(this, e);
        }
        private void OnMessageReceived(MessageEventArgs e)
        {
            EventHandler<MessageEventArgs> handler = MessageReceived;
            handler?.Invoke(this, e);
        }
        private void OnBubbleInput(InputEventArgs e)
        {
            EventHandler<InputEventArgs> handler = BubbleInput;
            handler?.Invoke(this, e);
        }
        internal void OnClaimFocus(FocusEventArgs e)
        {
            EventHandler<FocusEventArgs> handler = ClaimFocusEventHandler;
            handler?.Invoke(this, e);
        }

        private void OnRendered(EventArgs e)
        {
            EventHandler handler = Rendered;
            handler?.Invoke(this, e);
        }
        private void OnInputReceived(InputEventArgs e)
        {
            EventHandler<InputEventArgs> handler = InputReceived;
            handler?.Invoke(this, e);
        }

        #endregion EVENTS
    }
}
