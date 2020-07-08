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

        public Widget(LayoutRect bounds)
        {
            Bounds = bounds;
            View = new View(Bounds.Width, Bounds.Height);
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
            Draw();

            OnRendered(EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        public void HandleInput(InputEventArgs e)
        {
            Input(e);

            OnInputReceived(e);

            OnBubbleInput(e);
        }

        protected abstract void Draw();
        protected abstract void Input(InputEventArgs e);

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

        private protected event EventHandler<InputEventArgs> BubbleInput;
    }
}
