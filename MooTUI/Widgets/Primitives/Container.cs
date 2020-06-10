using MooTUI.Core;
using MooTUI.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets.Primitives
{
    /// <summary>
    /// Provides functionality to recieve events from a specified child.
    /// </summary>
    public abstract class Container : Widget
    {
        public Container(int width, int height) : base(width, height) { }

        /// <summary>
        /// Set object as an observer for a particular child.  Ensures that child has same Style and InputContext.
        /// </summary>
        protected void LinkChild(Widget child)
        {
            child.RenderEventHandler += Child_RenderEventHandler;
            child.ResizeEventHandler += Child_ResizeEventHandler;
            child.InputEventHandler += Child_InputEventHandler;
            child.LogicalBubble += Child_LogicalBubble;
            child.ClaimFocus += Child_ClaimFocus;
            child.SetStyle(Style, false);
        }

        /// <summary>
        /// Stop observing a particular child.  No effect if it already wasn't being observed.
        /// </summary>
        protected void UnlinkChild(Widget child)
        {
            child.RenderEventHandler -= Child_RenderEventHandler;
            child.ResizeEventHandler -= Child_ResizeEventHandler;
            child.InputEventHandler -= Child_InputEventHandler;
            child.LogicalBubble -= Child_LogicalBubble;
            child.ClaimFocus -= Child_ClaimFocus;
        }

        #region EVENT HANDLERS

        private void Child_RenderEventHandler(object sender, EventArgs e)
        {
            Render();
        }
        private void Child_ResizeEventHandler(object sender, EventArgs e)
        {
            OnChildResize();
        }
        private void Child_InputEventHandler(object sender, InputEventArgs e)
        {
            Input(e);
        }
        private void Child_LogicalBubble(object sender, LogicalBubbleEventArgs e)
        {
            BubbleLogicalRoot(e);
        }
        private void Child_ClaimFocus(object sender, FocusEventArgs e)
        {
            OnClaimFocus(e);
        }

        #endregion

        public override void SetStyle(Style style, bool overrideDefault)
        {
            base.SetStyle(style, overrideDefault);

            SetChildStyle(style, overrideDefault);
        }

        #region ABSTRACT METHODS

        /// <summary>
        /// Updates relative mouse position and returns Widget directly under mouse.
        /// </summary>
        public abstract Widget GetHoveredWidget(MouseContext m);
        protected abstract void SetChildStyle(Style style, bool overrideDefault);

        #endregion

        #region PROTECTED METHODS

        /// <summary>
        /// Called when a child changes size.
        /// </summary>
        protected abstract void OnChildResize();

        #endregion
    }
}
