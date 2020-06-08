using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MooTUI.IO
{
    public class Span
    {
        public static readonly char[] NewLineChars = { '\r', '\n' };
        public static readonly char[] WhiteSpaceChars = { ' ', '\t' };
        public static readonly char[] NonDisplayChars = { '\r', '\n', '\t' };

        public string Text { get; private set; }
        public List<int> BreakPoints { get; private set; }
        public List<string> DisplayText { get; private set; }

        public int Length { get => Text.Length; }

        public int Width { get; private set; }
        public int Height { get => DisplayText.Count; }

        public HJustification HJustification { get; private set; }
        public VJustification VJustification { get; private set; }

        public Span(string text, int width, HJustification hJustification, VJustification vJustification)
        {
            Width = width;

            HJustification = hJustification;
            VJustification = vJustification;

            Text = text;

            RefreshDisplay();
        }
        public Span(string text, int width)
            : this(text, width, HJustification.LEFT, VJustification.TOP) { }

        #region TEXT POPULATION

        public void SetText(string text)
        {
            Text = text;

            RefreshDisplay();
        }

        public void SetDisplayWidth(int width)
        {
            Width = width;

            RefreshDisplay();
        }

        public void RefreshDisplay()
        {
            GetBreakPoints();
            GetDisplayText();
        }

        private void GetBreakPoints()
        {
            List<int> breakPoints = new List<int>();

            int total = 0;
            int currentLine = 0;
            int currentWord = 0;
            foreach (char c in Text)
            {
                total++;

                if (NewLineChars.Contains(c))
                {
                    currentWord = 0;
                    currentLine = 0;

                    breakPoints.Add(total);
                }
                else if (WhiteSpaceChars.Contains(c))
                {
                    currentLine++;

                    if (currentWord > 0)
                    {
                        currentLine += currentWord;
                        currentWord = 0;
                    }
                }
                else
                {
                    currentWord++;

                    if (currentLine + currentWord >= Width)
                    {
                        if (currentWord >= Width)
                            currentWord = 0;

                        currentLine = 0;
                        breakPoints.Add(total - currentWord);
                    }
                }
            }

            BreakPoints = breakPoints;
        }
        private void GetDisplayText()
        {
            List<string> lines = new List<string>();

            int total = 0;
            foreach (int i in BreakPoints)
            {
                string line = Text.Substring(total, i - total);

                line = ProcessLine(line);

                lines.Add(line);

                total = i;
            }

            lines.Add(ProcessLine(Text.Substring(total, Length - total)));

            DisplayText = lines;
        }

        public string ProcessLine(string toProcess)
        {
            string line = toProcess;

            line = line.TrimEnd(WhiteSpaceChars);

            line = new string((from c in line
                               where !NonDisplayChars.Contains(c)
                               select c).ToArray());

            return line;
        }

        #endregion

        #region HELPER FUNCTIONS

        public (int x, int y) GetCellAtIndex(int index)
        {
            int cursorX = index;
            int cursorY = 0;
            while (cursorY < BreakPoints.Count && cursorX >= BreakPoints[cursorY])
            {
                cursorY++;
            }

            if (cursorY > 0)
                cursorX -= BreakPoints[cursorY - 1];

            cursorX = Math.Min(cursorX, Width - 1);

            return (cursorX, cursorY);
        }
        public int GetIndexAtCell(int x, int y)
        {
            if (y < 0)
                return 0;

            if (y > BreakPoints.Count || x >= Width)
                return Text.Length;

            int index = 0;
            if (y > 0)
                index += BreakPoints[y - 1];

            index += Math.Min(x, DisplayText[y].Length);

            return index;
        }

        #endregion
    }
}
