using MooTUI.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets.Primitives
{
    /// <summary>
    /// Contains default implementation for a container with exactly one child.
    /// </summary>
    public abstract class MonoContainer : Container
    {
        public Widget Content { get; private set; }

        private bool AllowsNewChild { get; set; }

        public MonoContainer(int width, int height) : base(width, height) { }

        protected void SetContent(Widget content)
        {
            if (Content != null)
            {
                if (AllowsNewChild)
                    UnlinkChild(Content);
                else
                    throw new InvalidOperationException("This Container already has a child!");
            }

            Content = content;
            LinkChild(content);
            Content.Render();
        }

        protected override IEnumerable<Widget> GetLogicalChildren()
        {
            if (Content == null)
                return new List<Widget>();
            else
                return new List<Widget>() { Content };
        }
    }
}
