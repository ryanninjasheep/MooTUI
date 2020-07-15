using MooTUI.IO;
using MooTUI.Layout;
using MooTUI.Input;
using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Core;

namespace MooTUI.Widgets.Primitives
{
    public abstract class Widget
    {
        private LayoutRect _bounds;

        public static Style Style { get; set; } = Style.Dark;

        public LayoutRect Bounds
        {
            get => _bounds;
            private set
            {
                if (!(_bounds is null))
                    _bounds.SizeChanged -= Bounds_SizeChanged;

                _bounds = value;
                _bounds.SizeChanged += Bounds_SizeChanged;
            }
        }
        public Visual Visual { get; protected set; }

        public int Width => Bounds.Width;
        public int Height => Bounds.Height;

        public bool HasParent { get; private set; }

        public Widget(LayoutRect bounds)
        {
            Bounds = bounds;
            Visual = new Visual(Width, Height);
        }

        /// <summary>
        /// Called after the View is drawn; bubbles up logical tree.
        /// </summary>
        public event EventHandler Rendered;

        /// <summary>
        /// Called after this Widget handles input, but before it is propagated up the  logical tree.
        /// </summary>
        public event EventHandler<InputEventArgs> InputReceived;

        public event EventHandler Resized;

        /// <summary>
        /// Draw View and bubble up logical tree, making parents render too.
        /// </summary>
        public void Render()
        {
            Draw();

            OnRendered(EventArgs.Empty);
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

        public void ClaimFocus() => OnClaimFocus(new FocusEventArgs(this));

        public bool HitTest(int x, int y) =>
            (x >= 0 && x < Width) && (y >= 0 && y < Height);

        /// <summary>
        /// Used for any drawing that is done any time the visual is rendered.
        /// </summary>
        protected virtual void Draw() { }
        /// <summary>
        /// Used for base drawing.
        /// </summary>
        protected abstract void RefreshVisual();
        protected abstract void Input(InputEventArgs e);
        protected virtual void Resize() { }

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

        private void OnResized(EventArgs e)
        {
            EventHandler handler = Resized;
            handler?.Invoke(this, e);
        }

        private void OnSizeChanged()
        {
            Visual = new Visual(Width, Height);

            Resize();

            OnResized(EventArgs.Empty);

            RefreshVisual();
            Render();
        }

        private void Bounds_SizeChanged(object sender, EventArgs e)
        {
            OnSizeChanged();
        }

        internal void OnBubbleInput(InputEventArgs e)
        {
            EventHandler<InputEventArgs> handler = BubbleInput;
            handler?.Invoke(this, e);
        }

        internal void OnClaimFocus(FocusEventArgs e)
        {
            EventHandler<FocusEventArgs> handler = BubbleFocus;
            handler?.Invoke(this, e);
        }

        internal event EventHandler<InputEventArgs> BubbleInput;
        internal event EventHandler<FocusEventArgs> BubbleFocus;

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
