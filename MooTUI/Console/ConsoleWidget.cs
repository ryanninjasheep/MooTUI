using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Input;
using MooTUI.Layout;
using MooTUI.Text;
using MooTUI.Widgets;
using MooTUI.Widgets.Primitives;

namespace MooTUI.Console
{
    internal class ConsoleWidget : MonoContainer
    {
        private ConsoleDisplay Display => ((Content as MonoContainer)!.Content as ConsoleDisplay)!;

        public InputManager InputManager { get; }

        public ConsoleWidget(LayoutRect bounds, TextSpan? title = null, BoxDrawing? lineStyle = null) : base(bounds)
        {
            ConsoleDisplay display = new ConsoleDisplay(bounds.Width - 2);

            InputManager = new InputManager();

            SetContent(new ScrollBox(
                Bounds.Clone(),
                display,
                text: title,
                lineStyle: lineStyle));
        }

        public char ReadChar(bool consume)
        {
            InputManager.AllowInput = true;
            InputManager.ConsumeInput = consume;

            DrawInputBuffer();

            char result = InputManager.CharInput.Task.Result;
            InputManager.AllowInput = false;

            if (result == '\n')
            {
                Display.Write(InputManager.LineInput);
                Display.Render();
            }

            return result;
        }

        public void Write(string s) 
        {
            Display.Write(s);
            Display.Render();
        }
        public void Write(TextSpan s)
        {
            Display.Write(s);
            Display.Render();
        }

        protected override void Input(InputEventArgs e)
        {
            if (InputManager.AllowInput && e is KeyboardInputEventArgs k)
            {
                InputManager.HandleInput(k);

                DrawInputBuffer();

                k.Handled = true;
            }
        }

        public override Widget GetHoveredWidget((int x, int y) relativeMouseLocation) => Content;

        protected override void DrawChild(Widget child)
        {
            Visual.Merge(Content.Visual);
        }

        protected override void OnChildResized(Widget child) =>
            throw new InvalidOperationException("ScrollBox child should never move!");

        protected override void RefreshVisual() => DrawChild(Content);

        protected internal override (int xOffset, int yOffset) GetChildOffset(Widget child) => (0, 0);

        private void DrawInputBuffer()
        {
            Display.InputBuffer = InputManager.GenerateDisplay(Display.Width, Display.CursorX);

            Display.Render();
        }
    }
}
