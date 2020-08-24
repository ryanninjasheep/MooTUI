using System;
using MooTUI.Drawing;
using System.Windows;
using Media = System.Windows.Media;
using SystemInput = System.Windows.Input;
using System.ComponentModel;
using System.Windows.Navigation;
using MooTUI.Input;
using System.Collections.Generic;

namespace MooTUI.Core.WPF
{
    /// <summary>
    /// Intermediary between WPF and MooTUI
    /// </summary>
    public class WPFMooViewer : FrameworkElement, IMooViewer
    {
        private Media.GlyphTypeface glyphTypeface;
        private double fontSize;
        private double cellWidth;
        private double cellHeight;

        private Visual Visual { get; set; }
        private Theme Theme { get; set; }

        private (int x, int y) AbsoluteMouseLocation { get; set; }

        /// <summary>
        /// Width and height in terms of CELLS, not pixels.
        /// </summary>
        public WPFMooViewer(int width, int height, Theme theme)
        {
            LoadTypeface("Consolas", 13, 7, 15);

            EventManager.RegisterClassHandler(typeof(Window), 
                SystemInput.Keyboard.KeyDownEvent, new SystemInput.KeyEventHandler(OnKeyDown), true);
            EventManager.RegisterClassHandler(typeof(Window), 
                SystemInput.Keyboard.KeyUpEvent, new SystemInput.KeyEventHandler(OnKeyUp), true);

            Theme = theme;

            SetSize(width, height);
        }

        private void LoadTypeface(string getFamily, int getFontSize, int getCellWidth, int getCellHeight)
        {
            Media.FontFamily family = new Media.FontFamily(getFamily);
            Media.Typeface typeface = new Media.Typeface(family,
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal);

            typeface.TryGetGlyphTypeface(out glyphTypeface);

            fontSize = getFontSize;
            cellWidth = getCellWidth;
            cellHeight = getCellHeight;
        }

        public void SetSize(int width, int height)
        {
            Width = width * cellWidth;
            Height = height * cellHeight;
        }

        #region RENDERING

        public void SetVisual(Visual v)
        {
            Visual = v;
            InvalidateVisual();
        }

        protected override void OnRender(Media.DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(
                new Media.SolidColorBrush(Theme.Palette[Color.Base03]),
                null, new Rect(0, 0, Width, Height)
                );

            if (Visual is null)
                return;

            // Background
            for (int j = 0; j < Visual.Height; j++) // Go row by row
            {
                int cursorX = 0;
                for (int i = 0; i < Visual.Width; i++)
                {
                    if (Visual[i, j].Back != Visual[cursorX, j].Back)
                    {
                        DrawBackground(cursorX, j, i - cursorX, dc);
                        cursorX = i;
                    }
                }
                DrawBackground(cursorX, j, Visual.Width - cursorX, dc); ;
            }

            // Foreground
            for (int j = 0; j < Visual.Height; j++) // Go row by row
            {
                int cursorX = 0;
                for (int i = 0; i < Visual.Width; i++)
                {
                    if (Visual[i, j].Fore != Visual[cursorX, j].Fore)
                    {
                        DrawGlyphRun(cursorX, j, i - cursorX, dc);
                        cursorX = i;
                    }
                }
                DrawGlyphRun(cursorX, j, Visual.Width - cursorX, dc);
            }
        }
        private void DrawGlyphRun(int xIndex, int yIndex, int length, Media.DrawingContext dc)
            // Assumes all glyphs are the same color
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = Visual[xIndex + i, yIndex].Char ?? ' ';
            }

            ushort[] charIndexes = new ushort[length];
            double[] advanceWidths = new double[length];
            for (int i = 0; i < length; i++)
            {
                try
                {
                    charIndexes[i] = glyphTypeface.CharacterToGlyphMap[chars[i]];
                }
                catch (KeyNotFoundException)
                {
                    charIndexes[i] = glyphTypeface.CharacterToGlyphMap[' '];
                }
                advanceWidths[i] = cellWidth;
            }

            Media.GlyphRun g = new Media.GlyphRun((float)1.25);
            ISupportInitialize isi = g;
            isi.BeginInit();
            {
                g.GlyphTypeface = glyphTypeface;
                g.FontRenderingEmSize = fontSize;
                g.GlyphIndices = charIndexes;
                g.AdvanceWidths = advanceWidths;
                g.BaselineOrigin = new Point(xIndex * cellWidth, 
                    yIndex * cellHeight + glyphTypeface.Baseline * fontSize);
            }
            isi.EndInit();

            dc.DrawGlyphRun(new Media.SolidColorBrush(GetColor(Visual[xIndex, yIndex].Fore)), g); ;
        }
        private void DrawBackground(int xIndex, int yIndex, int length, Media.DrawingContext dc)
            // Assumes all same color
        {
            dc.DrawRectangle(new Media.SolidColorBrush(GetColor(Visual[xIndex, yIndex].Back)), null,
                new Rect(xIndex * cellWidth, yIndex * cellHeight, length * cellWidth + 1, cellHeight + 1));
        }

        private Media.Color GetColor(Color c) => Theme.Palette[c];

        #endregion

        #region INPUT HANDLING

        public event EventHandler<InputEventArgs> InputEventHandler;
        protected void OnInputReceived(InputEventArgs e)
        {
            EventHandler<InputEventArgs> handler = InputEventHandler;
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
            int x = (int)Math.Floor(p.X / cellWidth);
            int y = (int)Math.Floor(p.Y / cellHeight);
            return (x, y);
        }

        #endregion
    }
}
