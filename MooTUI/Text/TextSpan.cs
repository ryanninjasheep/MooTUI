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

        public event EventHandler TextChanged;

        public TextSpan SubSpan(int start, int length)
        {
            if (start < 0 || length < 0 || start + length > Text.Length)
                throw new ArgumentOutOfRangeException();

            string substring = Text.Substring(start, length);
            ColorPair startingColors = ColorInfo.GetCurrentColorsAtIndex(start);
            TextSpan span = new TextSpan(substring, startingColors);

            foreach (int k in ColorInfo.Keys)
            {
                if (k > start && k < start + length)
                    span.SetColorInfo(k - start, ColorInfo[k]);
            }

            return span;
        }

        public void SetColorInfo(int index, ColorPair colors)
        {
            if (index < 0 || index > Text.Length)
                throw new ArgumentOutOfRangeException();

            if (ColorInfo.ContainsKey(index))
                ColorInfo[index] = colors;
            else if (ColorInfo.GetCurrentColorsAtIndex(index) == colors)
                return;
            else
                ColorInfo.Add(index, colors);

            OnTextChanged(EventArgs.Empty);
        }

        public void Append(string text, ColorPair colors)
        {
            if (ColorInfo[text.Length] != colors)
                ColorInfo.Add(text.Length, colors);

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
                visual[i, 0] = new Cell(Text[i], ColorInfo.GetCurrentColorsAtIndex(i));
            }

            return visual;
        }

        private void OnTextChanged(EventArgs e)
        {
            EventHandler handler = TextChanged;
            handler?.Invoke(this, e);
        }

        public class TextSpanColorInfo : SortedList<int, ColorPair>
        {
            public TextSpanColorInfo(ColorPair c)
            {
                Add(0, c);
            }

            public ColorPair GetCurrentColorsAtIndex(int index)
            {
                int keyIndex = Keys.ToList().BinarySearch(index);

                if (keyIndex < 0)
                    keyIndex = ~keyIndex - 1;

                return this[Keys[keyIndex]];
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
