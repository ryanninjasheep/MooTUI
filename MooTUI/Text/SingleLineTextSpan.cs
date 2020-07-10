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
    public class SingleLineTextSpan : TextSpan
    {
        public SingleLineTextSpan(string text = "", ColorPair c = new ColorPair())
            : base(text, c) { }

        public void Append(string text, ColorPair colors)
        {
            if (ColorInfo[text.Length] != colors)
                ColorInfo.Add(text.Length, colors);

            Text += text;
        }

        public override Visual Draw()
        {
            // This could be optimized

            Visual visual = new Visual(Math.Max(Text.Length, 1), 1);

            for (int i = 0; i < Text.Length; i++)
            {
                visual[i, 0] = new Cell(Text[i], ColorInfo.GetCurrentColorsAtIndex(i));
            }

            return visual;
        }
    }
}
