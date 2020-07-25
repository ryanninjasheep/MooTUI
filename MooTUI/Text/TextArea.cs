using MooTUI.Core;
using MooTUI.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace MooTUI.Text
{
    public class TextArea
    {
        public TextSpan Span { get; private set; }
        public string Text => Span.Text;

        public HJustification Justification { get; set; }
        public int Width { get; private set; }

        public List<TextSpan> Lines { get; private set; }

        public TextArea(string text, int width, ColorPair c = new ColorPair(),
            HJustification justification = HJustification.LEFT)
            : this(new TextSpan(text, c), width, justification) { }
        public TextArea(TextSpan text, int width, HJustification justification = HJustification.LEFT)
        {
            Span = text;
            Span.TextChanged += Span_TextChanged;

            Width = width;
            Justification = justification;
        }

        public static TextArea FromString(string s, int width, 
            HJustification justification = HJustification.LEFT) =>
            new TextArea(TextSpan.FromString(s), width, justification);

        public event EventHandler TextChanged;
        public event EventHandler HeightChanged;

        public Visual Draw()
        {
            GenerateLines();

            Visual visual = new Visual(Width, Lines.Count);

            for (int row = 0; row < Lines.Count; row++)
            {
                TextSpan currentLine = Lines[row];

                int trimmedLength = currentLine.Text.Trim().Length;
                int xOffset = Justification.GetOffset(trimmedLength, Width);

                for(int column = 0; column < currentLine.Length; column++)
                {
                    if (column + xOffset < Width)
                        visual[column + xOffset, row] = currentLine[column];
                }
            }

            return visual;
        }

        public void Resize(int width)
        {
            Width = width;
            GenerateLines();
        }

        private void GenerateLines()
        {
            List<TextSpan> lines = new List<TextSpan>();

            List<TextSpan> hardLineBreaks = GetHardLineBreaks();
            foreach(TextSpan s in hardLineBreaks)
            {
                lines.AddRange(GetSoftLineBreaks(s));
            }

            bool overflow = Lines?.Count != lines.Count;

            Lines = lines;

            if (overflow)
                OnHeightChanged(EventArgs.Empty);
        }

        private List<TextSpan> GetHardLineBreaks()
        {
            List<TextSpan> lines = new List<TextSpan>();

            int i = 0;
            if (Span.Length > 0)
            {
                while (true)
                {
                    int next = Span.Text.IndexOf('\n', i);

                    if (next == -1)
                        break;

                    next++;

                    lines.Add(Span.SubSpan(i, next - i));
                    i = next;
                }
            }

            lines.Add(Span.SubSpan(i));

            return lines;
        }

        private List<TextSpan> GetSoftLineBreaks(TextSpan s)
        {
            List<TextSpan> lines = new List<TextSpan>();

            char curr;
            int wordLength = 0;
            int lineLength = 0;
            for (int i = 0; i < s.Length; i++)
            {
                curr = s[i].Char ?? ' ';

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
                    if (lineLength + wordLength >= Width)
                    {
                        lines.Add(s.SubSpan(i - lineLength - wordLength + 1, lineLength));
                        if (lineLength == 0)
                            wordLength = 0;
                        else
                            lineLength = 0;
                    }
                }
            }

            lines.Add(s.SubSpan(s.Length - lineLength - wordLength));

            return lines;
        }

        private void Span_TextChanged(object sender, EventArgs e)
        {
            EventHandler handler = TextChanged;
            handler?.Invoke(sender, e);
        }

        private void OnHeightChanged(EventArgs e)
        {
            EventHandler handler = HeightChanged;
            handler?.Invoke(this, e);
        }
    }

    public static class VisualTextAreaExtensions
    {
        public static void DrawTextArea(this Visual v, TextArea text)
        {
            Visual textVisual = text.Draw();
            v.Merge(textVisual);
        }
    }
}
