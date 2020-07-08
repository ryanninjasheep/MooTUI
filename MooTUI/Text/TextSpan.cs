using MooTUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MooTUI.Text
{
    /// <summary>
    /// Contains a single line of enriched text -- that is, text with possible color data.
    /// </summary>
    public class TextSpan
    {
        public string Text { get; private set; }

        private ColorInfo Colors { get; set; }

        public TextSpan(string text = "", ColorPair c = new ColorPair())
        {
            Colors = new ColorInfo(c);
            Text = text;
        }

        public void Append(string text, ColorPair colors)
        {
            if (Colors[text.Length] != colors)
                Colors.Add(text.Length, colors);

            Text += text;
        }

        public void SetColors(int index, ColorPair colors)
        {
            if (index < 0 || index > Text.Length)
                throw new ArgumentOutOfRangeException();

            if (Colors.ContainsKey(index))
                Colors[index] = colors;
            else if (Colors.GetCurrentColorsAtIndex(index) == colors)
                return;
            else
                Colors.Add(index, colors);
        }

        public TextSpan SubSpan(int start, int length)
        {
            if (start < 0 || length < 0 || start + length > Text.Length)
                throw new ArgumentOutOfRangeException();

            string substring = Text.Substring(start, length);
            ColorPair startingColors = Colors.GetCurrentColorsAtIndex(start);
            TextSpan span = new TextSpan(substring, startingColors);

            foreach (int k in Colors.Keys)
            {
                if (k > start && k < start + length)
                    span.SetColors(k - start, Colors[k]);
            }

            return span;
        }

        public virtual Visual Draw()
        {
            // This could be optimized

            Visual visual = new Visual(Text.Length, 1);

            for (int i = 0; i < Text.Length; i++)
            {
                visual[i, 0] = new Cell(Text[i], Colors.GetCurrentColorsAtIndex(i));
            }

            return visual;
        }
    }

    public class ColorInfo : SortedList<int, ColorPair>
    {
        public ColorInfo(ColorPair c)
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
