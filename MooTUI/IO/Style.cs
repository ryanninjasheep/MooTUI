using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MooTUI.Core;

namespace MooTUI.IO
{
    public class Style
    {
        private Dictionary<string, ColorPair> ColorSchemes { get; }
        private Style Overflow { get; }

        public Style(Dictionary<string, ColorPair> style, Style overflow = null)
        {
            ColorSchemes = style;
            Overflow = overflow ?? Dark.Value;
        }

        public ColorPair GetColorPair(string key)
        {
            if (ColorSchemes.ContainsKey(key))
                return ColorSchemes[key];
            else if (this != Overflow)
                return Overflow.GetColorPair(key);
            else
                throw new KeyNotFoundException("The given key was not present in this Style chain.");
        }
        public Color GetFore(string key) => GetColorPair(key).Fore;
        public Color GetBack(string key) => GetColorPair(key).Back;

        public static readonly Lazy<Style> Dark = new Lazy<Style>(
            () => new Style(new Dictionary<string, ColorPair>()
        {
            { "Default",   new ColorPair(Color.Base0,    Color.Base03)   },
            { "Disabled",  new ColorPair(Color.Base01,   Color.Base03)   },
            { "Hover",     new ColorPair(Color.Base0,    Color.Base02)   },
            { "Active",    new ColorPair(Color.Base0,    Color.Base02)   },
            { "Cursor",    new ColorPair(Color.Base03,   Color.Base0)    },
            { "Selection", new ColorPair(Color.Base03,   Color.Base01)   },
            { "Message",   new ColorPair(Color.Base0,    Color.Base02)   },
            { "Error",     new ColorPair(Color.Red,      Color.Base02)   },
            { "Warning",   new ColorPair(Color.Yellow,   Color.Base02)   },
            { "Info",      new ColorPair(Color.Cyan,     Color.Base02)   },
        }));
        public static readonly Lazy<Style> Light = new Lazy<Style>(
            () => new Style(new Dictionary<string, ColorPair>()
        {
            { "Default",   new ColorPair(Color.Base00,   Color.Base3)    },
            { "Disabled",  new ColorPair(Color.Base1,    Color.Base3)    },
            { "Hover",     new ColorPair(Color.Base00,   Color.Base2)    },
            { "Active",    new ColorPair(Color.Base00,   Color.Base2)    },
            { "Cursor",    new ColorPair(Color.Base3,    Color.Base00)   },
            { "Selection", new ColorPair(Color.Base3,    Color.Base1)    },
            { "Message",   new ColorPair(Color.Base3,    Color.Base1)    },
            { "Error",     new ColorPair(Color.Base3,    Color.Red)      },
            { "Warning",   new ColorPair(Color.Base3,    Color.Yellow)   },
            { "Info",      new ColorPair(Color.Base3,    Color.Cyan)     },
        }));
        public static readonly Lazy<Style> HighContrast = new Lazy<Style>(
            () => new Style(new Dictionary<string, ColorPair>()
        {
            { "Default",   new ColorPair(Color.Base3,    Color.Base03)   },
            { "Disabled",  new ColorPair(Color.Base2,    Color.Base03)   },
            { "Hover",     new ColorPair(Color.Base3,    Color.Base02)   },
            { "Active",    new ColorPair(Color.Base3,    Color.Base02)   },
            { "Cursor",    new ColorPair(Color.Base03,   Color.Base3)    },
            { "Selection", new ColorPair(Color.Base03,   Color.Base2)    },
            { "Message",   new ColorPair(Color.Base03,   Color.Base0)    },
            { "Error",     new ColorPair(Color.Base03,   Color.Red)      },
            { "Warning",   new ColorPair(Color.Base03,   Color.Yellow)   },
            { "Info",      new ColorPair(Color.Base03,   Color.Cyan)     },
        }));
    }
}
