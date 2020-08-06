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
    public class Toggle : Widget
    {
        bool _isMouseOver = false;

        public bool Checked { get; private set; }
        public TextSpan Label { get; private set; }

        public static TextAreaEnclosure Enclosure { get; set; } = new TextAreaEnclosure(" [", "] ");

        public Toggle(TextSpan label, bool on = false) 
            : this(
                  new LayoutRect(
                      new FlexSize(label.Length + Enclosure.TotalWidth + 1),
                      new Size(1)),
                  label,
                  on) { }
        public Toggle(LayoutRect bounds, TextSpan label, bool on = false) : base(bounds)
        {
            Checked = on;
            Label = label;
        }

        public event EventHandler Click;

        protected override void RefreshVisual() { }

        protected override void Draw()
        {
            if (_isMouseOver)
                Visual.FillCell(new Cell(' ', Style.GetColorPair("Hover")));
            else
                Visual.FillCell(new Cell(' ', Style.GetColorPair("Default")));

            TextSpan check = new TextSpan(Checked ? "x" : " ");

            Visual text = new Visual(Label.Length + Enclosure.TotalWidth + 1, 1);
            text.Merge(Enclosure.DrawEnclosure(check));
            text.DrawTextSpan(Label, Enclosure.TotalWidth + 1);

            Visual.Merge(text, HJustification.CENTER, VJustification.CENTER);
        }

        protected override void Input(InputEventArgs e)
        {
            switch (e.InputType)
            {
                case InputTypes.MOUSE_ENTER:
                    _isMouseOver = true;
                    Render();
                    break;
                case InputTypes.MOUSE_LEAVE:
                    _isMouseOver = false;
                    Render();
                    break;
                case InputTypes.LEFT_CLICK:
                    OnClick(EventArgs.Empty);
                    Render();
                    break;
                default:
                    // ok, just do nothing.
                    break;
            }
        }

        private void OnClick(EventArgs e)
        {
            Checked = !Checked;

            EventHandler handler = Click;
            handler?.Invoke(this, e);
        }
    }
}
