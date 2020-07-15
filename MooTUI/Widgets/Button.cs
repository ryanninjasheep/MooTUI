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

        public static TextSpanEnclosure Enclosure { get; set; } = 
            new TextSpanEnclosure(" [ ", " ] ", new ColorPair());

        public Button(TextSpan text, LayoutRect bounds) : base(bounds)
        {
            Text = text;

            RefreshVisual();
        }

        public event EventHandler Click;

        protected override void RefreshVisual()
        {
            Visual.FillCell(new Cell(' ', Style.GetColorPair("Default")));
            Visual.Merge(Enclosure.DrawEnclosure(Text), HJustification.CENTER, VJustification.CENTER);
        }

        protected override void Input(InputEventArgs e)
        {
            switch (e.InputType)
            {
                case InputTypes.MOUSE_ENTER:
                    Visual.FillColors(Style.GetColorPair("Hover"));
                    Render();
                    break;
                case InputTypes.MOUSE_LEAVE:
                    Visual.FillColors(Style.GetColorPair("Default"));
                    Render();
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
