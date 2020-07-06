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
        /// <summary>
        /// No color.  Usually rendered as transparent.
        /// </summary>
        None,

        /// <summary>
        /// Darkest base color.  Background in dark themes.
        /// </summary>
        Base03,

        /// <summary>
        /// Second-darkest base color.  Secondary background in dark themes.
        /// </summary>
        Base02,

        /// <summary>
        /// Third-darkest base color.  Selection background in dark themes; emphasized foreground in light themes.
        /// </summary>
        Base01,

        /// <summary>
        /// Darker mid-tone base color.  Secondary foreground in dark themes; primary foreground in light themes.
        /// </summary>
        Base00,

        /// <summary>
        /// Lighter mid-tone base color.  Primary foreground in dark themes; secondary foreground in light themes.
        /// </summary>
        Base0,

        /// <summary>
        /// Third-lightest base color.  Emphasized foreground in dark themes; selection background in light themes.
        /// </summary>
        Base1,

        /// <summary>
        /// Second-lightest base color.  Secondary background in light themes.
        /// </summary>
        Base2,

        /// <summary>
        /// Lightest base color.  Background in light themes.
        /// </summary>
        Base3,

        Red,
        Orange,
        Yellow,
        Green,
        Cyan,
        Blue,
        Purple,
        Magenta,

        AltRed,
        AltOrange,
        AltYellow,
        AltGreen,
        AltCyan,
        AltBlue,
        AltPurple,
        AltMagenta
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

        public static readonly Theme Solarized = new Theme(new Dictionary<Color, Media.Color>()
        {
            { Color.None,       Colors.Transparent },
            { Color.Base03,     Media.Color.FromRgb(0x00, 0x2b, 0x36) },
            { Color.Base02,     Media.Color.FromRgb(0x07, 0x36, 0x42) },
            { Color.Base01,     Media.Color.FromRgb(0x58, 0x6e, 0x75) },
            { Color.Base00,     Media.Color.FromRgb(0x65, 0x7b, 0x83) },
            { Color.Base0,      Media.Color.FromRgb(0x83, 0x94, 0x96) },
            { Color.Base1,      Media.Color.FromRgb(0x93, 0xa1, 0xa1) },
            { Color.Base2,      Media.Color.FromRgb(0xee, 0xe8, 0xd5) },
            { Color.Base3,      Media.Color.FromRgb(0xfd, 0xf6, 0xe3) },

            { Color.Red,        Media.Color.FromRgb(0xdc, 0x32, 0x2f) },
            { Color.Orange,     Media.Color.FromRgb(0xcb, 0x4b, 0x16) },
            { Color.Yellow,     Media.Color.FromRgb(0xb5, 0x89, 0x00) },
            { Color.Green,      Media.Color.FromRgb(0x85, 0x99, 0x00) },
            { Color.Cyan,       Media.Color.FromRgb(0x2a, 0xa1, 0x98) },
            { Color.Blue,       Media.Color.FromRgb(0x26, 0x8b, 0xd2) },
            { Color.Purple,     Media.Color.FromRgb(0x6c, 0x71, 0xc4) },
            { Color.Magenta,    Media.Color.FromRgb(0xd3, 0x36, 0x82) },

            { Color.AltRed,     Media.Color.FromRgb(0xdc, 0x32, 0x2f) },
            { Color.AltOrange,  Media.Color.FromRgb(0xcb, 0x4b, 0x16) },
            { Color.AltYellow,  Media.Color.FromRgb(0xb5, 0x89, 0x00) },
            { Color.AltGreen,   Media.Color.FromRgb(0x85, 0x99, 0x00) },
            { Color.AltCyan,    Media.Color.FromRgb(0x2a, 0xa1, 0x98) },
            { Color.AltBlue,    Media.Color.FromRgb(0x26, 0x8b, 0xd2) },
            { Color.AltPurple,  Media.Color.FromRgb(0x6c, 0x71, 0xc4) },
            { Color.AltMagenta, Media.Color.FromRgb(0xd3, 0x36, 0x82) },
        });

        //// By w0ng
        //public static readonly Theme Hybrid = new Theme(new Dictionary<Color, Media.Color>()
        //{
        //    { Color.None,       Colors.Transparent },
        //    { Color.Foreground, Media.Color.FromRgb(0xc5, 0xc8, 0xc6)},
        //    { Color.Background, Media.Color.FromRgb(0x1d, 0x1f, 0x21)},
        //    { Color.Black,      Media.Color.FromRgb(0x28, 0x2a, 0x2e)},
        //    { Color.DarkGrey,   Media.Color.FromRgb(0x37, 0x3b, 0x41)},
        //    { Color.LightGrey,  Media.Color.FromRgb(0x70, 0x78, 0x80)},
        //    { Color.White,      Media.Color.FromRgb(0xc5, 0xc8, 0xc6)},
        //    { Color.Red,        Media.Color.FromRgb(0xa5, 0x42, 0x42)},
        //    { Color.AltRed,     Media.Color.FromRgb(0xcc, 0x66, 0x66)},
        //    { Color.Magenta,    Media.Color.FromRgb(0x85, 0x67, 0x8f)},
        //    { Color.AltMagenta, Media.Color.FromRgb(0xb2, 0x94, 0xbb)},
        //    { Color.Blue,       Media.Color.FromRgb(0x5f, 0x81, 0x9d)},
        //    { Color.AltBlue,    Media.Color.FromRgb(0x81, 0xa2, 0xbe)},
        //    { Color.Cyan,       Media.Color.FromRgb(0x5e, 0x8d, 0x87)},
        //    { Color.AltCyan,    Media.Color.FromRgb(0x8a, 0xbe, 0xb7)},
        //    { Color.Green,      Media.Color.FromRgb(0x8c, 0x94, 0x40)},
        //    { Color.AltGreen,   Media.Color.FromRgb(0xb5, 0xbd, 0x68)},
        //    { Color.Yellow,     Media.Color.FromRgb(0xde, 0x93, 0x5f)},
        //    { Color.AltYellow,  Media.Color.FromRgb(0xf0, 0xc6, 0x74)},
        //});
    }
}
