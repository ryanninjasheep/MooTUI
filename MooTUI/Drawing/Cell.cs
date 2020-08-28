using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Drawing
{
    public readonly struct Cell : IEquatable<Cell>
    {
        public char? Char { get; }
        public Color Fore { get; }
        public Color Back { get; }

        public Cell(char? c, Color fore, Color back)
        {
            Char = c is char ch && char.IsWhiteSpace(ch) ? ' ' : c;
            Fore = fore;
            Back = back;
        }
        public Cell(char? c, ColorPair colors) : this(c, colors.Fore, colors.Back) { }

        public Cell WithChar(char c) => new Cell(c, Fore, Back);
        public Cell WithFore(Color c) => new Cell(Char, c, Back);
        public Cell WithBack(Color c) => new Cell(Char, Fore, c);
        public Cell WithColors(ColorPair c) => new Cell(Char, c.Fore, c.Back);

        public Cell Overlay(Cell c) => 
            new Cell(
                c.Char ?? Char,
                c.Fore == Color.None ? Fore : c.Fore,
                c.Back == Color.None ? Back : c.Back
            );

        public override bool Equals(object obj) => obj is ColorPair c ? Equals(c) : false;
        public override int GetHashCode() => (int)Fore ^ (int)Back ^ Char ?? 0;

        public bool Equals(Cell other) => Fore == other.Fore && Back == other.Back && Char == other.Char;

        public static bool operator ==(Cell a, Cell b) => a.Equals(b);
        public static bool operator !=(Cell a, Cell b) => !a.Equals(b);
    }
}
