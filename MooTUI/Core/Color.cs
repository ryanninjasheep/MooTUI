using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Media = System.Windows.Media;

namespace MooTUI.Core
{
    public enum Color
    {
        None,
        Foreground,
        Background,
        Black,
        DarkGrey,
        LightGrey,
        White,
        Red,
        AltRed,
        Magenta,
        AltMagenta,
        Blue,
        AltBlue,
        Cyan,
        AltCyan,
        Green,
        AltGreen,
        Yellow,
        AltYellow
    }

    public class Theme
    {
        private static readonly List<Color> allColors = Enum.GetValues(typeof(Color)).Cast<Color>().ToList();

        public Dictionary<Color, Media.Color> Palette { get; private set; }

        public Theme(Dictionary<Color, Media.Color> palette)
        {
            Palette = palette;

            if (!EnsurePaletteIsComprehensive())
                throw new Exception("A palette must specify a System.Windows.Media color for every MooTUI color");
        }

        private bool EnsurePaletteIsComprehensive()
        {
            foreach (Color c in allColors)
            {
                if (!Palette.ContainsKey(c))
                    return false;
            }
            return true;
        }

        // By w0ng
        public static readonly Theme Hybrid = new Theme(new Dictionary<Color, Media.Color>()
        {
            { Color.None,       Colors.Transparent },
            { Color.Foreground, Media.Color.FromRgb(0xc5, 0xc8, 0xc6)},
            { Color.Background, Media.Color.FromRgb(0x1d, 0x1f, 0x21)},
            { Color.Black,      Media.Color.FromRgb(0x28, 0x2a, 0x2e)},
            { Color.DarkGrey,   Media.Color.FromRgb(0x37, 0x3b, 0x41)},
            { Color.LightGrey,  Media.Color.FromRgb(0x70, 0x78, 0x80)},
            { Color.White,      Media.Color.FromRgb(0xc5, 0xc8, 0xc6)},
            { Color.Red,        Media.Color.FromRgb(0xa5, 0x42, 0x42)},
            { Color.AltRed,     Media.Color.FromRgb(0xcc, 0x66, 0x66)},
            { Color.Magenta,    Media.Color.FromRgb(0x85, 0x67, 0x8f)},
            { Color.AltMagenta, Media.Color.FromRgb(0xb2, 0x94, 0xbb)},
            { Color.Blue,       Media.Color.FromRgb(0x5f, 0x81, 0x9d)},
            { Color.AltBlue,    Media.Color.FromRgb(0x81, 0xa2, 0xbe)},
            { Color.Cyan,       Media.Color.FromRgb(0x5e, 0x8d, 0x87)},
            { Color.AltCyan,    Media.Color.FromRgb(0x8a, 0xbe, 0xb7)},
            { Color.Green,      Media.Color.FromRgb(0x8c, 0x94, 0x40)},
            { Color.AltGreen,   Media.Color.FromRgb(0xb5, 0xbd, 0x68)},
            { Color.Yellow,     Media.Color.FromRgb(0xde, 0x93, 0x5f)},
            { Color.AltYellow,  Media.Color.FromRgb(0xf0, 0xc6, 0x74)},
        });
    }
}
