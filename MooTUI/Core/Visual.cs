using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MooTUI.Core
{
    public class Visual
    {
        public int Width { get; private set; }
        public int Height { get => Cells.Length / Width; }

        private Cell[] Cells { get; set; }

        public Visual(int width, int height)
        {
            Width = width;
            Cells = new Cell[width * height];
        }

        public Cell this[int x, int y]
        {
            get => Cells[x + y * Width];
            set => Cells[x + y * Width] = value;
        }

        public void FillChar(char fill)
        {
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = Cells[i].WithChar(fill);
            }
        }
        public void FillForeColor(Color fill)
        {
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = Cells[i].WithFore(fill);
            }
        }
        public void FillBackColor(Color fill)
        {
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = Cells[i].WithBack(fill);
            }
        }
        public void FillCell(Cell fill)
        {
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = fill;
            }
        }

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

                    this[i + xIndex, j + yIndex] = this[i + xIndex, j + yIndex].Overlay(top);
                }
            }
        }
    }
}
