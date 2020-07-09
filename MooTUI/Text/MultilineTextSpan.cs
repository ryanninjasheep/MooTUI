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

        private List<SingleLineTextSpan> Lines { get; set; }

        public MultilineTextSpan(string text = "", ColorPair c = new ColorPair(), 
            HJustification justification = HJustification.LEFT)
            : base(text, c)
        {
            Lines = new List<SingleLineTextSpan>();
            Justification = justification;
        }

        public override Visual Draw()
        {
            GenerateLines();

            Visual visual = new Visual(Width, Lines.Count);

            for (int i = 0; i < Lines.Count; i++)
            {
                Visual line = Lines[i].Draw();

                visual.Merge(line, Justification.GetOffset(line.Width, Width), i);
            }

            return visual;
        }

        private void GenerateLines()
        {
            List<SingleLineTextSpan> lines = new List<SingleLineTextSpan>();

            List<SingleLineTextSpan> hardLineBreaks = GetHardLineBreaks();
            foreach(SingleLineTextSpan s in hardLineBreaks)
            {
                lines.AddRange(GetSoftLineBreaks(s));
            }

            Lines = lines;
        }

        private List<SingleLineTextSpan> GetHardLineBreaks()
        {
            List<SingleLineTextSpan> lines = new List<SingleLineTextSpan>();

            int i = 0;
            while (true)
            {
                int next = Text.IndexOf('\n', i + 1);

                if (next == -1)
                    break;

                lines.Add(SubSpan(i, next - i));
                i = next;
            }

            lines.Add(SubSpan(i, Text.Length - i));

            return lines;
        }

        private List<SingleLineTextSpan> GetSoftLineBreaks(SingleLineTextSpan s)
        {
            List<SingleLineTextSpan> lines = new List<SingleLineTextSpan>();

            char curr;
            int wordLength = 0;
            int lineLength = 0;
            for (int i = 0; i < s.Text.Length - 1; i++)
            {
                curr = s.Text[i];

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
                        lines.Add(s.SubSpan(i - lineLength, lineLength));
                        if (lineLength == 0)
                            wordLength = 0;
                        else
                            lineLength = 0;
                    }
                }
            }

            lines.Add(s.SubSpan(Text.Length - lineLength, lineLength));

            return lines;
        }
    }
}
