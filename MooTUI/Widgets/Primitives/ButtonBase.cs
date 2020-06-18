using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Input;

namespace MooTUI.Widgets.Primitives
{
    public abstract class ButtonBase : Widget
    {
        public ButtonBase(int width, int height) : base(width, height) { }

        #region INPUT

        protected override void Input(InputEventArgs e)
        {
            switch (e.InputType)
            {
                case InputTypes.MOUSE_ENTER:
                    OnMouseEnter();
                    break;
                case InputTypes.MOUSE_LEAVE:
                    OnMouseLeave();
                    break;
                case InputTypes.LEFT_CLICK:
                    OnLeftClick(e);
                    break;
                default:
                    // ok, just do nothing.
                    break;

            }
        }

        protected virtual void OnMouseEnter() { }
        protected virtual void OnMouseLeave() { }

        protected virtual void OnLeftClick(InputEventArgs e)
        {
            OnClick(EventArgs.Empty);

            e.Handled = true;
        }

        public event EventHandler Click;
        protected void OnClick(EventArgs e)
        {
            EventHandler handler = Click;
            handler?.Invoke(this, e);
        }

        #endregion
    }
}
