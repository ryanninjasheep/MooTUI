using MooTUI.Core;
using MooTUI.Text;
using MooTUI.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MooTUI.IO
{
    /// <summary>
    /// Contains all data for rendering a View, as well as helper functions for merging Views.
    /// </summary>
    public class View
    {
        public int Width { get => Visual.Width; }
        public int Height { get => Visual.Height; }

        public Visual Visual { get; private set; }

        public View(int width, int height, Color defaultFore, Color defaultBack)
        {
            Visual = new Visual(width, height);

            Visual.FillForeColor(defaultFore);
            Visual.FillBackColor(defaultBack);
            Visual.FillChar(' ');
        }
        public View(int getWidth, int getHeight) 
            : this(getWidth, getHeight, Color.Base03, Color.Base0) { }

        #region PUBLIC HELPER FUNCTIONS

        public void FillColorScheme(ColorScheme fill) => FillColorScheme(fill, 0, 0, Width, Height);
        public void FillForeColor(Color fill) => FillForeColor(fill, 0, 0, Width, Height);
        public void FillBackColor(Color fill) => FillBackColor(fill, 0, 0, Width, Height);
        public void FillChar(char fill) => FillChar(fill, 0, 0, Width, Height);

        public void FillColorScheme(ColorScheme fill, int xStart, int yStart, int width, int height)
        {
            FillForeColor(fill.Fore, xStart, yStart, width, height);
            FillBackColor(fill.Back, xStart, yStart, width, height);
        }
        public void FillForeColor(Color fill, int xStart, int yStart, int width, int height) =>
            ApplyShader(Shaders.Fill(fill), true, false, xStart, yStart, width, height);
        public void FillBackColor(Color fill, int xStart, int yStart, int width, int height) => 
            ApplyShader(Shaders.Fill(fill), false, true, xStart, yStart, width, height);
        public void FillChar(char fill, int xStart, int yStart, int width, int height)
        {
            for (int i = xStart; i < Width && i - xStart < width; i++)
            {
                for (int j = yStart; j < Height && j - yStart < height; j++)
                {
                    Visual[i, j] = Visual[i, j].WithChar(fill);
                }
            }
        }

        public void SetColorScheme(int x, int y, ColorScheme c)
        {
            SetForeColor(x, y, c.Fore);
            SetBackColor(x, y, c.Back);
        }
        public void SetForeColor(int x, int y, Color c) => Visual[x, y] = Visual[x, y].WithFore(c);
        public void SetBackColor(int x, int y, Color c) => Visual[x, y] = Visual[x, y].WithBack(c);
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


        public void DrawSpan(TextSpan span, HJustification h = HJustification.CENTER, VJustification v = VJustification.CENTER)
        {
            Visual visual = span.Draw();

            Visual.Merge(visual, h.GetOffset(visual.Width, Width), v.GetOffset(visual.Height, Height));
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

        public void Merge(View v) => Visual.Merge(v.Visual);
        public void Merge(View v, int xIndex, int yIndex) => Visual.Merge(v.Visual, xIndex, yIndex);
        public void Merge(View v, int xIndex, int yIndex, int xStart, int yStart, int width, int height) =>
            Visual.Merge(v.Visual, xIndex, yIndex, xStart, yStart, width, height);

        /// <summary>
        /// Applies a particular function to all cells in the View.
        /// </summary>
        public void ApplyShader(Func<Color, int, int, Color> shader, 
            bool affectFore, bool affectBack) =>
            ApplyShader(shader, affectFore, affectBack, 0, 0, Width, Height);
        /// <summary>
        /// Applies a particular function to a certain range of cells in the View.
        /// </summary>
        public void ApplyShader(Func<Color, int, int, Color> shader, 
            bool affectFore, bool affectBack,
            int xStart, int yStart, int width, int height)
        {
            for (int i = xStart; i < Width && i - xStart < width; i++)
            {
                for (int j = yStart; j < Height && j - yStart < height; j++)
                {
                    Cell shaded = Visual[i, j];

                    if (affectFore)
                        shaded = shaded.WithFore(shader(shaded.Fore, i, j));

                    if (affectBack)
                        shaded = shaded.WithBack(shader(shaded.Back, i, j));

                    Visual[i, j] = shaded;
                }
            }
        }

        #endregion

        #region PRIVATE HELPER FUNCTIONS

        private int GetCenterStart(int baseLength, int overlayLength) =>
            (int)Math.Floor((double)(baseLength - overlayLength) / 2);

        #endregion
    }
}
