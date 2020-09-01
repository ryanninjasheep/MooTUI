using MooTUI.Layout;
using MooTUI.Input;
using MooTUI.Drawing;
using MooTUI.Control;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets.Primitives
{
    public abstract partial class Widget
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

        public Widget(LayoutRect bounds)
        {
            Bounds = bounds ?? throw new ArgumentNullException(nameof(bounds));
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
        /// <summary>
        /// Called whenever this widget changes size.
        /// </summary>
        public event EventHandler Resized;

        /// <summary>
        /// Draw View and bubble up logical tree, making parents render too.
        /// </summary>
        public void Render()
        {
            Draw();

            OnRendered(EventArgs.Empty);
            OnBubbleEvent(new RenderEventArgs(this));
        }

        /// <summary>
        /// Attempt to handle the given InputEventArgs, bubbling input up the logical tree.
        /// </summary>
        public void HandleInput(InputEventArgs e)
        {
            Input(e);

            OnInputReceived(e);
            OnBubbleEvent(e);
        }

        public void ClaimFocus() => OnBubbleEvent(new ClaimFocusEventArgs(this));

        public bool HitTest(int x, int y) =>
            (x >= 0 && x < Width) && (y >= 0 && y < Height);

        /// <summary>
        /// Used for any drawing that is done any time the visual is rendered.
        /// </summary>
        protected virtual void Draw() { }
        /// <summary>
        /// Used for base drawing.  Called whenever the entire visual needs to be redrawn.
        /// Unnecessary to call after object init.
        /// </summary>
        protected abstract void RefreshVisual();
        /// <summary>
        /// Deal with input.
        /// </summary>
        protected abstract void Input(InputEventArgs e);
        /// <summary>
        /// If any additional changes need to be done when this widget is resized, put them here.
        /// </summary>
        protected virtual void Resize() { }

        protected virtual void EnsureRegionVisible(int x, int y, int width = 1, int height = 1) =>
            OnBubbleEvent(new RegionEventArgs(this, x, y, width, height));

        private void Bounds_SizeChanged(object sender, EventArgs e) => OnSizeChanged();

        private void OnSizeChanged()
        {
            Visual = new Visual(Width, Height);

            Resize();

            OnResized(EventArgs.Empty);

            RefreshVisual();
            Render();
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
        private void OnResized(EventArgs e)
        {
            EventHandler handler = Resized;
            handler?.Invoke(this, e);
        }
    }

    public abstract partial class Widget
    {
        internal bool HasParent { get; private set; }

        internal event EventHandler<BubblingEventArgs>? BubbleEvent;
        internal void OnBubbleEvent(BubblingEventArgs e)
        {
            EventHandler<BubblingEventArgs>? handler = BubbleEvent;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// !!! ONLY CALL FROM Container.LinkChild !!!
        /// </summary>
        internal void Bind()
        {
            if (HasParent)
                throw new InvalidOperationException("Cannot bind Widget because it already has a parent.");

            RefreshVisual();
            HasParent = true;
        }

        /// <summary>
        /// !!! ONLY CALL FROM Container.UnlinkChild !!!
        /// </summary>
        internal void Release() => HasParent = false;
    }
}
