using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Media = System.Windows.Media;

namespace MooTUI.Drawing
{
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

        public static readonly Lazy<Theme> Solarized = new Lazy<Theme>(
            () => new Theme(new Dictionary<Color, Media.Color>()
        {
            { Color.None,       Colors.Transparent },
            { Color.Base03,     Media.Color.FromRgb(0x00, 0x2B, 0x36) },
            { Color.Base02,     Media.Color.FromRgb(0x07, 0x36, 0x42) },
            { Color.Base01,     Media.Color.FromRgb(0x58, 0x6E, 0x75) },
            { Color.Base00,     Media.Color.FromRgb(0x65, 0x7B, 0x83) },
            { Color.Base0,      Media.Color.FromRgb(0x83, 0x94, 0x96) },
            { Color.Base1,      Media.Color.FromRgb(0x93, 0xA1, 0xA1) },
            { Color.Base2,      Media.Color.FromRgb(0xEE, 0xE8, 0xD5) },
            { Color.Base3,      Media.Color.FromRgb(0xFD, 0xF6, 0xE3) },

            { Color.Red,        Media.Color.FromRgb(0xDC, 0x32, 0x2F) },
            { Color.Orange,     Media.Color.FromRgb(0xCB, 0x4B, 0x16) },
            { Color.Yellow,     Media.Color.FromRgb(0xB5, 0x89, 0x00) },
            { Color.Green,      Media.Color.FromRgb(0x85, 0x99, 0x00) },
            { Color.Cyan,       Media.Color.FromRgb(0x2A, 0xA1, 0x98) },
            { Color.Blue,       Media.Color.FromRgb(0x26, 0x8B, 0xD2) },
            { Color.Purple,     Media.Color.FromRgb(0x6C, 0x71, 0xC4) },
            { Color.Magenta,    Media.Color.FromRgb(0xD3, 0x36, 0x82) },

            { Color.AltRed,     Media.Color.FromRgb(0xAE, 0x1F, 0x1C) },
            { Color.AltOrange,  Media.Color.FromRgb(0x9A, 0x38, 0x10) },
            { Color.AltYellow,  Media.Color.FromRgb(0x8A, 0x68, 0x00) },
            { Color.AltGreen,   Media.Color.FromRgb(0x65, 0x74, 0x00) },
            { Color.AltCyan,    Media.Color.FromRgb(0x1F, 0x7A, 0x74) },
            { Color.AltBlue,    Media.Color.FromRgb(0x1C, 0x69, 0xA0) },
            { Color.AltPurple,  Media.Color.FromRgb(0x42, 0x47, 0xA5) },
            { Color.AltMagenta, Media.Color.FromRgb(0xA6, 0x23, 0x62) },
        }));

