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
        public int Width { get => Cells.GetLength(0); }
        public int Height { get => Cells.GetLength(1); }

        public Cell[,] Cells { get; set; }

        public Visual(int width, int height)
        {
            Cells = new Cell[width, height];
        }

        public Cell this[int x, int y] 
        {
            get => Cells[x, y]; 
            set => Cells[x, y] = value; 
        }

        public void FillChar(char fill)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Cells[i, j] = Cells[i, j].WithChar(fill);
                }
            }
        }
        public void FillForeColor(Color fill)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Cells[i, j] = Cells[i, j].WithFore(fill);
                }
            }
        }
        public void FillBackColor(Color fill)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Cells[i, j] = Cells[i, j].WithBack(fill);
                }
            }
        }
        public void FillCell(Cell fill)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Cells[i, j] = fill;
                }
            }
        }
    }
}
