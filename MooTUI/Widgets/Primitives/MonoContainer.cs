using MooTUI.Layout;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets.Primitives
{
    public abstract class MonoContainer : Container
    {
        public Widget Content { get; private set; }

        public MonoContainer(LayoutRect bounds) : base(bounds) { }

        protected void SetContent(Widget content)
        {
            if (Content != null)
                throw new InvalidOperationException("This Container can only have one child.");

            Content = content;
            LinkChild(content);
            Content.Render();
        }

        protected override IEnumerable<Widget> GetLogicalChildren()
        {
            if (Content != null)
                return new List<Widget>() { Content };
            else
                return new List<Widget>();
        }
    }
}
