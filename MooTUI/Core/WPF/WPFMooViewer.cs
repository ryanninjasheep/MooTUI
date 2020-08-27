using MooTUI.Drawing;
using MooTUI.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using SystemInput = System.Windows.Input;
using Media = System.Windows.Media;

namespace MooTUI.Core.WPF
{
    public abstract class WPFMooViewer : FrameworkElement, IMooViewer
    {
        public Visual? Visual { get; private set; }
        public Theme Theme { get; private set; }

        public int CellWidth { get; protected set; }
        public int CellHeight { get; protected set; }

        private (int x, int y) AbsoluteMouseLocation { get; set; }

        /// <summary>
        /// Width and height in terms of CELLS, not pixels.
        /// </summary>
        public WPFMooViewer(int width, int height, Theme theme)
        {
            EventManager.RegisterClassHandler(typeof(Window),
                SystemInput.Keyboard.KeyDownEvent, new SystemInput.KeyEventHandler(OnKeyDown), true);
            EventManager.RegisterClassHandler(typeof(Window),
                SystemInput.Keyboard.KeyUpEvent, new SystemInput.KeyEventHandler(OnKeyUp), true);

            Theme = theme;

            SetSize(width, height);
        }

        public void SetSize(int width, int height)
        {
            Width = width * CellWidth;
            Height = height * CellHeight;
        }

        public void SetVisual(Drawing.Visual v)
        {
            Visual = v;
            InvalidateVisual();
        }

        protected sealed override void OnRender(Media.DrawingContext drawingContext) => Render(drawingContext);

        protected abstract void Render(Media.DrawingContext dc);

        protected Media.Color GetColor(Color c) => Theme.Palette[c];

        public event EventHandler<InputEventArgs>? InputEventHandler;
        protected void OnInputReceived(InputEventArgs e)
        {
            EventHandler<InputEventArgs>? handler = InputEventHandler;
            handler?.Invoke(this, e);
        }

        public void OnKeyDown(object sender, SystemInput.KeyEventArgs e)
        {
            InputEventArgs.HandleKeyDown(e.Key);
            OnInputReceived(new KeyboardInputEventArgs(e.Key));
        }
        protected void OnKeyUp(object sender, SystemInput.KeyEventArgs e)
        {
            InputEventArgs.HandleKeyUp(e.Key);
        }

        // This may not be necessary if mouse move is still calledS
        //protected override void OnMouseEnter(SystemInput.MouseEventArgs e)
        //{
        //    base.OnMouseEnter(e);

        //    AbsoluteMouse = GetCellAtMousePosition();

        //    OnInputReceived(GenerateInputContext(InputTypes.MOUSE_ENTER));

        //}
        protected override void OnMouseLeave(SystemInput.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            // This may have to change!  It's basically an attempt to nullify the value.
            AbsoluteMouseLocation = (-1, -1);

            OnInputReceived(new MouseLeaveInputEventArgs());
        }
        protected override void OnMouseMove(SystemInput.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var c = GetCellAtMousePosition();
            if (c != AbsoluteMouseLocation)
            {
                // If cell changed...

                AbsoluteMouseLocation = c;

                OnInputReceived(new MouseMoveInputEventArgs(AbsoluteMouseLocation));
            }
        }

        protected override void OnMouseLeftButtonDown(SystemInput.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            OnInputReceived(new MouseClickInputEventArgs(AbsoluteMouseLocation,
                MouseClickInputEventArgs.MouseButton.LEFT));
        }
        protected override void OnMouseRightButtonDown(SystemInput.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            OnInputReceived(new MouseClickInputEventArgs(AbsoluteMouseLocation,
                MouseClickInputEventArgs.MouseButton.RIGHT));
        }

        protected override void OnMouseWheel(SystemInput.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            OnInputReceived(new ScrollInputEventArgs(AbsoluteMouseLocation, e.Delta));
        }

        private (int x, int y) GetCellAtMousePosition()
        {
            Point p = SystemInput.Mouse.GetPosition(this);
            int x = (int)Math.Floor(p.X / CellWidth);
            int y = (int)Math.Floor(p.Y / CellHeight);
            return (x, y);
        }
    }
}
