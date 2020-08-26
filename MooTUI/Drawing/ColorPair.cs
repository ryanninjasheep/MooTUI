using System;

namespace MooTUI.Drawing
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

        /// <summary>
        /// First, ensures argument is properly formatted ('red/green'), then returns the appropriate ColorPair.  If the
        /// argument is not formatted properly, returns the empty ColorPair.
        /// </summary>
        public static ColorPair Parse(string s, Style? style = null)
        {
            int i = s.IndexOf('/');
            if (i != -1)
            {
                string foreText = s.Substring(0, i);
                string backText = s.Substring(i + 1);

                Color fore = Color.None;
                Color back = Color.None;
                Enum.TryParse(foreText, true, out fore);
                Enum.TryParse(backText, true, out back);
                return new ColorPair(fore, back);
            }
            else if (style != null)
            {
                try
                {
                    return style.GetColorPair(s);
                }
                catch
                {
                    return new ColorPair();
                }
            }
            else
            {
                return new ColorPair();
            }
        }

        public ColorPair Invert() => new ColorPair(Back, Fore);

        public override bool Equals(object obj) => obj is ColorPair c ? Equals(c) : false;
        public override int GetHashCode() => (int)Fore ^ (int)Back;

        public bool Equals(ColorPair other) => Fore == other.Fore && Back == other.Back;

        public static bool operator ==(ColorPair a, ColorPair b) => a.Equals(b);
        public static bool operator !=(ColorPair a, ColorPair b) => !a.Equals(b);
    }
}
