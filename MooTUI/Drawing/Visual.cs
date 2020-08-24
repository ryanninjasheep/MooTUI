using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Linq;
using System.Runtime.CompilerServices;
using MooTUI.Layout;
using System.Windows;

namespace MooTUI.Drawing
{
    public class Visual
    {
        public int Width { get; private set; }
        public int Height { get => Cells.Length / Width; }

        private Cell[] Cells { get; set; }

        public Visual(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Visual cannot have size less than 1");

            Width = width;
            Cells = new Cell[width * height];
        }
        public Visual(int width, int height, ColorPair initialColors) : this(width, height)
        {
            FillForeColor(initialColors.Fore);
            FillBackColor(initialColors.Back);
            FillChar(' ');
        }

        public Cell this[int x, int y]
        {
            get => Cells[x + y * Width];
            set => Cells[x + y * Width] = value;
        }

        public void SetChar(int x, int y, char c) => this[x, y] = this[x, y].WithChar(c);
        public void SetFore(int x, int y, Color c) => this[x, y] = this[x, y].WithFore(c);
        public void SetBack(int x, int y, Color c) => this[x, y] = this[x, y].WithBack(c);
        public void SetColors(int x, int y, ColorPair c) => this[x, y] = this[x, y].WithColors(c);

        public void FillChar(char fill, int xStart = 0, int yStart = 0) =>
            FillChar(fill, xStart, yStart, Width, Height);
        public void FillForeColor(Color fill, int xStart = 0, int yStart = 0) =>
            FillForeColor(fill, xStart, yStart, Width, Height);
        public void FillBackColor(Color fill, int xStart = 0, int yStart = 0) =>
            FillBackColor(fill, xStart, yStart, Width, Height);
        public void FillColors(ColorPair fill, int xStart = 0, int yStart = 0) =>
            FillColors(fill, xStart, yStart, Width, Height);
        public void FillCell(Cell fill, int xStart = 0, int yStart = 0) =>
            FillCell(fill, xStart, yStart, Width, Height);

        public void FillChar(char fill, int xStart, int yStart, int width, int height) =>
            Fill(
                (c) => c.WithChar(fill),
                xStart, yStart, width, height);
        public void FillForeColor(Color fill, int xStart, int yStart, int width, int height) =>
            Fill(
                (c) => c.WithFore(fill),
                xStart, yStart, width, height);
        public void FillBackColor(Color fill, int xStart, int yStart, int width, int height) =>
            Fill(
                (c) => c.WithBack(fill),
                xStart, yStart, width, height);
        public void FillColors(ColorPair fill, int xStart, int yStart, int width, int height) =>
            Fill(
                (c) => c.WithColors(fill),
                xStart, yStart, width, height);
        public void FillCell(Cell fill, int xStart, int yStart, int width, int height) =>
            Fill(
                (c) => fill,
                xStart, yStart, width, height);

        public void Merge(Visual v, HJustification hJust, VJustification vJust) =>
            Merge(v, hJust.GetOffset(v.Width, Width), vJust.GetOffset(v.Height, Height));
        /// <summary>
        /// Overlays a Visual at the specified location.
        /// </summary>
        public void Merge(Visual v, int xIndex = 0, int yIndex = 0, int xStart = 0, int yStart = 0) =>
            Merge(v, xIndex, yIndex, xStart, yStart, v.Width, v.Height);
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

                    this[i + xIndex, j + yIndex] = this[i + xIndex, j + yIndex].Overlay(top);
                }
            }
        }

        /// <summary>
        /// Performs a certain function on each Cell in a specified region of the Visual.
        /// </summary>
        private void Fill(Func<Cell, Cell> func, int xStart, int yStart, int width, int height)
        {
            for (int i = xStart; i < Width && i - xStart < width; i++)
            {
                for (int j = yStart; j < Height && j - yStart < height; j++)
                {
                    this[i, j] = func(this[i, j]);
                }
            }
        }
    }
}
