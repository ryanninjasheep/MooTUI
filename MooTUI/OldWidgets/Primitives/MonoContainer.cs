﻿using MooTUI.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.OldWidgets.Primitives
{
    /// <summary>
    /// Contains default implementation for a container with exactly one child.
    /// </summary>
    public abstract class MonoContainer : Container
    {
        public Widget Content { get; private set; }

        public MonoContainer(int width, int height) : base(width, height) { }

        protected void SetContent(Widget content)
        {
            if (Content != null)
                UnlinkChild(Content);

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
