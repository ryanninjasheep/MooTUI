using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace MooTUI.Core
{
    public readonly struct Cell
    {
        public char? Char { get; }
        public Color? Fore { get; }
        public Color? Back { get; }

        public Cell(char? c, Color? fore, Color? back)
        {
            Char = c;
            Fore = fore;
            Back = back;
        }

        public Cell WithChar(char c) => new Cell(c, Fore, Back);
        public Cell WithFore(Color c) => new Cell(Char, c, Back);
        public Cell WithBack(Color c) => new Cell(Char, Fore, c);

        public Cell Overlay(Cell c) => new Cell(
            c.Char ?? Char,
            c.Fore ?? Fore,
            c.Back ?? Back
            );
    }
}
