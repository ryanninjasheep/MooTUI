using MooTUI.Core;
using MooTUI.IO;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
                text = value;
                OnTextChanged(EventArgs.Empty);
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

        /// <summary>
        /// Attempts to color a given string using ColorPair parsing of arguments in brackets.
        /// </summary>
        public static TextSpan Parse(string s, Style style = null)
        {
            TextSpan span = new TextSpan();

            span.ParseAppend(s, style);

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

        protected void ParseAppend(string s, Style style)
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
                    colors = ColorPair.Parse(text, style);
                    text = "";
                }
                else
                {
                    text += c;
                }
            }

            Append(text, colors);
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

    public static class VisualTextSpanExtensions
    {
        public static void DrawTextSpan(this Visual v, TextSpan text, int x = 0, int y = 0)
        {
            Visual textVisual = text.Draw();
            v.Merge(textVisual, x, y);
        }
    }
}
