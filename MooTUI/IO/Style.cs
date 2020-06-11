using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Media;

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
            { "Default",   new ColorScheme(Colors.Black,     Colors.White)       },
        });
        public static readonly Style SimpleLight = new Style(new Dictionary<string, ColorScheme>()
        {
            { "Default",   new ColorScheme(Colors.Black,     Colors.White)       },
            { "Disabled",  new ColorScheme(Colors.LightGray, Colors.White)       },
            { "Hover",     new ColorScheme(Colors.Black,     Colors.Khaki)       },
            { "Active",    new ColorScheme(Colors.Black,     Colors.White)       },
            { "Cursor",    new ColorScheme(Colors.Black,     Colors.LightGray)   },
            { "Selection", new ColorScheme(Colors.Black,     Colors.LightBlue)   },
            { "Message",   new ColorScheme(Colors.Black,     Colors.LightGray)   },
            { "Error",     new ColorScheme(Colors.White,     Colors.Red)         },
            { "Warning",   new ColorScheme(Colors.Black,     Colors.Yellow)      },
            { "Info",      new ColorScheme(Colors.Black,     Colors.LightBlue)   },
        });
        public static readonly Style Test = new Style(new Dictionary<string, ColorScheme>()
        {
            { "Default",   new ColorScheme(Color.FromRgb(0x9b, 0x90, 0x81), Color.FromRgb(0x18, 0x1b, 0x20)) },
            { "Disabled",  new ColorScheme(Color.FromRgb(0x5f, 0x5f, 0x5f), Color.FromRgb(0x18, 0x1b, 0x20)) },
            { "Hover",     new ColorScheme(Color.FromRgb(0x9b, 0x90, 0x81), Color.FromRgb(0x35, 0x35, 0x35)) },
            { "Active",    new ColorScheme(Color.FromRgb(0xcd, 0xcd, 0xcd), Color.FromRgb(0x35, 0x35, 0x35)) },
            { "Cursor",    new ColorScheme(Color.FromRgb(0xcd, 0xcd, 0xcd), Color.FromRgb(0x5f, 0x5f, 0x5f)) },
            { "Selection", new ColorScheme(Color.FromRgb(0xcd, 0xcd, 0xcd), Color.FromRgb(0x43, 0x58, 0x61)) },
            { "Message",   new ColorScheme(Color.FromRgb(0x35, 0x35, 0x35), Color.FromRgb(0x18, 0x1b, 0x20)) },
            { "Error",     new ColorScheme(Color.FromRgb(0x6d, 0x61, 0x37), Color.FromRgb(0x18, 0x1b, 0x20)) },
            { "Warning",   new ColorScheme(Color.FromRgb(0xcd, 0xcd, 0xcd), Color.FromRgb(0x18, 0x1b, 0x20)) },
            { "Info",      new ColorScheme(Color.FromRgb(0x43, 0x58, 0x61), Color.FromRgb(0x18, 0x1b, 0x20)) },
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

        public static ColorScheme Null => new ColorScheme(Colors.Transparent, Colors.Transparent);
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
