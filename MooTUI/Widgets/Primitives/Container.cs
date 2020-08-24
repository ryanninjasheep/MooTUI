using MooTUI.Input;
using MooTUI.Control;
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
        /// Returns Widget directly under mouse.
        /// </summary>
        public abstract Widget GetHoveredWidget((int x, int y) relativeMouseLocation);
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

        protected internal abstract (int xOffset, int yOffset) GetChildOffset(Widget child);

        protected void LinkChild(Widget child)
        {
            child.Bind();

            child.Rendered += Child_Rendered;
            child.BubbleInput += Child_BubbleInput;
            child.BubbleFocus += Child_BubbleFocus;
            child.Resized += Child_Resized;
            child.LayoutUpdated += Child_LayoutUpdated;
            child.EnsureVisible += Child_EnsureVisible;
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
            child.EnsureVisible -= Child_EnsureVisible;
        }

        private void Child_Rendered(object sender, EventArgs e)
        {
            if (Lock)
                return;

            DrawChild(sender as Widget);
            Render();
        }
        private void Child_BubbleInput(object sender, InputEventArgs e)
        {
            if (e is MouseInputEventArgs m)
            {
                (int, int) offset = GetChildOffset(sender as Widget);
                m.SetRelativeMouse(offset);
            }
            HandleInput(e);
        }
        private void Child_BubbleFocus(object sender, ClaimFocusEventArgs e) => OnClaimFocus(e);
        private void Child_Resized(object sender, EventArgs e) 
        {
            if (Lock)
                return; 
            
            OnChildResized(sender as Widget);
        }
        private void Child_LayoutUpdated(object sender, EventArgs e) => OnLayoutUpdated(e);
        private void Child_EnsureVisible(object sender, RegionEventArgs e)
        {
            (int xOffset, int yOffset) = GetChildOffset(sender as Widget);
            EnsureRegionVisible(e.X + xOffset, e.Y + yOffset, e.Width, e.Height);
        }
    }
}
