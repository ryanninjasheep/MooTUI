using MooTUI.Core;
using MooTUI.Layout;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Text
{
    public class MultilineTextSpan : TextSpan
    {
        public HJustification Justification { get; set; }
        public int Width { get; private set; }

        private List<TextSpan> Lines { get; set; }

        public MultilineTextSpan(string text = "", ColorPair c = new ColorPair(), 
            HJustification justification = HJustification.LEFT)
            : base(text, c)
        {
            Lines = new List<TextSpan>();
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
            List<TextSpan> lines = new List<TextSpan>();

            char curr;
            int wordLength = 0;
            int lineLength = 0;
            for (int i = 0; i < Text.Length - 1; i++)
            {
                curr = Text[i];

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
                        lines.Add(SubSpan(i - lineLength, lineLength));
                        if (lineLength == 0)
                            wordLength = 0;
                        else
                            lineLength = 0;
                    }
                }
            }

            Lines = lines;
        }
    }
}
