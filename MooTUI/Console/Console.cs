using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MooTUI.Core;
using MooTUI.Core.WPF;
using MooTUI.Text;

namespace MooTUI.Console
{
    public class Console
    {
        private ConsoleWidget Widget { get; }

        /// <summary>
        /// Generates and displays a new Console.
        /// </summary>
        /// <param name="width">The width of the console (must be at least 8)</param>
        /// <param name="height">The height of the console (must be at least 4)</param>
        public Console(int width, int height) : this(width, height, null, Drawing.Theme.Basic.Value) { }
        /// <summary>
        /// Generates and displays a new Console.
        /// </summary>
        /// <param name="width">The width of the console (must be at least 8)</param>
        /// <param name="height">The height of the console (must be at least 4)</param>
        public Console(int width, int height, TextSpan title)
            : this(width, height, title, Drawing.Theme.Basic.Value) { }
        /// <summary>
        /// Generates and displays a new Console.
        /// </summary>
        /// <param name="width">The width of the console (must be at least 8)</param>
        /// <param name="height">The height of the console (must be at least 4)</param>
        /// <param name="theme">The color scheme of the console (optional)</param>
        public Console(int width, int height, TextSpan? title, Drawing.Theme theme)
        {
            if (width < 8)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 8.");
            if (height < 4)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be at least 4.");

            Widget = new ConsoleWidget(new Layout.LayoutRect(width, height), title);

            Thread thread = new Thread(() => DisplayWindow(width, height, theme ?? Drawing.Theme.Basic.Value));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            Widget.Render();
        }

        public void Write(string s) => Widget.Write(s);
        public void Write(TextSpan s) => Widget.Write(s);

        public void WriteLine(string s)
        {
            Widget.Write(s);
            Widget.Write("\n");
        }
        public void WriteLine(TextSpan s)
        {
            Widget.Write(s);
            Widget.Write("\n");
        }

        public void WriteParse(string s) => Write(TextSpan.Parse(s));
        public void WriteLineParse(string s) => WriteLine(TextSpan.Parse(s));

        public string ReadLine()
        {
            char c;
            do
            {
                c = ReadChar();

            }
            while (c != '\n');
            return Widget.InputManager.LineInput;
        }
        public char ReadChar() => ReadChar(false);
        public char ReadChar(bool consume) => Widget.ReadChar(consume);

        private void DisplayWindow(int width, int height, Drawing.Theme theme)
        {
            WPFFormattedTextViewer viewer = 
                new WPFFormattedTextViewer(width, height, 8, 17, 14.5, theme);

            Window window = new Window();
            window.Content = viewer;
            window.Background = new SolidColorBrush(theme.Palette[MooTUI.Drawing.Color.Base03]);
            window.SizeToContent = SizeToContent.WidthAndHeight;

            new MooInterface(viewer, Widget);
            Widget.ClaimFocus();
            Widget.Render();

            window.ShowDialog();
        }
    }
}
