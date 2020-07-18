using MooTUI.Core;
using MooTUI.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MooTUI.Text
{
    public class MultilineTextSpan : TextSpan
    {
        public HJustification Justification { get; set; }
        public int Width { get; private set; }

        private List<string> Lines { get; set; }

        public MultilineTextSpan(string text, int width, ColorPair c = new ColorPair(), 
            HJustification justification = HJustification.LEFT)
            : base(text, c)
        {
            Width = width;
            Justification = justification;
        }

        public static MultilineTextSpan FromString(string s, int width, 
            HJustification justification = HJustification.LEFT)
        {
            MultilineTextSpan span = new MultilineTextSpan("", width, justification: justification);

            span.ParseAppend(s);

            return span;
        }

        public override Visual Draw()
        {
            GenerateLines();

            Visual visual = new Visual(Width, Lines.Count);

            int i = 0;

            for (int row = 0; row < Lines.Count; row++)
            {
                string currentLine = Lines[row];

                int trimmedLength = currentLine.Trim().Length;
                int xOffset = Justification.GetOffset(trimmedLength, Width);

                for(int column = 0; column < currentLine.Length; column++)
                {
                    if (column + xOffset < Width)
                        visual[column + xOffset, row] = new Cell(Text[i], ColorInfo.GetColorsAtIndex(i));
                    i++;
                }
            }

            return visual;
        }

        private void GenerateLines()
        {
            List<string> lines = new List<string>();

            List<string> hardLineBreaks = GetHardLineBreaks();
            foreach(string s in hardLineBreaks)
            {
                lines.AddRange(GetSoftLineBreaks(s));
            }

            Lines = lines;
        }

        private List<string> GetHardLineBreaks()
        {
            List<string> lines = new List<string>();

            int i = 0;
            while (true)
            {
                int next = Text.IndexOf('\n', i + 1);

                if (next == -1)
                    break;

                lines.Add(Text.Substring(i, next - i));
                i = next;
            }

            lines.Add(Text.Substring(i));

            return lines;
        }

        private List<string> GetSoftLineBreaks(string s)
        {
            List<string> lines = new List<string>();

            char curr;
            int wordLength = 0;
            int lineLength = 0;
            for (int i = 0; i < s.Length; i++)
            {
                curr = s[i];

                if (char.IsWhiteSpace(curr))
                {
                    if (wordLength > 0)
                    {
                        lineLength += wordLength;
                        wordLength = 0;
                    }
                    lineLength++;
                }
                else
                {
                    wordLength++;
                    if (lineLength + wordLength > Width)
                    {
                        lines.Add(s.Substring(i - lineLength - wordLength + 1, lineLength));
                        if (lineLength == 0)
                            wordLength = 0;
                        else
                            lineLength = 0;
                    }
                }
            }

            lines.Add(s.Substring(Text.Length - lineLength - wordLength));

            return lines;
        }
    }
}
