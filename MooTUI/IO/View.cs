using MooTUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Media = System.Windows.Media;

namespace MooTUI.IO
{
    public enum HJustification { LEFT, CENTER, RIGHT }
    public enum VJustification { TOP, CENTER, BOTTOM }

    /// <summary>
    /// Contains all data for rendering a View, as well as helper functions for merging Views.
    /// </summary>
    public class View
    {
        public int Width { get => Visual.Width; }
        public int Height { get => Visual.Height; }

        public Visual Visual { get; private set; }

        public View(int width, int height, Media.Color defaultFore, Media.Color defaultBack)
        {
            Visual = new Visual(width, height);

            Visual.FillForeColor(defaultFore);
            Visual.FillBackColor(defaultBack);
            Visual.FillChar(' ');
        }
        public View(int getWidth, int getHeight) 
            : this(getWidth, getHeight, Media.Colors.Transparent, Media.Colors.Transparent) { }

        #region PUBLIC HELPER FUNCTIONS

        public void FillColorScheme(ColorScheme fill) => FillColorScheme(fill, 0, 0, Width, Height);
        public void FillForeColor(Media.Color fill) => FillForeColor(fill, 0, 0, Width, Height);
        public void FillBackColor(Media.Color fill) => FillBackColor(fill, 0, 0, Width, Height);
        public void FillChar(char fill) => FillChar(fill, 0, 0, Width, Height);

        public void FillColorScheme(ColorScheme fill, int xStart, int yStart, int width, int height)
        {
            FillForeColor(fill.Fore, xStart, yStart, width, height);
            FillBackColor(fill.Back, xStart, yStart, width, height);
        }
        public void FillForeColor(Media.Color fill, int xStart, int yStart, int width, int height) =>
            ApplyShader(Shaders.Fill(fill), false, true, false, xStart, yStart, width, height);
        public void FillBackColor(Media.Color fill, int xStart, int yStart, int width, int height) => 
            ApplyShader(Shaders.Fill(fill), false, false, true, xStart, yStart, width, height);
        public void FillChar(char fill, int xStart, int yStart, int width, int height) =>
            ApplyShader(Shaders.FillChar(fill), true, false, false, xStart, yStart, width, height);

        public void SetColorScheme(int x, int y, ColorScheme c)
        {
            SetForeColor(x, y, c.Fore);
            SetBackColor(x, y, c.Back);
        }
        public void SetForeColor(int x, int y, Media.Color c) => Visual[x, y] = Visual[x, y].WithFore(c);
        public void SetBackColor(int x, int y, Media.Color c) => Visual[x, y] = Visual[x, y].WithBack(c);
        public void SetChar(int x, int y, char c) => Visual[x, y] = Visual[x, y].WithChar(c);

        public void SetText(string s) => SetText(s, 0, 0);
        public void SetText(string s, int xStart, int yStart)
        {
            int x = xStart;
            int y = yStart;
            foreach (char c in s)
            {
                if (x >= Width)
                {
                    return;
                }

                Visual[x, y] = Visual[x, y].WithChar(c);
                x++;
            }
        }

        public void DrawSpan(Span span)
        {
            int yStart = span.VJustification switch
            {
                VJustification.TOP => 0,
                VJustification.CENTER => GetCenterStart(Height, span.Height),
                VJustification.BOTTOM => Height - span.Height,
                _ => throw new NotImplementedException(),
            };

            foreach (string s in span.DisplayText)
            {
                int xStart = span.HJustification switch
                {
                    HJustification.LEFT => 0,
                    HJustification.CENTER => GetCenterStart(Width, s.Length),
                    HJustification.RIGHT => Width - s.Length - 1,
                    _ => throw new NotImplementedException(),
                };
                SetText(s, xStart, yStart);

                yStart++;

                if (yStart >= Height)
                    return;
            }
        }

        public void Merge(View v) => Merge(v.Visual);
        public void Merge(View v, int xIndex, int yIndex) => Merge(v.Visual, xIndex, yIndex);
        public void Merge(View v, int xIndex, int yIndex, int xStart, int yStart, int width, int height) =>
            Merge(v.Visual, xIndex, yIndex, xStart, yStart, width, height);

        public void Merge(Visual v) => Merge(v, 0, 0);
        /// <summary>
        /// Overlays a Visual at the specified location.
        /// </summary>
        public void Merge(Visual v, int xIndex, int yIndex) => Merge(v, xIndex, yIndex, 0, 0, v.Width, v.Height);
        /// <summary>
        /// Overlays a certain range of a Visual at the specified location.  
        /// X and Y Index refers to location in this Visual to begin overlay.
        /// </summary>
        public void Merge(Visual v, int xIndex, int yIndex, int xStart, int yStart, int width, int height)
        {
            for (int j = 0; j + yStart < v.Height && j + yIndex < Height && j < height; j++)
            {
                for (int i = 0; i + xStart < v.Width && i + xIndex < Width && i < width; i++)
                {
                    Cell top = v[i + xStart, j + yStart];

                    Visual[i + xIndex, j + yIndex] = Visual[i + xIndex, j + yIndex].Overlay(top);
                }
            }
        }

        /// <summary>
        /// Applies a particular function to all cells in the View.
        /// </summary>
        public void ApplyShader(Func<Cell[,], int, int, Cell> shader, 
            bool affectChar, bool affectFore, bool affectBack) =>
            ApplyShader(shader, affectChar, affectFore, affectBack, 0, 0, Width, Height);
        /// <summary>
        /// Applies a particular function to a certain range of cells in the View.
        /// </summary>
        public void ApplyShader(Func<Cell[,], int, int, Cell> shader, 
            bool affectChar, bool affectFore, bool affectBack,
            int xStart, int yStart, int width, int height)
        {
            Visual buffer = new Visual(width, height);

            for (int i = xStart; i < Width && i - xStart < width; i++)
            {
                for (int j = yStart; j < Height && j - yStart < height; j++)
                {
                    Cell shaded = shader(Visual.Cells, i, j);

                    char? c = affectChar ? shaded.Char : null;
                    Media.Color? fore = affectFore ? shaded.Fore : null;
                    Media.Color? back = affectBack ? shaded.Back : null;

                    buffer[i - xStart, j - yStart] = new Cell(c, fore, back);
                }
            }

            Merge(buffer, xStart, yStart);
        }

        #endregion

        #region PRIVATE HELPER FUNCTIONS

        private int GetCenterStart(int baseLength, int overlayLength) =>
            (int)Math.Floor((double)(baseLength - overlayLength) / 2);

        #endregion
    }
}
