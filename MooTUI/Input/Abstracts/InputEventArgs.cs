using MooTUI.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MooTUI.Input
{
    public abstract class InputEventArgs : BubblingEventArgs
    {
        private static bool _shift;
        private static bool _ctrl;
        private static bool _alt;
        private static bool _caps;

        public bool Handled { get; set; }

        public bool Shift => _shift;
        public bool Ctrl => _ctrl;
        public bool Alt => _alt;
        public bool Caps => _caps;

        public InputEventArgs() : base(null)
        {
            Handled = false;
        }

        public static void HandleKeyDown(Key key)
        {
            if (key == Key.LeftShift || key == Key.RightShift)
                _shift = true;
            else if (key == Key.LeftCtrl || key == Key.RightCtrl)
                _ctrl = true;
            else if (key == Key.LeftAlt || key == Key.RightAlt)
                _alt = true;
        }

        public static void HandleKeyUp(Key key)
        {
            if (key == Key.LeftShift || key == Key.RightShift)
                _shift = false;
            else if (key == Key.LeftCtrl || key == Key.RightCtrl)
                _ctrl = false;
            else if (key == Key.LeftAlt || key == Key.RightAlt)
                _alt = false;
            else if (key == Key.CapsLock)
                _caps = !_caps;
        }
    }
}
