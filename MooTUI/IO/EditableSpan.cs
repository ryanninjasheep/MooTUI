using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.IO
{
    public class EditableSpan : SelectableSpan
    {
        public bool CanEdit { get; set; }

        public int Cursor { get; private set; }

        public EditableSpan(string text, int width, HJustification hJustification, VJustification vJustification,
            bool canSelect, bool canEdit)
            : base(text, width, hJustification, vJustification, canSelect)
        {
            CanEdit = canEdit;
        }
        public EditableSpan(string text, int width, HJustification hJustification, VJustification vJustification,
            bool canSelect)
            : this(text, width, hJustification, vJustification, canSelect, true) { }
        public EditableSpan(string text, int width, HJustification hJustification, VJustification vJustification)
            : this(text, width, hJustification, vJustification, true) { }
        public EditableSpan(string text, int width)
            : this(text, width, HJustification.LEFT, VJustification.TOP) { }

        #region CURSOR

        public void SetCursor(int location, bool selectionActive = false)
        {
            if (location < 0)
            {
                SetCursor(0, selectionActive);
            }
            else if (location > Length)
            {
                SetCursor(Length, selectionActive);
            }
            else
            {
                if (CanSelect && selectionActive)
                    MoveSelection(location - Cursor);
                else
                    SetSelection(location);

                Cursor = location;
            }
        }
        public void MoveCursor(int delta, bool selectionActive = false)
        {
            SetCursor(Cursor + delta, selectionActive);
        }

        public bool CursorUp(bool selectionActive = false)
        {
            if (Cursor == 0)
                return false;

            (int x, int y) = GetCellAtIndex(Cursor);
            SetCursor(GetIndexAtCell(x, y - 1), selectionActive);

            return true;
        }
        public bool CursorDown(bool selectionActive = false)
        {
            if (Cursor == Length)
                return false;

            (int x, int y) = GetCellAtIndex(Cursor);
            SetCursor(GetIndexAtCell(x, y + 1), selectionActive);

            return true;
        }
        public bool CursorLeft(bool selectionActive = false)
        {
            if (Cursor == 0)
                return false;

            if (!selectionActive && Cursor != SelectionMin)
                SetCursor(SelectionMin);
            else
                MoveCursor(-1, selectionActive);

            return true;
        }
        public bool CursorRight(bool selectionActive = false)
        {
            if (Cursor == Length)
                return false;

            if (!selectionActive && Cursor != SelectionMax)
                SetCursor(SelectionMax);
            else
                MoveCursor(1, selectionActive);

            return true;
        }

        public (int x, int y) GetCursorCell()
        {
            return GetCellAtIndex(Cursor);
        }

        #endregion

        #region EDITING

        public void Write(string s)
        {
            if (!CanEdit)
                return;

            DeleteSelection();

            SetText(Text.Substring(0, Cursor) + s + Text.Substring(Cursor));

            MoveCursor(s.Length);
        }
        public void Write(char c)
        {
            Write(c.ToString());
        }

        public void DeleteSelection()
        {
            if (CanEdit && IsSelectionActive)
            {
                SetText(Text.Substring(0, SelectionMin) + Text.Substring(SelectionMax));
                SetCursor(SelectionMin);
            }
        }

        public void Backspace()
        {
            if (!CanEdit)
                return;

            if (IsSelectionActive)
            {
                DeleteSelection();
            }
            else if (Cursor > 0)
            {
                SetText(Text.Substring(0, Cursor - 1) + Text.Substring(Cursor));
                MoveCursor(-1);
            }
        }

        #endregion
    }
}
