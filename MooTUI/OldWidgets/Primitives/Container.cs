using MooTUI.Input;
using MooTUI.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MooTUI.OldWidgets.Primitives
{
    /// <summary>
    /// Includes a number of abstract methods that interact with children, in addition to providing a
    /// way to link and unlink children.
    /// </summary>
    public abstract class Container : Widget
    {
        public Container(int width, int height) : base(width, height) { }

        /// <summary>
        /// Sets this container as an observer for the specified child.
        /// </summary>
        protected void LinkChild(Widget child)
        {
            child.ClaimAsChild();
            child.SetStyle(Style, false);

            child.Resized += Child_Resized;
            child.MessageReceived += Child_MessageReceived;
            child.BubbleInput += Child_BubbleInput;
            child.ClaimFocusEventHandler += Child_ClaimFocus;
            child.Rendered += Child_Rendered;
        }

        /// <summary>
        /// Unlinks the specified child.
        /// </summary>
        protected void UnlinkChild(Widget child)
        {
            child.ReleaseAsChild();

            child.Resized -= Child_Resized;
            child.MessageReceived -= Child_MessageReceived;
            child.BubbleInput -= Child_BubbleInput;
            child.ClaimFocusEventHandler -= Child_ClaimFocus;
            child.Rendered -= Child_Rendered;
        }

        private void Child_Resized(object sender, EventArgs e) => OnChildResize();

        private void Child_MessageReceived(object sender, MessageEventArgs e) => BubbleMessage(e);

        private void Child_ClaimFocus(object sender, FocusEventArgs e) => OnClaimFocus(e);

        private void Child_BubbleInput(object sender, InputEventArgs e) => HandleInput(e);

        private void Child_Rendered(object sender, EventArgs e) => Render();

        /// <summary>
        /// Attempts to populate Style change to children.
        /// </summary>
        public override void SetStyle(Style style, bool overrideDefault)
        {
            base.SetStyle(style, overrideDefault);

            foreach (Widget w in GetLogicalChildren())
            {
                w.SetStyle(style, false);
            }
        }

        /// <summary>
        /// Determines if the specified Widget is a child of this Container.
        /// </summary>
        internal bool Contains(Widget w) => GetLogicalChildren().Contains(w);

        #region ABSTRACT METHODS

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
        /// Called whenever a logical child is resized.
        /// </summary>
        /// <remarks>
        /// Do not call manually.
        /// </remarks>
        protected abstract void OnChildResize();

        #endregion
    }
}
