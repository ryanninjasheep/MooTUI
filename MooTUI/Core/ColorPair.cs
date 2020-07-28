using System;

namespace MooTUI.Core
{
    public struct ColorPair : IEquatable<ColorPair>
    {
        public Color Fore { get; }
        public Color Back { get; }

        public ColorPair(Color fore, Color back)
        {
            Fore = fore;
            Back = back;
        }

        public ColorPair Invert() => new ColorPair(Back, Fore);

        public override bool Equals(object obj) => obj is ColorPair c ? Equals(c) : false;
        public override int GetHashCode() => (int)Fore ^ (int)Back;

        public bool Equals(ColorPair other) => Fore == other.Fore && Back == other.Back;

        public static bool operator ==(ColorPair a, ColorPair b) => a.Equals(b);
        public static bool operator !=(ColorPair a, ColorPair b) => !a.Equals(b);
    }
}
