﻿using System;
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
    }
}
