using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
        public Console(int width, int height) : this(width, height, Drawing.Theme.Basic.Value) { }
        /// <summary>
        /// Generates and displays a new Console.
        /// </summary>
        /// <param name="width">The width of the console (must be at least 8)</param>
        /// <param name="height">The height of the console (must be at least 4)</param>
        /// <param name="theme">The color scheme of the console (optional)</param>
        public Console(int width, int height, Drawing.Theme theme)
        {
            if (width < 8)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 8.");
            if (height < 4)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be at least 4.");

            Widget = new ConsoleWidget(new Layout.LayoutRect(width, height));

            Thread thread = new Thread(() => DisplayWindow(width, height, theme ?? Drawing.Theme.Basic.Value));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            Widget.Render();
        }

        public void Write(string s) { }
        public void Write(TextSpan s) { }

        public void WriteLine(string s) { }
        public void WriteLine(TextSpan s) { }

        public void WriteParse(string s) { }
        public void WriteLineParse(string s) { }

        public string ReadLine()
        {
            string result = "";
            char c;
            do
            {
                c = ReadChar(false);
                result += c;

            }
            while (c != '\n');
            return result;
        }
        public char ReadChar() => ReadChar(false);
        public char ReadChar(bool consume) => Widget.ReadChar(consume);

        private void DisplayWindow(int width, int height, Drawing.Theme theme)
        {
            WPFFormattedTextViewer viewer = new WPFFormattedTextViewer(width, height, theme);

            Window window = new Window();
            window.Content = viewer;

            new MooInterface(viewer, Widget);
            Widget.ClaimFocus();

            //System.Console.WriteLine("Showing window");
            Widget.Render();
            window.ShowDialog();
            //System.Console.WriteLine("Window closed");
        }
    }
}
