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

        public static readonly Style Default = new Style(new Dictionary<string, ColorScheme>()
        {
            { "Default",   new ColorScheme(Color.Foreground, Color.Background)   },
        });
        public static readonly Style SimpleLight = new Style(new Dictionary<string, ColorScheme>()
        {
            { "Default",   new ColorScheme(Color.Foreground, Color.Background)   },
            { "Disabled",  new ColorScheme(Color.DarkGrey,   Color.Background)   },
            { "Hover",     new ColorScheme(Color.Yellow,     Color.Background)   },
            { "Active",    new ColorScheme(Color.Foreground, Color.AltYellow)    },
            { "Cursor",    new ColorScheme(Color.Background, Color.Foreground)   },
            { "Selection", new ColorScheme(Color.Foreground, Color.Blue)         },
            { "Message",   new ColorScheme(Color.Foreground, Color.LightGrey)    },
            { "Error",     new ColorScheme(Color.Foreground, Color.Red)          },
            { "Warning",   new ColorScheme(Color.Foreground, Color.Yellow)       },
            { "Info",      new ColorScheme(Color.Foreground, Color.Cyan)         },
        });
    }

    public struct ColorScheme
    {
        public Color Fore { get; private set; }
        public Color Back { get; private set; }

        public ColorScheme(Color fore, Color back)
        {
            Fore = fore;
            Back = back;
        }

        public ColorScheme Invert() => new ColorScheme(Back, Fore);

        public static ColorScheme Null => new ColorScheme(Color.None, Color.None);
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
