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
        /// being called when manipulating child size.
        /// </summary>
        protected abstract void OnChildResized(Widget child);

        protected internal abstract (int xOffset, int yOffset) GetChildOffset(Widget child);

        protected void LinkChild(Widget child)
        {
            child.Bind();

            child.EnsureVisible += Child_EnsureVisible;

            child.BubbleEvent += Child_BubbleEvent;
        }

        protected void UnlinkChild(Widget child)
        {
            if (!GetLogicalChildren().Contains(child))
                throw new ArgumentException("The given Widget is not a child of this container.");

            child.Release();

            child.EnsureVisible -= Child_EnsureVisible;

            child.BubbleEvent -= Child_BubbleEvent;
        }

        private void Child_BubbleEvent(object sender, BubblingEventArgs e)
        {
            switch (e)
            {
                case RenderEventArgs r:
                    Child_Rendered(r);
                    break;
                case InputEventArgs i:
                    Child_BubbleInput(i);
                    break;
                case ResizeEventArgs r:
                    Child_Resized(r);
                    break;
            }

            if (e is ConditionalBubblingEventArgs c && !c.Continue)
                return;

            e.Previous = this;
            OnBubbleEvent(e);
        }

        private void Child_Rendered(RenderEventArgs r)
        {
            if (Lock)
                return;

            DrawChild(r.Previous);
            Render(r);
        }
        private void Child_BubbleInput(InputEventArgs i)
        {
            if (i is MouseInputEventArgs m && i.Previous != null)
            {
                (int, int) offset = GetChildOffset(i.Previous);
                m.SetRelativeMouse(offset);
            }
            HandleInput(i);
        }
        private void Child_Resized(ResizeEventArgs r) 
        {
            if (Lock)
                return; 
            
            OnChildResized(r.Sender);
        }
        private void Child_EnsureVisible(object sender, RegionEventArgs e)
        {
            (int xOffset, int yOffset) = GetChildOffset(sender as Widget);
            EnsureRegionVisible(e.X + xOffset, e.Y + yOffset, e.Width, e.Height);
        }
    }
}
