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
        public TextSpan Text { get; private set; }

        public Button(TextSpan text, LayoutRect bounds) : base(bounds)
        {
            Text = text;
            View.FillColorScheme(Style.GetColorPair("Default"));
        }

        public event EventHandler Click;

        protected override void Draw()
        {
            View.ClearText();
            View.DrawSpan(Text);
        }

        protected override void Input(InputEventArgs e)
        {
            switch (e.InputType)
            {
                case InputTypes.MOUSE_ENTER:
                    View.FillColorScheme(Style.GetColorPair("Hover"));
                    break;
                case InputTypes.MOUSE_LEAVE:
                    View.FillColorScheme(Style.GetColorPair("Default"));
                    break;
                case InputTypes.LEFT_CLICK:
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
