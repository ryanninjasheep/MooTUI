using MooTUI.Input;
using MooTUI.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MooTUI.Widgets.Primitives
{
    public abstract class Container : Widget
    {
        /// <summary>
        /// When manipulating child size, use this to ensure we don't reach an infinite loop.
        /// </summary>
        protected bool Lock { get; set; }

        public Container(LayoutRect bounds) : base(bounds) { }

        /// <summary>
        /// Updates relative mouse position and returns Widget directly under mouse.
        /// </summary>
        public abstract Widget GetHoveredWidget(MouseContext m);
        /// <summary>
        /// Returns an unordered list of all logical children.  When implementing, ensure
        /// that the method doesn't return null and also that none of the elements in
        /// the list are null.
        /// </summary>
        protected abstract IEnumerable<Widget> GetLogicalChildren();
        /// <summary>
        /// Draws a particular child of this object.  Do not call render from this method.
        /// </summary>
        protected abstract void DrawChild(Widget child);
        /// <summary>
        /// Contains additional behavior to occur if a child is resized.  Use lock to prevent this from
        /// being called when maniputlating child size.
        /// </summary>
        protected abstract void OnChildResized(Widget child);

        protected void LinkChild(Widget child)
        {
            child.Bind();

            child.Rendered += Child_Rendered;
            child.BubbleInput += Child_BubbleInput;
            child.BubbleFocus += Child_BubbleFocus;
            child.Resized += Child_Resized;
            child.LayoutUpdated += Child_LayoutUpdated;
        }

        protected void UnlinkChild(Widget child)
        {
            if (!GetLogicalChildren().Contains(child))
                throw new ArgumentException("The given Widget is not a child of this container.");

            child.Release();

            child.Rendered -= Child_Rendered;
            child.BubbleInput -= Child_BubbleInput;
            child.BubbleFocus -= Child_BubbleFocus;
            child.Resized -= Child_Resized;
            child.LayoutUpdated -= Child_LayoutUpdated;
        }

        private void Child_Rendered(object sender, EventArgs e)
        {
            if (Lock)
                return;

            DrawChild(sender as Widget);
            Render();
        }
        private void Child_BubbleInput(object sender, InputEventArgs e) => HandleInput(e);
        private void Child_BubbleFocus(object sender, IO.FocusEventArgs e) => OnClaimFocus(e);
        private void Child_Resized(object sender, EventArgs e) 
        {
            if (Lock)
                return; 
            
            OnChildResized(sender as Widget);
        }
        private void Child_LayoutUpdated(object sender, EventArgs e) => OnLayoutUpdated(e);
    }
}
