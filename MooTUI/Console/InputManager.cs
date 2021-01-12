using MooTUI.Drawing;
using MooTUI.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MooTUI.Console
{
    internal class InputManager
    {
        public string Text { get; set; }

        public TaskCompletionSource<char> CharInput { get; set; }
        public string LineInput { get; set; }

        public bool AllowInput { get; set; }
        public bool ConsumeInput { get; set; }

        public int Cursor { get; private set; }

        public InputManager()
        {
            // Useless change to test a thing
            Text = "";
            CharInput = new TaskCompletionSource<char>();
            LineInput = "";
        }

        public void HandleInput(KeyboardInputEventArgs e)
        {
            if (e.Char is char c)
            {
                PushChar(c);
            }
            else if (e.Key == System.Windows.Input.Key.Enter)
            {
                PushChar('\n');
                LineInput = Text;
                Text = "";
                Cursor = 0;
            }
            else if (e.Key == System.Windows.Input.Key.Back)
            {
                Backspace();
            }
            else if (e.Key == System.Windows.Input.Key.Left)
            {
                Cursor--;
                if (Cursor < 0)
                    Cursor = 0;
            }
            else if (e.Key == System.Windows.Input.Key.Right)
            {
                Cursor++;
                if (Cursor > Text.Length)
                    Cursor = Text.Length;
            }
            else if (e.Key == System.Windows.Input.Key.Up)
            {
                Cursor = 0;
            }
            else if (e.Key == System.Windows.Input.Key.Down)
            {
                Cursor = Text.Length;
            }
        }

        public Visual GenerateDisplay(int width, int startX)
        {
            int length = Text.Length;
            if (Cursor == length)
                length++;

            int height = (length + startX) / width + 1;
            Visual v = new Visual(width, height);

            int cursorX = startX;
            int cursorY = 0;

            for (int i = 0; i < length; i++)
            {
                if (cursorX >= width)
                {
                    cursorX = 0;
                    cursorY++;
                }

                v[cursorX, cursorY] = new Cell(
                    i == Cursor ? '_' : Text[i],
                    Color.None,
                    Color.None
                    );

                cursorX++;
            }

            return v;
        }

        private void PushChar(char c)
        {
            if (!ConsumeInput)
            {
                Text = Text.Insert(Cursor, c.ToString());
                Cursor++;
            }

            CharInput.SetResult(c);
            CharInput = new TaskCompletionSource<char>();
        }

        private void Backspace()
        {
            if (Cursor > 0)
            {
                Text = Text.Substring(0, Cursor - 1) + Text.Substring(Cursor);
                Cursor--;
            }
        }
    }
}
