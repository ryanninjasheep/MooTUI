using System;
using MooTUI.Drawing;
using System.Windows;
using Media = System.Windows.Media;
using SystemInput = System.Windows.Input;
using System.ComponentModel;
using System.Windows.Navigation;
using MooTUI.Input;
using System.Collections.Generic;
using System.Globalization;

namespace MooTUI.Core.WPF
{
    /// <summary>
    /// Intermediary between WPF and MooTUI
    /// </summary>
    public class WPFFormattedTextViewer : WPFMooViewer
    {
        private Media.Typeface typeface;

        private double emSize;

        /// <summary>
        /// Width and height in terms of CELLS, not pixels.
        /// </summary>
        public WPFFormattedTextViewer(int width, int height, 
            int cellWidth, int cellHeight, double emSize, 
            Theme theme)
            : base(width, height, theme)
        {
            typeface = new Media.Typeface("Consolas");

            CellWidth = cellWidth;
            CellHeight = cellHeight;

            this.emSize = emSize;

            SetSize(width, height);
        }

        public WPFFormattedTextViewer(int width, int height, Theme theme)
            : this(width, height, 7, 15, 12.725, theme) { }

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

            for (int j = 0; j < Visual.Height; j++)
            {
                string text = ConcatenateLine(Visual, j);

                Media.FormattedText formattedText = new Media.FormattedText(
                    text,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    typeface,
                    emSize,
                    new Media.SolidColorBrush(GetColor(Visual![0, j].Fore)),
                    100
                    );

                int cursorX = 0;
                for (int i = 0; i < Visual.Width; i++)
                {
                    if (Visual[i, j].Fore != Visual[cursorX, j].Fore)
                    {
                        formattedText.SetForegroundBrush(
                            new Media.SolidColorBrush(GetColor(Visual[cursorX, j].Fore)),
                            cursorX, 
                            i - cursorX
                            );
                        cursorX = i;
                    }
                }

                dc.DrawText(formattedText, new Point(0, CellHeight * j));
            }
        }

        private void DrawBackground(int xIndex, int yIndex, int length, Media.DrawingContext dc)
            // Assumes all same color
        {
            dc.DrawRectangle(new Media.SolidColorBrush(GetColor(Visual![xIndex, yIndex].Back)), null,
                new Rect(xIndex * CellWidth, yIndex * CellHeight, length * CellWidth + 1, CellHeight + 1));
        }

        private static string ConcatenateLine(Visual v, int line)
        {
            string text = "";
            for (int i = 0; i < v.Width; i++)
            {
                text += v[i, line].Char;
            }
            return text;
        }
    }
}
