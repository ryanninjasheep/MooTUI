using MooTUI.Core;
using MooTUI.Input;
using MooTUI.IO.EventArgs;
using MooTUI.Layout;
using MooTUI.Text;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Sys = System.Windows.Input;

namespace MooTUI.Widgets
{
    public class TextInput : Widget
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

        // Only used if auto-resizing
        private int? MinSize { get; }

        private TextArea Prompt { get; set; }

        public TextInput(LayoutRect bounds, bool doesExpand = false, int? charLimit = null, 
            string promptText = "") : base(bounds)
        {
            TextArea = new TextArea("", Width);
            Prompt = new TextArea(
                promptText, 
                Width, 
                new ColorPair(Style.GetFore("Disabled"), Color.None));

            if (doesExpand)
            {
                if (Height == 1)
                {
                    MinSize = Width;
                }
                else
                {
                    TextArea.HeightChanged += TextArea_HeightChanged;
                    MinSize = Height;
                }
            }

            TextArea.TextChanged += TextArea_TextChanged;

            CharLimit = charLimit;
        }

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
                ClearSelection();

            if (CharLimit is int limit && TextArea.Text.Length + s.Length > limit)
                s = s.Substring(0, limit - TextArea.Text.Length);

            TextArea.Span.Insert(Cursor, s);
            MoveCursor(s.Length, 0);

            OnTextChanged(EventArgs.Empty);
        }

        public void SetText(string s)
        {
            Cursor = -1;
            TextArea.SetText(s);
            Render();
        }

        protected override void Resize()
        {
            TextArea.Resize(Width);
        }

        protected override void Input(InputEventArgs e)
        {
            switch (e.InputType)
            {
                case InputTypes.FOCUS:
                    IsFocused = true;
                    Render();
                    break;
                case InputTypes.UNFOCUS:
                    IsFocused = false;
                    Render();
                    break;
                case InputTypes.MOUSE_ENTER:
                    IsHovered = true;
                    Render();
                    break;
                case InputTypes.MOUSE_LEAVE:
                    IsHovered = false;
                    Render();
                    break;
                case InputTypes.LEFT_CLICK:
                    OnLeftClick(e);
                    break;
                case InputTypes.KEY_DOWN:
                    OnKeyDown(e);
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


        private void OnLeftClick(InputEventArgs e)
        {
            OnClaimFocus(new FocusEventArgs(this));

            SetCursorCoords(e.Mouse.Mouse.X, e.Mouse.Mouse.Y, e.Keyboard.Shift);

            e.Handled = true;
        }

        private void OnKeyDown(InputEventArgs e)
        {
            switch (e.Keyboard.LastKeyPressed)
            {
                case Sys.Key.Enter:
                    Write("\n");
                    e.Handled = true;
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
                    e.Handled = true;
                    return;
                case Sys.Key.Up:
                    e.Handled = MoveCursor(0, -1, e.Keyboard.Shift);
                    return;
                case Sys.Key.Down:
                    e.Handled = MoveCursor(0, 1, e.Keyboard.Shift);
                    return;
                case Sys.Key.Left:
                    e.Handled = MoveCursor(-1, 0, e.Keyboard.Shift);
                    return;
                case Sys.Key.Right:
                    e.Handled = MoveCursor(1, 0, e.Keyboard.Shift);
                    return;
            }

            if (e.Keyboard.GetCommand() != Command.NONE)
            {
                switch (e.Keyboard.GetCommand())
                {
                    case Command.COPY:
                        Clipboard.SetText(GetSelectedText());
                        break;
                    case Command.CUT:
                        Clipboard.SetText(GetSelectedText());
                        DeleteSelection();
                        break;
                    case Command.PASTE:
                        Write(Clipboard.GetText());
                        break;
                    default:
                        // ok
                        break;
                }

                Render();
                e.Handled = true;
                return;
            }

            if (e.Keyboard.KeyIsChar)
            {
                Write(e.Keyboard.GetCharInput().ToString());
                e.Handled = true;
                return;
            }
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

        private void TextArea_HeightChanged(object sender, EventArgs e)
        {
            if (Bounds.HeightData is FlexSize && MinSize is int min)
            {
                Bounds.SetSizes(
                    Bounds.WidthData,
                    new FlexSize(Math.Max(TextArea.Draw().Height + 1, min)));
            }
        }

        private void TextArea_TextChanged(object sender, EventArgs e)
        {
            if (Height == 1 && Bounds.WidthData is FlexSize && MinSize is int min)
            {
                Bounds.SetSizes(
                    new FlexSize(Math.Max(Text.Length + 1, min)),
                    Bounds.HeightData);
            }
        }
    }
}
