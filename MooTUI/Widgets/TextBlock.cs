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
        public TextArea Text { get; private set; }

        public TextBlock(LayoutRect bounds, TextArea text) : base(bounds)
        {
            Text = text;
        }
        public TextBlock(LayoutRect bounds, string text)
            : this(bounds, TextArea.FromString(text, bounds.Width)) { }

        public static TextBlock FromSpan(TextArea text)
        {
            Visual v = text.Draw();
            return new TextBlock(new LayoutRect(v.Width, v.Height), text);
        }

        public void SetText(TextArea text)
        {
            Text = text;
            RefreshVisual();
            Render();
        }

        protected override void RefreshVisual()
        {
            Visual.FillCell(new Cell(' ', Style.GetColorPair("Default")));
            Visual.DrawTextArea(Text);
        }

        protected override void Input(Input.InputEventArgs e) { }
    }
}
