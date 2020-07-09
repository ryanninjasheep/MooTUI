using MooTUI.Core;
using MooTUI.Layout;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Text
{
    public class TextSpanEnclosure
    {
        public string Left { get; private set; }
        public string Right { get; private set; }

        public ColorPair Colors { get; set; }

        public TextSpanEnclosure(string left, string right, ColorPair colors)
        {
            if (left.Length != right.Length)
                throw new ArgumentException("Left and Right must be same length");

            Left = left;
            Right = right;
            Colors = colors;
        }

        public Visual DrawEnclosure(TextSpan span)
        {
            Visual inner = span.Draw();

            Visual visual = new Visual(inner.Width + Left.Length + Right.Length, inner.Height);
            visual.Merge(inner, HJustification.CENTER, VJustification.CENTER);

            int vCenter = VJustification.CENTER.GetOffset(1, visual.Height);

            for (int i = 0; i < Left.Length; i++)
            {
                visual[i, vCenter] = new Cell(Left[i], Colors);
                visual[i + Left.Length + inner.Width, vCenter] = new Cell(Right[i], Colors);
            }

            return visual;
        }
    }
}
