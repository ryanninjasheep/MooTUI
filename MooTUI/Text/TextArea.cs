using MooTUI.Drawing;
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
        private List<TextSpan> lines;

        public TextSpan Span { get; private set; }
        public string Text => Span.Text;

        public HJustification Justification { get; set; }
        public int Width { get; private set; }
        public int Height => Lines.Count;

        public List<TextSpan> Lines 
        {
            get => lines; 
            private set
            {
                bool heightChanged = Lines?.Count != lines.Count;

                lines = value;

                if (heightChanged)
                    OnHeightChanged(EventArgs.Empty);
            }
        }

        public TextArea(string text, int width, ColorPair c = new ColorPair(),
            HJustification justification = HJustification.LEFT)
            : this(new TextSpan(text, c), width, justification) { }
        public TextArea(TextSpan text, int width, HJustification justification = HJustification.LEFT)
        {
            Span = text;
            Span.TextChanged += Span_TextChanged;

            Width = width;
            Justification = justification;

            lines = GenerateLines();
        }

        /// <summary>
        /// Attempts to color a given string using ColorPair parsing of arguments in brackets.
        /// </summary>
        public static TextArea Parse(string s, int width, Style? style = null,
            HJustification justification = HJustification.LEFT) =>
            new TextArea(TextSpan.Parse(s, style), width, justification);

        public event EventHandler? TextChanged;
        public event EventHandler? HeightChanged;

        public Visual Draw()
        {
            Visual visual = new Visual(Width, Lines.Count);

            for (int row = 0; row < Lines.Count; row++)
            {
                TextSpan currentLine = Lines[row];

                int trimmedLength = currentLine.Text.Trim().Length;
                int xOffset = Justification.GetOffset(trimmedLength, Width);

                for (int column = 0; column < currentLine.Length; column++)
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

        public void SetText(string text)
        {
            Span.Delete(0, Span.Length);
            Span.Append(text);

            GenerateLines();
        }

        private List<TextSpan> GenerateLines()
        {
            List<TextSpan> lines = new List<TextSpan>();

            List<TextSpan> hardLineBreaks = GetHardLineBreaks();
            foreach (TextSpan s in hardLineBreaks)
            {
                lines.AddRange(GetSoftLineBreaks(s));
            }

            return lines;
        }

        private List<TextSpan> GetHardLineBreaks()
        {
            List<TextSpan> lines = new List<TextSpan>();

            int i = 0;
            while (true)
            {
                int next = Span.Text.IndexOf('\n', i);

                if (next == -1)
                    break;

                next++;

                lines.Add(Span.SubSpan(i, next - i));
                i = next;
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
            Lines = GenerateLines();

            EventHandler? handler = TextChanged;
            handler?.Invoke(sender, e);
        }

        private void OnHeightChanged(EventArgs e)
        {
            EventHandler? handler = HeightChanged;
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
