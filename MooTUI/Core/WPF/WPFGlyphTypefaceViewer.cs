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
    public class WPFGlyphTypefaceViewer : WPFMooViewer
    {
        private Media.GlyphTypeface glyphTypeface;
        private double fontSize;

        /// <summary>
        /// Width and height in terms of CELLS, not pixels.
        /// </summary>
        public WPFGlyphTypefaceViewer(int width, int height, Theme theme) : base(width, height, theme)
        {
            glyphTypeface = LoadTypeface("Consolas", 13, 7, 15);

            SetSize(width, height);
        }

        private Media.GlyphTypeface LoadTypeface(string getFamily, int getFontSize, int getCellWidth, int getCellHeight)
        {
            Media.GlyphTypeface glyphTypeface;

            Media.FontFamily family = new Media.FontFamily(getFamily);
            Media.Typeface typeface = new Media.Typeface(family,
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal);

            typeface.TryGetGlyphTypeface(out glyphTypeface);

            fontSize = getFontSize;
            CellWidth = getCellWidth;
            CellHeight = getCellHeight;

            return glyphTypeface;
        }

        protected override void Render(Media.DrawingContext dc)
        {
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
                chars[i] = Visual![xIndex + i, yIndex].Char ?? ' ';
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
                advanceWidths[i] = CellWidth;
            }

            Media.GlyphRun g = new Media.GlyphRun((float)1);
            ISupportInitialize isi = g;
            isi.BeginInit();
            {
                g.GlyphTypeface = glyphTypeface;
                g.FontRenderingEmSize = fontSize;
                g.GlyphIndices = charIndexes;
                g.AdvanceWidths = advanceWidths;
                g.BaselineOrigin = new Point(xIndex * CellWidth, 
                    (int)(yIndex * CellHeight + glyphTypeface.Baseline * fontSize));
            }
            isi.EndInit();

            dc.DrawGlyphRun(new Media.SolidColorBrush(GetColor(Visual![xIndex, yIndex].Fore)), g); ;
        }
        private void DrawBackground(int xIndex, int yIndex, int length, Media.DrawingContext dc)
            // Assumes all same color
        {
            dc.DrawRectangle(new Media.SolidColorBrush(GetColor(Visual![xIndex, yIndex].Back)), null,
                new Rect(xIndex * CellWidth, yIndex * CellHeight, length * CellWidth + 1, CellHeight + 1));
        }
    }
}
