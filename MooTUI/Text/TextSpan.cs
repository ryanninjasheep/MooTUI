using MooTUI.Core;
using MooTUI.IO;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

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

        public int Length => Text.Length;

        public Cell this[int index] => new Cell(Text[index], ColorInfo.GetColorsAtIndex(index));

        protected TextSpanColorInfo ColorInfo { get; set; }

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
            if (start < 0 || length < 0 || start + length > Length)
                throw new ArgumentOutOfRangeException();

            string substring = Text.Substring(start, length);
            TextSpan span = new TextSpan(substring);

            foreach ((int i, ColorPair c) in ColorInfo.Data)
            {
                if (i >= start && i < start + length)
                    span.SetColorInfo(i - start, c);
            }

            return span;
        }
        public TextSpan SubSpan(int start) => SubSpan(start, Length - start);

        public Visual Draw()
        {
            Visual visual = new Visual(Length, 1);

            for (int i = 0; i < Length; i++)
            {
                visual[i, 0] = this[i];
            }

            return visual;
        }

        public void SetColorInfo(int index, ColorPair colors)
        {
            if (index < 0 || index > Length)
                throw new ArgumentOutOfRangeException();

            else if (ColorInfo.GetColorsAtIndex(index) == colors)
                return;
            else
                ColorInfo.Add(index, colors);

            OnTextChanged(EventArgs.Empty);
        }

        public void Append(string text, ColorPair? colors = null)
        {
            if (colors is ColorPair c && ColorInfo.GetColorsAtIndex(Length) != c)
                ColorInfo.Add(Length, c);

            Text += text;
        }

        public void Insert(int index, string text, ColorPair? colors = null)
        {
            ColorInfo.Insert(index, text.Length, colors);

            Text = Text.Insert(index, text);
        }

        public void Delete(int index, int length)
        {
            ColorInfo.Delete(index, length);

            Text = Text.Remove(index, length);
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
            else
            {
                try
                {
                    return Widget.Style.GetColorPair(s);
                }
                catch
                {
                    return new ColorPair();
                }
            }
        }

        private void OnTextChanged(EventArgs e)
        {
            EventHandler handler = TextChanged;
            handler?.Invoke(this, e);
        }

        protected class TextSpanColorInfo
        {
            public List<(int index, ColorPair colors)> Data { get; private set; }

            public TextSpanColorInfo(ColorPair c)
            {
                Data = new List<(int index, ColorPair colors)>();
                Add(0, c);
            }

            public ColorPair GetColorsAtIndex(int index)
            {
                // Since the list is sorted, I could do more efficient stuff, but this works :D

                for (int i = 0; i < Data.Count - 1; i++)
                {
                    if (index >= Data[i].index && index < Data[i + 1].index)
                    {
                        return Data[i].colors;
                    }
                }

                return Data.Last().colors;
            }

            public void Add(int index, ColorPair c)
            {
                for (int i = 0; i < Data.Count - 1; i++)
                {
                    if (index == Data[i].index)
                    {
                        Data[i] = (Data[i].index, c);
                        return;
                    }
                    else if (index > Data[i].index && index < Data[i + 1].index)
                    {
                        Data.Insert(i + 1, (index, c));
                        return;
                    }
                }

                Data.Add((index, c));
            }

            public void Delete(int index, int length)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    if (Data[i].index >= index)
                    {
                        if (Data[i].index < index + length)
                        {
                            Data.RemoveAt(i);
                        }
                        else
                        {
                            Data[i] = (Data[i].index - length, Data[i].colors);
                        }
                    }
                }
            }

            public void Insert(int index, int length, ColorPair? colors = null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    if (Data[i].index >= index)
                        Data[i] = (Data[i].index + length, Data[i].colors);
                }

                if (colors is ColorPair c)
                {
                    if (GetColorsAtIndex(index) != c)
                    {
                        Add(index + length, GetColorsAtIndex(index));
                        Add(index, c);
                    }
                }
            }
        }
    }
}