        public static readonly Lazy<Theme> Gruvbox = new Lazy<Theme>(
            () => new Theme(new Dictionary<Color, Media.Color>()
        {
            { Color.None,       Colors.Transparent },
            { Color.Base03,     Media.Color.FromRgb(0x3c, 0x38, 0x36) },
            { Color.Base02,     Media.Color.FromRgb(0x50, 0x49, 0x45) },
            { Color.Base01,     Media.Color.FromRgb(0x66, 0x5c, 0x54) },
            { Color.Base00,     Media.Color.FromRgb(0x7c, 0x6f, 0x64) },
            { Color.Base0,      Media.Color.FromRgb(0xa8, 0x99, 0x84) },
            { Color.Base1,      Media.Color.FromRgb(0xbd, 0xae, 0x93) },
            { Color.Base2,      Media.Color.FromRgb(0xd5, 0xc4, 0xa1) },
            { Color.Base3,      Media.Color.FromRgb(0xed, 0xdb, 0xb2) },

            { Color.Red,        Media.Color.FromRgb(0xcc, 0x24, 0x1d) },
            { Color.Orange,     Media.Color.FromRgb(0xd6, 0x5d, 0x0e) },
            { Color.Yellow,     Media.Color.FromRgb(0xd7, 0x99, 0x21) },
            { Color.Green,      Media.Color.FromRgb(0x98, 0x97, 0x1a) },
            { Color.Cyan,       Media.Color.FromRgb(0x68, 0x9d, 0x6a) },
            { Color.Blue,       Media.Color.FromRgb(0x45, 0x85, 0x88) },
            { Color.Purple,     Media.Color.FromRgb(0x8f, 0x3f, 0x71) },
            { Color.Magenta,    Media.Color.FromRgb(0xb1, 0x62, 0x86) },

            { Color.AltRed,     Media.Color.FromRgb(0xfb, 0x49, 0x34) },
            { Color.AltOrange,  Media.Color.FromRgb(0xfe, 0x80, 0x19) },
            { Color.AltYellow,  Media.Color.FromRgb(0xfa, 0xbd, 0x2f) },
            { Color.AltGreen,   Media.Color.FromRgb(0xb8, 0xbb, 0x26) },
            { Color.AltCyan,    Media.Color.FromRgb(0x8e, 0xc0, 0x7c) },
            { Color.AltBlue,    Media.Color.FromRgb(0x83, 0xa5, 0x98) },
            { Color.AltPurple,  Media.Color.FromRgb(0xb1, 0x62, 0x86) },
            { Color.AltMagenta, Media.Color.FromRgb(0xd3, 0x86, 0x9b) },
        }));

        public static readonly Lazy<Theme> Basic = new Lazy<Theme>(
            () => new Theme(new Dictionary<Color, Media.Color>()
        {
            { Color.None,       Colors.Transparent },
            { Color.Base03,     Media.Color.FromRgb(0x00, 0x00, 0x00) },
            { Color.Base02,     Media.Color.FromRgb(0x33, 0x33, 0x33) },
            { Color.Base01,     Media.Color.FromRgb(0x55, 0x55, 0x55) },
            { Color.Base00,     Media.Color.FromRgb(0x77, 0x77, 0x77) },
            { Color.Base0,      Media.Color.FromRgb(0x99, 0x99, 0x99) },
            { Color.Base1,      Media.Color.FromRgb(0xBB, 0xBB, 0xBB) },
            { Color.Base2,      Media.Color.FromRgb(0xDD, 0xDD, 0xDD) },
            { Color.Base3,      Media.Color.FromRgb(0xFF, 0xFF, 0xFF) },

            { Color.Red,        Media.Color.FromRgb(0xFF, 0x00, 0x00) },
            { Color.Orange,     Media.Color.FromRgb(0xFF, 0x99, 0x00) },
            { Color.Yellow,     Media.Color.FromRgb(0xFF, 0xFF, 0x00) },
            { Color.Green,      Media.Color.FromRgb(0x00, 0xFF, 0x00) },
            { Color.Cyan,       Media.Color.FromRgb(0x00, 0xFF, 0xFF) },
            { Color.Blue,       Media.Color.FromRgb(0x00, 0x00, 0xFF) },
            { Color.Purple,     Media.Color.FromRgb(0x99, 0x00, 0xFF) },
            { Color.Magenta,    Media.Color.FromRgb(0xFF, 0x00, 0xFF) },

            { Color.AltRed,     Media.Color.FromRgb(0xC3, 0x00, 0x00) },
            { Color.AltOrange,  Media.Color.FromRgb(0xC3, 0x74, 0x00) },
            { Color.AltYellow,  Media.Color.FromRgb(0xC3, 0xC3, 0x00) },
            { Color.AltGreen,   Media.Color.FromRgb(0x00, 0xC3, 0x00) },
            { Color.AltCyan,    Media.Color.FromRgb(0x00, 0xC3, 0xC3) },
            { Color.AltBlue,    Media.Color.FromRgb(0x00, 0x00, 0xC3) },
            { Color.AltPurple,  Media.Color.FromRgb(0x74, 0x00, 0xC3) },
            { Color.AltMagenta, Media.Color.FromRgb(0xC3, 0x00, 0xC3) },
        }));

    }
}
