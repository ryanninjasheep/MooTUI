using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MooTUI.Core;

namespace MooTUI.IO
{
    public class Style
    {
        private Dictionary<string, ColorScheme> ColorSchemes { get; }

        public Style(Dictionary<string, ColorScheme> style)
        {
            ColorSchemes = style;
        }

        public ColorScheme GetColorScheme(ColorFamily key)
        {
            foreach(string s in key)
            {
                if (ColorSchemes.ContainsKey(s))
                    return GetColorScheme(s);
            }

            throw new ArgumentException("Style does not contain a ColorScheme for any values in the ColorFamily", "key");
        }
        public Color GetFore(ColorFamily key) => GetColorScheme(key).Fore;
        public Color GetBack(ColorFamily key) => GetColorScheme(key).Back;

        public ColorScheme GetColorScheme(string key) => ColorSchemes[key];
        public Color GetFore(string key) => GetColorScheme(key).Fore;
        public Color GetBack(string key) => GetColorScheme(key).Back;

        public static readonly Style Dark = new Style(new Dictionary<string, ColorScheme>()
        {
            { "Default",   new ColorScheme(Color.Base0,    Color.Base03)   },
            { "Disabled",  new ColorScheme(Color.Base01,   Color.Base03)   },
            { "Hover",     new ColorScheme(Color.Base0,    Color.Base02)   },
            { "Active",    new ColorScheme(Color.Base0,    Color.Base02)   },
            { "Cursor",    new ColorScheme(Color.Base03,   Color.Base0)    },
            { "Selection", new ColorScheme(Color.Base03,   Color.Base01)   },
            { "Message",   new ColorScheme(Color.Base0,    Color.Base02)   },
            { "Error",     new ColorScheme(Color.Red,      Color.Base02)   },
            { "Warning",   new ColorScheme(Color.Yellow,   Color.Base02)   },
            { "Info",      new ColorScheme(Color.Blue,     Color.Base02)   },
        });
        public static readonly Style Light = new Style(new Dictionary<string, ColorScheme>()
        {
            { "Default",   new ColorScheme(Color.Base00,   Color.Base3)    },
            { "Disabled",  new ColorScheme(Color.Base1,    Color.Base3)    },
            { "Hover",     new ColorScheme(Color.Base00,   Color.Base2)    },
            { "Active",    new ColorScheme(Color.Base00,   Color.Base2)    },
            { "Cursor",    new ColorScheme(Color.Base3,    Color.Base00)   },
            { "Selection", new ColorScheme(Color.Base3,    Color.Base1)    },
            { "Message",   new ColorScheme(Color.Base3,    Color.Base1)    },
            { "Error",     new ColorScheme(Color.Base3,    Color.Red)      },
            { "Warning",   new ColorScheme(Color.Base3,    Color.Yellow)   },
            { "Info",      new ColorScheme(Color.Base3,    Color.Blue)     },
        });
    }

    public struct ColorScheme
    {
        public Color Fore { get; private set; }
        public Color Back { get; private set; }

        public ColorScheme(Color fore = Color.None, Color back = Color.None)
        {
            Fore = fore;
            Back = back;
        }

        public ColorScheme Invert() => new ColorScheme(Back, Fore);
    }

    public class ColorFamily : IEnumerable<string>
    {
        public List<string> Keys { get; }

        public ColorFamily()
        {
            Keys = new List<string>();
        }

        public IEnumerator<string> GetEnumerator() => Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(string key)
        {
            Keys.Add(key);
        }
    }
}
