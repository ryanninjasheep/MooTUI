using MooTUI.Core;
using MooTUI.Input;
using MooTUI.Layout;
using MooTUI.Text;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets
{
    public class Button : Widget
    {
        bool _isMouseOver = false;

        public TextArea Text { get; private set; }

        public static TextAreaEnclosure Enclosure { get; set; } = 
            new TextAreaEnclosure(" [ ", " ] ", new ColorPair());

        public bool IsSimple { get; private set; }

        public Button(string text) 
            : this(
                new LayoutRect(
                    new FlexSize(text.Length + Enclosure.TotalWidth),
                    new Size(1)),
                text)
        { }
        public Button(LayoutRect bounds, string text, bool isSimple = false) : base(bounds)
        {
            Text = TextArea.Parse(
                text, 
                Width - Enclosure.TotalWidth, 
                justification: HJustification.CENTER);
            IsSimple = isSimple;
        }

        public event EventHandler Click;

        protected override void RefreshVisual() { }

        protected override void Draw()
        {
            if (_isMouseOver)
                Visual.FillCell(new Cell(' ', Style.GetColorPair("Hover")));
            else
                Visual.FillCell(new Cell(' ', Style.GetColorPair("Default")));

            if (IsSimple)
                Visual.DrawTextArea(Text);
            else
                Visual.Merge(Enclosure.DrawEnclosure(Text), HJustification.CENTER, VJustification.CENTER);
        }

        protected override void Input(InputEventArgs e)
        {
            switch (e)
            {
                case MouseEnterInputEventArgs _:
                    _isMouseOver = true;
                    Render();
                    break;
                case MouseLeaveInputEventArgs _:
                    _isMouseOver = false;
                    Render();
                    break;
                case MouseClickInputEventArgs c:
                    OnClick(EventArgs.Empty);
                    break;
                default:
                    // ok, just do nothing.
                    break;
            }
        }

        private void OnClick(EventArgs e)
        {
            EventHandler handler = Click;
            handler?.Invoke(this, e);
        }
    }
}
