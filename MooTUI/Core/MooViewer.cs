﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Navigation;

namespace MooTUI.Core
{
    /// <summary>
    /// Intermediary between WPF and MooTUI; delegates all input to a Window
    /// </summary>
    public class MooViewer : FrameworkElement
    {
        private GlyphTypeface glyphTypeface;
        private double fontSize;
        private double cellWidth;
        private double cellHeight;

        private Visual Visual { get; set; }
        private Color BaseColor { get; set; }

        public MouseContext MouseContext { get; private set; }
        public KeyboardContext KeyboardContext { get; private set; }

        public int MaxContentWidth { get; private set; }
        public int MaxContentHeight { get; private set; }

        public MooViewer()
        {
            LoadTypeface("Consolas", 13, 7, 15);

            EventManager.RegisterClassHandler(typeof(Window), 
                Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            EventManager.RegisterClassHandler(typeof(Window), 
                Keyboard.KeyUpEvent, new KeyEventHandler(OnKeyUp), true);

            MouseContext = new MouseContext();
            KeyboardContext = new KeyboardContext();

            //TEMP
            BaseColor = Colors.White;

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            UpdateSize();
        }

        private void LoadTypeface(string getFamily, int getFontSize, int getCellWidth, int getCellHeight)
        {
            FontFamily family = new FontFamily(getFamily);
            Typeface typeface = new Typeface(family,
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal);

            typeface.TryGetGlyphTypeface(out glyphTypeface);

            fontSize = getFontSize;
            cellWidth = getCellWidth;
            cellHeight = getCellHeight;
        }

        #region RENDERING

        public void SetVisual(Visual v)
        {
            Visual = v;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(new SolidColorBrush(BaseColor), null, new Rect(0, 0, Width, Height));

            // Background
            for (int j = 0; j < Visual.Height; j++) // Go row by row
            {
                int cursorX = 0;
                for (int i = 0; i < Visual.Width; i++)
                {
                    if (Visual.GetBackColor(i, j) != Visual.GetBackColor(cursorX, j))
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
                    if (Visual.GetForeColor(i, j) != Visual.GetForeColor(cursorX, j))
                    {
                        DrawGlyphRun(cursorX, j, i - cursorX, dc);
                        cursorX = i;
                    }
                }
                DrawGlyphRun(cursorX, j, Visual.Width - cursorX, dc);
            }
        }
        private void DrawGlyphRun(int xIndex, int yIndex, int length, DrawingContext dc) // Assumes all glyphs are the same color
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = Visual.GetChar(xIndex + i, yIndex);
            }

            ushort[] charIndexes = new ushort[length];
            double[] advanceWidths = new double[length];
            for (int i = 0; i < length; i++)
            {
                charIndexes[i] = glyphTypeface.CharacterToGlyphMap[chars[i]];
                advanceWidths[i] = cellWidth;
            }

            GlyphRun g = new GlyphRun((float)1.25);
            ISupportInitialize isi = g;
            isi.BeginInit();
            {
                g.GlyphTypeface = glyphTypeface;
                g.FontRenderingEmSize = fontSize;
                g.GlyphIndices = charIndexes;
                g.AdvanceWidths = advanceWidths;
                g.BaselineOrigin = new Point(xIndex * cellWidth, yIndex * cellHeight + glyphTypeface.Baseline * fontSize);
            }
            isi.EndInit();

            dc.DrawGlyphRun(new SolidColorBrush(Visual.GetForeColor(xIndex, yIndex)), g); ;
        }
        private void DrawBackground(int xIndex, int yIndex, int length, DrawingContext dc) // Assumes all same color
        {
            dc.DrawRectangle(new SolidColorBrush(Visual.GetBackColor(xIndex, yIndex)), null,
                new Rect(xIndex * cellWidth, yIndex * cellHeight, length * cellWidth + 1, cellHeight + 1));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            UpdateSize();
        }
        public void UpdateSize()
        {
            if (Parent == null)
            {
                Width = 900;
                Height = 500;
            }

            MaxContentWidth = (int)Math.Floor(Width / cellWidth);
            MaxContentHeight = (int)Math.Floor(Height / cellHeight);
        }

        #endregion

        #region INPUT HANDLING

        public event EventHandler<InputEventArgs> InputEventHandler;
        protected void OnInputReceived(InputEventArgs e)
        {
            EventHandler<InputEventArgs> handler = InputEventHandler;
            handler?.Invoke(this, e);
        }

        public InputEventArgs GenerateInputContext(InputTypes i) =>
            new InputEventArgs(i, MouseContext, KeyboardContext);

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardContext.HandleKeyDown(e);
            OnInputReceived(GenerateInputContext(InputTypes.KEY_DOWN));
        }
        protected void OnKeyUp(object sender, KeyEventArgs e)
        {
            KeyboardContext.HandleKeyUp(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            MouseContext.SetAbsoluteMouse(GetCellAtMousePosition());

            OnInputReceived(GenerateInputContext(InputTypes.MOUSE_ENTER));

        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            // This may have to change!  It's basically an attempt to nullify the value.
            MouseContext.SetAbsoluteMouse((-1, -1));

            OnInputReceived(GenerateInputContext(InputTypes.MOUSE_LEAVE));
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var c = GetCellAtMousePosition();
            if (c != MouseContext.AbsoluteMouse)
            {
                // If cell changed...

                MouseContext.SetAbsoluteMouse(c);

                OnInputReceived(GenerateInputContext(InputTypes.MOUSE_MOVE));
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            OnInputReceived(GenerateInputContext(InputTypes.LEFT_CLICK));
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            OnInputReceived(GenerateInputContext(InputTypes.RIGHT_CLICK));
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            MouseContext.SetScrollDelta(e.Delta);

            OnInputReceived(GenerateInputContext(InputTypes.SCROLL));
        }

        private (int x, int y) GetCellAtMousePosition()
        {
            Point p = Mouse.GetPosition(this);
            int x = (int)Math.Floor(p.X / cellWidth);
            int y = (int)Math.Floor(p.Y / cellHeight);
            return (x, y);
        }

        #endregion
    }
}