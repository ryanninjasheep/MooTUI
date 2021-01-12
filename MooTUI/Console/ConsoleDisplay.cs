using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Widgets.Primitives;
using MooTUI.Text;
using MooTUI.Layout;
using MooTUI.Input;
using MooTUI.Drawing;

namespace MooTUI.Console
{
    internal class ConsoleDisplay : Widget
    {
        private (int x, int y) cursor;

        private Visual Buffer { get; set; }

        public Visual? InputBuffer { get; set; }

        public int CursorX 
        {
            get => cursor.x; 
            set
            {
                cursor.x = value;
                if (CursorX >= Width)
                {
                    NewLine();
                }

            }
        }
        public int CursorY
        {
            get => cursor.y;
            set
            {
                cursor.y = value;
                if (CursorY >= Height)
                {
                    Bounds.TryResize(Width, CursorY + 1);
                }
            }
        }

        public InputManager? InputManager { get; set; }

        public ConsoleDisplay(int width) : base(new LayoutRect(new Size(width), new FlexSize(1))) 
        {
            Buffer = new Visual(Width, Height);
        }

        public void AppendLine()
        {
            CursorX = 0;
            CursorY = Height;
        }
        public void NewLine()
        {
            CursorX = 0;
            CursorY++;
        }

        public void Write(char c, Color fore = Color.None, Color back = Color.None)
        {
            if (c == '\n')
            {
                NewLine();
                return;
            }

            Buffer[CursorX, CursorY] = new Cell(c, fore, back);
            CursorX++;

            Render();
        }

        public void Write(string s)
        {
            foreach (char c in s)
                Write(c);
        }

        public void Write(TextSpan s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                Cell c = s[i];
                Write(c.Char ?? ' ', c.Fore, c.Back);
            }
        }

        protected override void Input(InputEventArgs e) { }

        protected override void RefreshVisual() { }

        protected override void Resize()
        {
            Visual oldBuffer = Buffer;
            Buffer = new Visual(Width, Height);
            Buffer.Merge(oldBuffer);

            InputBuffer = null;

            EnsureRegionVisible(0, Height - 1);
        }

        protected override void Draw()
        {
            Visual = new Visual(Width, Height);
            Visual.Merge(Buffer);

            if (InputBuffer is Visual v)
            {
                Visual.Merge(InputBuffer, 0, CursorY);
            }
        }
    }
}
