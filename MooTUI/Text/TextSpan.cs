using MooTUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MooTUI.Text
{
    public class TextSpan
    {
        private string text;

        public string Text
        {
            get => text;
            protected set
            {
                OnTextChanged(EventArgs.Empty);
                text = value;
            }
        }
        public TextSpanColorInfo ColorInfo { get; protected set; }

        public TextSpan(string text = "", ColorPair c = new ColorPair())
        {
            ColorInfo = new TextSpanColorInfo(c);
            Text = text;
        }

        public static TextSpan FromString(string s)
        {
            TextSpan span = new TextSpan();

            span.ParseAppend(s);

            return span;
        }

        public event EventHandler TextChanged;

        public TextSpan SubSpan(int start, int length)
        {
            if (start < 0 || length < 0 || start + length > Text.Length)
                throw new ArgumentOutOfRangeException();

            string substring = Text.Substring(start, length);
            TextSpan span = new TextSpan(substring);

            // TODO: FIX
            //foreach (int k in ColorInfo.Keys)
            //{
            //    if (k > start && k < start + length)
            //        span.SetColorInfo(k - start, ColorInfo.GetColorsAtIndex(k));
            //}

            return span;
        }

        public void SetColorInfo(int index, ColorPair colors)
        {
            if (index < 0 || index > Text.Length)
                throw new ArgumentOutOfRangeException();

            else if (ColorInfo.GetColorsAtIndex(index) == colors)
                return;
            else
                ColorInfo.Add(index, colors);

            OnTextChanged(EventArgs.Empty);
        }

        public void Append(string text, ColorPair colors)
        {
            if (ColorInfo.GetColorsAtIndex(Text.Length) != colors)
                ColorInfo.Add(Text.Length, colors);

            Append(text);
        }
        public void Append(string text)
        {
            Text += text;
        }

        public virtual Visual Draw()
        {
            // This could be optimized probably

            Visual visual = new Visual(Math.Max(Text.Length, 1), 1);

            for (int i = 0; i < Text.Length; i++)
            {
                visual[i, 0] = new Cell(Text[i], ColorInfo.GetColorsAtIndex(i));
            }

            return visual;
        }

        protected void ParseAppend(string s)
        {
            ColorPair colors = new ColorPair();
            string text = "";
            foreach (char c in s)
            {
                if (c == '{')
                {
                    Append(text, colors);
                    text = "";
                }
                else if (c == '}')
                {
                    colors = ParseColorArgument(text);
                    text = "";
                }
                else
                {
                    text += c;
                }
            }

            Append(text, colors);
        }

        /// <summary>
        /// First, ensures argument is properly formatted, then returns the appropriate ColorPair.  If the argument
        /// is not formatted properly, returns the empty ColorPair.
        /// </summary>
        private static ColorPair ParseColorArgument(string s)
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
            return new ColorPair();
        }

        private void OnTextChanged(EventArgs e)
        {
            EventHandler handler = TextChanged;
            handler?.Invoke(this, e);
        }

        public class TextSpanColorInfo
        {
            private List<(int index, ColorPair colors)> _data;

            public TextSpanColorInfo(ColorPair c)
            {
                _data = new List<(int index, ColorPair colors)>();
                Add(0, c);
            }

            public ColorPair GetColorsAtIndex(int index)
            {
                // Since the list is sorted, I could do more efficient stuff, but this works :D

                for (int i = 0; i < _data.Count - 1; i++)
                {
                    if (index >= _data[i].index && index < _data[i + 1].index)
                    {
                        return _data[i].colors;
                    }
                }

                return _data.Last().colors;
            }

            public void Add(int index, ColorPair c)
            {
                for (int i = 0; i < _data.Count - 1; i++)
                {
                    if (index == _data[i].index)
                    {
                        _data[i] = (_data[i].index, c);
                        return;
                    }
                    else if (index > _data[i].index && index < _data[i + 1].index)
                    {
                        _data.Insert(i + 1, (index, c));
                        return;
                    }
                }

                _data.Add((index, c));
            }
        }
    }

    public static class VisualTextSpanExtensions
    {
        public static void DrawSpan(this Visual v, TextSpan span)
        {
            Visual spanVisual = span.Draw();
            v.Merge(spanVisual);
        }
    }
}
