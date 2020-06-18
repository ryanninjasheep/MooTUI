using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Input;
using MooTUI.IO;

namespace MooTUI.Widgets
{
    public class ExpandingTextBox : TextBox
    {
        public enum ExpansionDirection { RIGHT, DOWN }
        private ExpansionDirection Direction { get; }
        public int MinSize { get; private set; }

        /// <summary>
        /// Creates a textbox that expands horizontally.
        /// </summary>
        public ExpandingTextBox(int minWidth) : base(minWidth, 1)
        {
            Direction = ExpansionDirection.RIGHT;
            MinSize = minWidth;
        }
        /// <summary>
        /// Creates a textbox that expands vertically.
        /// </summary>
        public ExpandingTextBox(int width, int minHeight) : base(width, minHeight)
        {
            Direction = ExpansionDirection.DOWN;
            MinSize = minHeight;
        }

        protected override void Input(InputEventArgs e)
        {
            base.Input(e);

            if (e.InputType == InputTypes.KEY_DOWN)
            {
                UpdateSize();
            }
        }

        private void UpdateSize()
        {
            if (Direction == ExpansionDirection.RIGHT)
            {
                int lineLength = Text.Length + 1;
                if (lineLength != Width)
                {
                    Resize(Math.Max(lineLength, MinSize), 1);
                    OnEnsureCursorVisible();
                    Render();
                }
            }
            else if (Direction == ExpansionDirection.DOWN)
            {
                int height = Span.Height + 1;
                if (height != Height)
                {
                    Resize(Width, Math.Max(height, MinSize));
                    OnEnsureCursorVisible();
                    Render();
                }
            }
        }
    }
}
