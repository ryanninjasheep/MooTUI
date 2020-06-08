using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Markup;

namespace MooTUI.IO
{
    public class SelectableSpan : Span
    {
        public bool CanSelect { get; set; }

        public int SelectionStart { get; private set; }
        public int SelectionExtent { get; private set; }

        public bool IsSelectionActive { get => SelectionExtent != 0; }
        public int SelectionMin
        {
            get => Math.Min(SelectionStart, SelectionStart + SelectionExtent);
        }
        public int SelectionMax
        {
            get => Math.Max(SelectionStart, SelectionStart + SelectionExtent);
        }

        public SelectableSpan(string text, int width, HJustification hJustification, VJustification vJustification,
            bool canSelect) : base(text, width, hJustification, vJustification)
        {
            CanSelect = canSelect;
        }
        public SelectableSpan(string text, int width, HJustification hJustification, VJustification vJustification)
            : this(text, width, hJustification, vJustification, true) { }
        public SelectableSpan(string text, int width)
            : this(text, width, HJustification.LEFT, VJustification.TOP) { }

        #region SELECTION

        public void SetSelection(int index)
        {
            if (index < 0)
            {
                SetSelection(0);
            }
            else if (index > Length)
            {
                SetSelection(Length);
            }
            else
            {
                SelectionStart = index;
                SelectionExtent = 0;
            }
        }
        public void MoveSelection(int delta)
        {
            if (!CanSelect)
                return;

            SelectionExtent += delta;
        }

        public List<(int x, int y)> GetSelectedCells()
        {
            if (!IsSelectionActive)
                return null;

            List<(int x, int y)> points = new List<(int x, int y)>();

            int cursorX;
            int cursorY;
            (cursorX, cursorY) = GetCellAtIndex(SelectionMin);

            for (int i = SelectionMin; i < SelectionMax; i++)
            {
                if (cursorX <= DisplayText[cursorY].Length)
                    points.Add((cursorX, cursorY));

                cursorX++;

                if (cursorY < BreakPoints.Count && i + 1 >= BreakPoints[cursorY])
                {
                    cursorY++;
                    cursorX = 0;
                }
            }

            //if (GetCellAtIndex(SelectionMax) != (cursorX, cursorY))
            //    throw new Exception();

            return points;
        }

        public string GetSelection()
        {
            if (!IsSelectionActive)
                return "";

            return Text.Substring(SelectionMin, SelectionMax - SelectionMin);
        }

        #endregion
    }
}
