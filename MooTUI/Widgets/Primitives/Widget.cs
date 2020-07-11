﻿using MooTUI.IO;
using MooTUI.Layout;
using MooTUI.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets.Primitives
{
    public abstract class Widget
    {
        protected static Style Style { get; set; } = Style.Dark;
        public static void SetStyle(Style s) => Style = s;

        public View View { get; protected set; }
        public LayoutRect Bounds { get; private set; }

        public int Width => Bounds.Width;
        public int Height => Bounds.Height;

        public bool HasParent { get; private set; }

        private bool RenderLock { get; set; }

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
        /// Called after this Widget handles input, but before it is propagated up the  logical tree.
        /// </summary>
        public event EventHandler<InputEventArgs> InputReceived;

        /// <summary>
        /// Draw View and bubble up logical tree, making parents render too.
        /// </summary>
        public void Render()
        {
            if (RenderLock)
                return;

            Draw();

            OnRendered(EventArgs.Empty);
        }

        public void Resize(LayoutRect bounds)
        {
            Bounds = bounds;

            View = new View(Width, Height);

            Resize();

            RefreshVisual();
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

        protected void LockRendering() => RenderLock = true;
        protected void UnlockRendering() => RenderLock = false;

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

        private void OnBubbleInput(InputEventArgs e)
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
