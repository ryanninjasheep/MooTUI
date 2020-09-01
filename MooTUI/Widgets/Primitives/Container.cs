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

            child.BubbleEvent += Child_BubbleEvent;
        }

        protected void UnlinkChild(Widget child)
        {
            if (!GetLogicalChildren().Contains(child))
                throw new ArgumentException("The given Widget is not a child of this container.");

            child.Release();

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
                case ResizedEventArgs r:
                    Child_Resized(r);
                    break;
                case RegionEventArgs r:
                    Child_EnsureVisible(r);
                    break;
            }

            e.Previous = this;
            OnBubbleEvent(e);
        }

        private void Child_Rendered(RenderEventArgs r)
        {
            if (Lock)
                return;

            DrawChild(r.Previous!);
            Render();
        }
        private void Child_BubbleInput(InputEventArgs e)
        {
            if (e is MouseInputEventArgs m && e.Previous is Widget child)
            {
                (int, int) offset = GetChildOffset(child);
                m.SetRelativeMouse(offset);
            }
            HandleInput(e);
        }
        private void Child_Resized(ResizedEventArgs r) 
        {
            if (Lock)
                return; 
            
            OnChildResized(r.Previous!);
        }
        private void Child_EnsureVisible(RegionEventArgs r)
        {
            (int xOffset, int yOffset) = GetChildOffset(r.Origin!);
            EnsureRegionVisible(r.X + xOffset, r.Y + yOffset, r.Width, r.Height);
        }
    }
}
