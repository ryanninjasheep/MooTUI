using MooTUI.IO;
using MooTUI.Layout;
using MooTUI.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets.Primitives
{
    public abstract class Widget
    {
        public View View { get; protected set; }
        public LayoutRect Bounds { get; private set; }

        public int Width => Bounds.Width;
        public int Height => Bounds.Height;

        public bool HasParent { get; private set; }

        public Widget(LayoutRect bounds)
        {
            Bounds = bounds;
            View = new View(Width, Height);
        }

        /// <summary>
        /// Called after the View is drawn; bubbles up logical tree.
        /// </summary>
        public event EventHandler Rendered;

        /// <summary>
        /// Called whenever this Widget changes size; bubbles up logical tree.
        /// </summary>
        public event EventHandler Resized;

        /// <summary>
        /// Called after this Widget handles input, but before it is propagated up the  logical tree.
        /// </summary>
        public event EventHandler<InputEventArgs> InputReceived;

        /// <summary>
        /// Draw View and bubble up logical tree, making parents render too.
        /// </summary>
        public void Render()
        {
            Draw();

            OnRendered(EventArgs.Empty);
        }

        public void SetBounds(LayoutRect bounds)
        {
            Bounds = bounds;

            View = new View(Width, Height);

            Resize();

            OnResized(EventArgs.Empty);

            Render();
        }

        /// <summary>
        /// Attempt to handle the given InputEventArgs, bubbling input up the logical tree.
        /// </summary>
        public void HandleInput(InputEventArgs e)
        {
            Input(e);

            OnInputReceived(e);

            OnBubbleInput(e);
        }

        protected abstract void Draw();
        protected abstract void Input(InputEventArgs e);
        protected virtual void Resize() { }

        private void OnRendered(EventArgs e)
        {
            EventHandler handler = Rendered;
            handler?.Invoke(this, e);
        }

        private void OnResized(EventArgs e)
        {
            EventHandler handler = Resized;
            handler?.Invoke(this, e);
        }

        private void OnInputReceived(InputEventArgs e)
        {
            EventHandler<InputEventArgs> handler = InputReceived;
            handler?.Invoke(this, e);
        }

        private void OnBubbleInput(InputEventArgs e)
        {
            EventHandler<InputEventArgs> handler = BubbleInput;
            handler?.Invoke(this, e);
        }

        internal event EventHandler<InputEventArgs> BubbleInput;

        /// <summary>
        /// !!! ONLY CALL FROM Container.LinkChild !!!
        /// </summary>
        internal void Bind()
        {
            if (HasParent)
                throw new InvalidOperationException("Cannot bind Widget because it already has a parent.");

            HasParent = true;
        }

        // Currently unused.
        // /// <summary>
        // /// !!! ONLY CALL FROM Container.UnlinkChild !!!
        // /// </summary>
        // internal void Release() => HasParent = false;
    }
}
