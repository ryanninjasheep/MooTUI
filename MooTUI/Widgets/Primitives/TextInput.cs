using MooTUI.Drawing;
using MooTUI.Input;
using MooTUI.Layout;
using MooTUI.Control;
using MooTUI.Text;
using System;
using System.Windows;
using Sys = System.Windows.Input;

namespace MooTUI.Widgets.Primitives
{
    public abstract partial class TextInput : Widget
    {
        public TextInput(LayoutRect bounds, int? charLimit, string promptText = "") : base(bounds)
        {
            TextArea = new TextArea("", Width);
            Prompt = new TextArea(
                promptText, 
                Width, 
                new ColorPair(Style.GetFore("Disabled"), Color.None));

            CharLimit = charLimit;
        }

        protected override void Resize()
        {
            TextArea.Resize(Width);
        }

        protected override void Input(InputEventArgs e)
        {
            switch (e)
            {
                case FocusInputEventArgs _:
                    IsFocused = true;
                    if (Cursor == -1)
                    {
                        SetCursorCoords(0, 0);
                        MoveCursor(Text.Length, 0, true);
                    }
                    Render();
                    break;
                case UnfocusInputEventArgs _:
                    IsFocused = false;
                    RemoveCursor();
                    Render();
                    break;
                case MouseEnterInputEventArgs _:
                    IsHovered = true;
                    Render();
                    break;
                case MouseLeaveInputEventArgs _:
                    IsHovered = false;
                    Render();
                    break;
                case MouseClickInputEventArgs c:
                    OnClick(c);
                    break;
                case KeyboardInputEventArgs k:
                    OnKeyDown(k);
                    if (Cursor != -1)
                    {
                        (int x, int y) = GetCursorCoords();
                        if (x > 0)
                            EnsureRegionVisible(x - 1, y, 2, 1);
                        else
                            EnsureRegionVisible(x, y);
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void RefreshVisual()
        {
            Draw();
        }

        protected override void Draw()
        {
            if (IsFocused)
            {
                Visual.FillCell(new Cell(' ', Style.GetColorPair("Active")));

                Visual.DrawTextArea(TextArea);
                DrawSelection();
                DrawCursor();
            }
            else if (IsHovered)
            {
                Visual.FillCell(new Cell(' ', Style.GetColorPair("Hover")));

                if (Text.Length == 0)
                    Visual.DrawTextArea(Prompt);
                else
                    Visual.DrawTextArea(TextArea);
            }
            else
            {
                Visual.FillCell(new Cell(' ', Style.GetColorPair("Default")));

                if (Text.Length == 0)
                    Visual.DrawTextArea(Prompt);
                else
                    Visual.DrawTextArea(TextArea);
            }
        }

        protected abstract bool HandleEnter();

        private void DrawSelection()
        {
            if (!IsSelectionActive)
                return;

            (int startX, int startY) = GetCoordsAtIndex(SelectionStart);
            (int endX, int endY) = GetCoordsAtIndex(SelectionEnd);

            if (startY == endY)
            {
                Visual.FillColors(Style.GetColorPair("Selection"), startX, startY, endX - startX, 1);
            }
            else
            {
                Visual.FillColors(Style.GetColorPair("Selection"), startX, startY, Width - startX, 1);
                for (int i = startY + 1; i < endY; i++)
                {
                    Visual.FillColors(Style.GetColorPair("Selection"), 0, i, Width, 1);
                }
                Visual.FillColors(Style.GetColorPair("Selection"), 0, endY, endX, 1);
            }
        }
        private void DrawCursor()
        {
            (int x, int y) = GetCursorCoords();

            if (x > -1 && x < Width && y > -1 && y < Height)
                Visual.SetColors(x, y, Style.GetColorPair("Cursor"));
        }


        private void OnClick(MouseClickInputEventArgs c)
        {
            OnBubbleEvent(new ClaimFocusEventArgs(this));

            SetCursorCoords(c.Location.X, c.Location.Y, c.Shift);

            c.Handled = true;
        }

        private void OnKeyDown(KeyboardInputEventArgs k)
        {
            switch (k.Key)
            {
                case Sys.Key.Enter:
                    k.Handled = HandleEnter();
                    return;
                case Sys.Key.Back:
                    if (IsSelectionActive)
                    {
                        DeleteSelection();
                    }
                    else if (Cursor > 0)
                    {
                        MoveCursor(-1, 0);
                        TextArea.Span.Delete(Cursor, 1);
                        Render();
                    }
                    else
                    {
                        return;
                    }
                    OnTextChanged(EventArgs.Empty);
                    k.Handled = true;
                    return;
                case Sys.Key.Up:
                    k.Handled = MoveCursor(0, -1, k.Shift);
                    return;
                case Sys.Key.Down:
                    k.Handled = MoveCursor(0, 1, k.Shift);
                    return;
                case Sys.Key.Left:
                    if (IsSelectionActive && !k.Shift && Cursor != SelectionStart)
                    {
                        Cursor = SelectionStart;
                        ClearSelection();
                        Render();
                        k.Handled = true;
                    }
                    else
                    {
                        k.Handled = MoveCursor(-1, 0, k.Shift);
                    }
                    return;
                case Sys.Key.Right:
                    if (IsSelectionActive && !k.Shift && Cursor != SelectionEnd)
                    {
                        Cursor = SelectionEnd;
                        ClearSelection();
                        Render();
                        k.Handled = true;
                    }
                    else
                    {
                        k.Handled = MoveCursor(1, 0, k.Shift);
                    }
                    return;
                case Sys.Key.C:
                    if (k.Ctrl)
                    {
                        Clipboard.SetText(GetSelectedText());

                        Render();
                        k.Handled = true;
                        return;
                    }
                    break;
                case Sys.Key.X:
                    if (k.Ctrl)
                    {
                        Clipboard.SetText(GetSelectedText());
                        DeleteSelection();

                        Render();
                        k.Handled = true;
                        return;
                    }
                    break;
                case Sys.Key.V:
                    if (k.Ctrl)
                    {
                        Write(Clipboard.GetText());

                        Render();
                        k.Handled = true;
                        return;
                    }
                    break;
            }

            if (k.Char is char c)
            {
                Write(c.ToString());
                k.Handled = true;
                return;
            }
        }
    }

    public abstract partial class TextInput
    {
        private TextArea _text;

        public string Text => TextArea.Text;
        public int Cursor { get; private set; }

        public int SelectionStart => Math.Min(SelectionFrom, SelectionTo);
        public int SelectionEnd => Math.Max(SelectionFrom, SelectionTo);
        public bool IsSelectionActive => SelectionFrom != SelectionTo;

        protected TextArea TextArea
        {
            get => _text;
            private set
            {
                _text = value;
                OnTextChanged(EventArgs.Empty);
            }
        }

        private int SelectionFrom { get; set; }
        private int SelectionTo { get; set; }

        private bool IsFocused { get; set; }
        private bool IsHovered { get; set; }

        private int? CharLimit { get; }

        private TextArea Prompt { get; set; }

        public event EventHandler TextChanged;

        public (int x, int y) GetCursorCoords() => GetCoordsAtIndex(Cursor);

        public string GetSelectedText() => Text.Substring(SelectionStart, SelectionEnd - SelectionStart);

        public void ClearSelection()
        {
            SelectionFrom = Cursor;
            SelectionTo = Cursor;
        }

        public void Write(string s)
        {
            if (Cursor == -1)
                return;

            if (IsSelectionActive)
                DeleteSelection();

            if (CharLimit is int limit && TextArea.Text.Length + s.Length > limit)
                s = s.Substring(0, limit - TextArea.Text.Length);

            TextArea.Span.Insert(Cursor, s);
            MoveCursor(s.Length, 0);

            OnTextChanged(EventArgs.Empty);
        }

        public void SetText(string s)
        {
            TextArea.SetText(s);
            RemoveCursor();
            Render();
        }

        private void RemoveCursor()
        {
            Cursor = -1;
            ClearSelection();
        }
        private void DefaultCursor()
        {
            Cursor = 0;
            ClearSelection();
        }

        private bool MoveCursor(int deltaX, int deltaY, bool selectionActive = false)
        {
            if (Cursor == -1
                || (deltaX < 0 && Cursor == 0)
                || (deltaX > 0 && Cursor == Text.Length))
                return false;

            (int x, int y) = GetCursorCoords();

            if ((deltaY < 0 && y == 0)
                || (deltaY > 0 && y == TextArea.Lines.Count))
                return false;

            SetCursorCoords(x + deltaX, y + deltaY, selectionActive);
            return true;
        }

        private void SetCursorCoords(int x, int y, bool selectionActive = false)
        {
            y = Math.Min(y, TextArea.Lines.Count - 1);
            int cursor = x;
            for (int i = 0; i < y; i++)
            {
                cursor += TextArea.Lines[i].Length;
            }
            Cursor = Math.Min(cursor, Text.Length);
            if (Cursor == -1)
            {
                SelectionFrom = 0;
                SelectionTo = 0;
            }
            else
            {
                if (!selectionActive)
                    SelectionFrom = Cursor;
                SelectionTo = Cursor;
            }

            Render();
        }

        private void DeleteSelection()
        {
            TextArea.Span.Delete(SelectionStart, SelectionEnd - SelectionStart);
            Cursor = SelectionStart;
            Render();
            OnTextChanged(EventArgs.Empty);
            ClearSelection();
        }

        private (int x, int y) GetCoordsAtIndex(int index)
        {
            int x = index;
            int y = 0;

            while (y < TextArea.Lines.Count - 1 && x >= TextArea.Lines[y].Length)
            {
                x -= TextArea.Lines[y].Length;
                y++;
            }

            x = Math.Min(x, Width - 1);

            return (x, y);
        }

        private void OnTextChanged(EventArgs e)
        {
            EventHandler handler = TextChanged;
            handler?.Invoke(this, e);
        }
    }
}
