using MooTUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MooTUI.Text
{
    public abstract class TextSpan
    {
        public string Text { get; protected set; }
        public ColorInfo ColorInfo { get; protected set; }

        public TextSpan(string text = "", ColorPair c = new ColorPair())
        {
            ColorInfo = new ColorInfo(c);
            Text = text;
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
        }

        public SingleLineTextSpan SubSpan(int start, int length)
        {
            if (start < 0 || length < 0 || start + length > Text.Length)
                throw new ArgumentOutOfRangeException();

            string substring = Text.Substring(start, length);
            ColorPair startingColors = ColorInfo.GetCurrentColorsAtIndex(start);
            SingleLineTextSpan span = new SingleLineTextSpan(substring, startingColors);

            foreach (int k in ColorInfo.Keys)
            {
                if (k > start && k < start + length)
                    span.SetColorInfo(k - start, ColorInfo[k]);
            }

            return span;
        }

        public abstract Visual Draw();
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
