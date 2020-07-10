using MooTUI.Core;
using MooTUI.Layout;
using MooTUI.Text;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets
{
    public class TextBlock : Widget
    {
        public TextSpan Text { get; private set; }

        public TextBlock(TextSpan text, LayoutRect bounds) : base(bounds)
        {
            Text = text;

            View.DrawSpan(text);
        }

        public TextBlock FromText(TextSpan text)
        {
            Visual v = text.Draw();
            return new TextBlock(text, new LayoutRect(v.Width, v.Height));
        }

        protected override void Draw()
        {
            View.DrawSpan(Text);
        }

        protected override void Input(Input.InputEventArgs e) { }
    }
}
