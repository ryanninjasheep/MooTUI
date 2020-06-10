using MooTUI.Core;
using MooTUI.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MooTUI.Widgets.Primitives
{
    /// <summary>
    /// Includes a number of abstract methods that interact with children, in addition to providing a
    /// way to link and unlink children.
    /// </summary>
    public abstract class Container : Widget
    {
        public Container(int width, int height) : base(width, height) { }

        /// <summary>
        /// Attempts to link the specified widget to this Container.  Only works 
        /// if logical child is already contained and doesn't have another parent.
        /// </summary>
        protected void LinkChild(Widget child)
        {
            AssertContains(child);

            child.LinkParent(this);
            child.SetStyle(Style, false);
        }
        /// <summary>
        /// Unlinks the specified child, allowing it to be linked to another Container.
        /// </summary>
        protected void UnlinkChild(Widget child)
        {
            AssertContains(child);

            child.UnlinkParent();
        }

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

        /// <summary>
        /// Throws an exception if the specified Widget is not a child of this Container.
        /// </summary>
        protected void AssertContains(Widget w)
        {
            if (!Contains(w))
                throw new InvalidOperationException("Parent does not contain child.");
        }

        /// <summary>
        /// Ensures the given widget is a logical child of this Container, then calls OnChildResize().
        /// </summary>
        /// <remarks>
        /// Should only be called by a child!
        /// </remarks>
        internal void OnChildResize(Widget child)
        {
            AssertContains(child);

            OnChildResize();
        }

        /// <summary>
        /// Ensures the given widget is a logical child of this Container, then renders this Widget.
        /// </summary>
        /// <remarks>
        /// Should only be called by a child!
        /// </remarks>
        internal void OnChildRender(Widget child)
        {
            AssertContains(child);

            Render();
        }
        
        /// <summary>
        /// Ensures the given widget is a logical child of this Container, then attempts to handle
        /// the given input.
        /// </summary>
        /// <remarks>
        /// Should only be called by a child!
        /// </remarks>
        internal void OnChildInput(Widget child, InputEventArgs e)
        {
            AssertContains(child);

            HandleInput(e);
        }

        #region ABSTRACT METHODS

        /// <summary>
        /// Updates relative mouse position and returns Widget directly under mouse.
        /// </summary>
        public abstract Widget GetHoveredWidget(MouseContext m);

        /// <summary>
        /// Returns an unordered list of all logical children.
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
