using System.Data.Common;
using System.Text;

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
}
