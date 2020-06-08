using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MooTUI.Core
{
    public enum InputTypes
    {
        FOCUS, UNFOCUS,
        MOUSE_ENTER, MOUSE_LEAVE,
        MOUSE_MOVE,
        LEFT_CLICK, RIGHT_CLICK,
        SCROLL,
        KEY_DOWN,
    }

    public class InputEventArgs : EventArgs
    {
        private static readonly InputTypes[] MouseMovement = 
            { InputTypes.MOUSE_MOVE, InputTypes.MOUSE_ENTER, InputTypes.MOUSE_LEAVE };
        private static readonly InputTypes[] HoverDependent = 
            { InputTypes.LEFT_CLICK, InputTypes.RIGHT_CLICK, InputTypes.SCROLL };
        private static readonly InputTypes[] FocusDependent = 
            { InputTypes.KEY_DOWN };

        public InputTypes InputType { get; }

        public MouseContext Mouse { get; set; }
        public KeyboardContext Keyboard { get; set; }

        public bool Handled { get; set; }

        public InputEventArgs(InputTypes inputType, MouseContext mouse, KeyboardContext keyboard)
        {
            InputType = inputType;

            Mouse = mouse;
            Keyboard = keyboard;

            Handled = false;
        }

        public bool IsMouseMovement()
        {
            return MouseMovement.Contains(InputType);
        }
        public bool IsHoverDependent()
        {
            return HoverDependent.Contains(InputType);
        }
        public bool IsFocusDependent()
        {
            return FocusDependent.Contains(InputType);
        }

        public InputEventArgs CopyWithNewInputType(InputTypes i) => new InputEventArgs(i, Mouse, Keyboard);
    }
}
