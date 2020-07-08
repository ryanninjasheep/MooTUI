using MooTUI.Input;
using MooTUI.Layout;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets.Primitives
{
    public abstract class Container : Widget
    {
        public Container(LayoutRect bounds) : base(bounds) { }

        protected void LinkChild(Widget child)
        {
            child.Bind();

            child.Rendered += Child_Rendered;
            child.Resized += Child_Resized;
            child.BubbleInput += Child_BubbleInput;
        }

        // NOTE: I'm not including this because right now I'm not sure if it's even necessary
        // for children to be unlinkable.  Maybe UIs are not changeable once built?
        // protected void UnlinkChild(Widget child) { }

        /// <summary>
        /// Updates relative mouse position and returns Widget directly under mouse.
        /// </summary>
        public abstract void GetHoveredWidget(MouseContext m);

        /// <summary>
        /// Returns an unordered list of all logical children.  When implementing, ensure
        /// that the method doesn't return null and also that none of the elements in
        /// the list are null.
        /// </summary>
        protected abstract IEnumerable<Widget> GetLogicalChildren();

        /// <summary>
        /// Called when a direct logical child is resized.
        /// </summary>
        protected abstract void OnChildResize(Widget child);

        private void Child_Rendered(object sender, EventArgs e) => Render();
        private void Child_Resized(object sender, EventArgs e) => OnChildResize((Widget)sender);
        private void Child_BubbleInput(object sender, InputEventArgs e) => HandleInput(e);
    }
}
