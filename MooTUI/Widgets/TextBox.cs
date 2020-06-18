using MooTUI.Widgets.Primitives;
using MooTUI.Input;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MooTUI.IO;

namespace MooTUI.Widgets
{
    public class TextBox : Widget, IEnsureCursorVisible
    {
        public Span PromptText { get; }
        public EditableSpan Span { get; private set; }
        public string Text { get => Span.Text; }

        private bool IsFocused { get; set; }
        private bool IsHovered { get; set; }

        public TextBox(int width, int height, string promptText) : base(width, height)
        {
            PromptText = new Span(promptText, width);
            Span = new EditableSpan("", width);

            IsFocused = false;
            IsHovered = false;
        }
        public TextBox(int width, int height) : this(width, height, "") { }

        public event EventHandler<CursorRegionEventArgs> EnsureCursorVisible;
        protected void OnEnsureCursorVisible()
        {
            (int x, int y) = Span.GetCursorCell();
            CursorRegionEventArgs e = new CursorRegionEventArgs(x, y);

            EventHandler<CursorRegionEventArgs> handler = EnsureCursorVisible;
            handler?.Invoke(this, e);
        }

        public event EventHandler TextChanged;
        protected virtual void OnTextChanged()
        {
            EventHandler handler = TextChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected override void AdjustResize()
        {
            Span.SetDisplayWidth(Width);
        }

        public void Write(string text)
        {
            Span.Write(text);

            OnTextChanged();

            Render();
        }
        public void Backspace()
        {
            Span.Backspace();

            OnTextChanged();

            Render();
        }
        public void HandleCommand(Command c)
        {
            switch (c)
            {
                case Command.COPY:
                    Clipboard.SetText(Span.GetSelection());
                    break;
                case Command.CUT:
                    Clipboard.SetText(Span.GetSelection());
                    Span.DeleteSelection();
                    OnTextChanged();
                    break;
                case Command.PASTE:
                    Write(Clipboard.GetText());
                    break;
                default:
                    // ok
                    break;
            }
        }

        #region DISPLAY CONSTANTS

        private static readonly ColorFamily Base = new ColorFamily() { "TextBox_Default", "Default" };
        private static readonly ColorFamily Hover = new ColorFamily() { "TextBox_Hover", "Hover" };
        private static readonly ColorFamily Active = new ColorFamily() { "TextBox_Active", "Active" };
        private static readonly ColorFamily Prompt = new ColorFamily() { "TextBox_Pompt", "Default" };
        private static readonly ColorFamily Selection = new ColorFamily() { "TextBox_Selection", "Selection" };
        private static readonly ColorFamily Cursor = new ColorFamily() { "TextBox_Cursor", "Cursor" };

        #endregion

        protected override void Draw()
        {
            base.Draw();

            if (IsFocused)
            {
                View.FillColorScheme(Style.GetColorScheme(Active));

                DrawSelection();
                DrawCursor();
            }
            else if (IsHovered)
            {
                View.FillColorScheme(Style.GetColorScheme(Hover));
            }
            else if (Span.Length == 0)
            {
                View.FillColorScheme(Style.GetColorScheme(Prompt));
            }
            else
            {
                View.FillColorScheme(Style.GetColorScheme(Base));
            }

            View.DrawSpan(Span);
        }

        protected void DrawSelection()
        {
            if (Span.IsSelectionActive)
            {
                List<(int, int)> selectedCells = Span.GetSelectedCells();
                foreach ((int x, int y) in selectedCells)
                {
                    View.SetColorScheme(x, y, Style.GetColorScheme(Selection));
                }
            }
        }
        protected void DrawCursor()
        {
            (int x, int y) = Span.GetCursorCell();

            if (x < Width && y < Height)
                View.SetColorScheme(x, y, Style.GetColorScheme(Cursor));
        }

        protected override void Input(Input.InputEventArgs e)
        {
            switch (e.InputType)
            {
                case InputTypes.FOCUS:
                    OnFocus();
                    break;
                case InputTypes.UNFOCUS:
                    OnUnfocus();
                    break;
                case InputTypes.MOUSE_ENTER:
                    OnMouseEnter();
                    break;
                case InputTypes.MOUSE_LEAVE:
                    OnMouseLeave();
                    break;
                case InputTypes.LEFT_CLICK:
                    OnLeftClick(e);
                    break;
                case InputTypes.KEY_DOWN:
                    OnKeyDown(e);
                    OnEnsureCursorVisible();
                    break;
                default:
                    break;
            }
        }

        private void OnFocus()
        {
            IsFocused = true;

            Render();
        }
        private void OnUnfocus()
        {
            IsFocused = false;

            Render();
        }

        private void OnMouseEnter()
        {
            IsHovered = true;

            Render();
        }
        private void OnMouseLeave()
        {
            IsHovered = false;

            Render();
        }

        private void OnLeftClick(Input.InputEventArgs e)
        {
            OnClaimFocus(new FocusEventArgs(this));

            Span.SetCursor(Span.GetIndexAtCell(e.Mouse.Mouse.X, e.Mouse.Mouse.Y));
            Render();

            e.Handled = true;
        }

        private void OnKeyDown(Input.InputEventArgs e)
        {
            bool hadEffect;

            switch (e.Keyboard.LastKeyPressed)
            {
                case Key.Enter:
                    Write("\n");
                    e.Handled = true;
                    return;
                case Key.Back:
                    Backspace();
                    e.Handled = true;
                    return;
                case Key.Up:
                    hadEffect = Span.CursorUp(e.Keyboard.Shift);
                    Render();
                    e.Handled = hadEffect;
                    return;
                case Key.Down:
                    hadEffect = Span.CursorDown(e.Keyboard.Shift);
                    Render();
                    e.Handled = hadEffect;
                    return;
                case Key.Left:
                    hadEffect = Span.CursorLeft(e.Keyboard.Shift);
                    Render();
                    e.Handled = hadEffect;
                    return;
                case Key.Right:
                    hadEffect = Span.CursorRight(e.Keyboard.Shift);
                    Render();
                    e.Handled = hadEffect;
                    return;
            }

            if (e.Keyboard.GetCommand() != Command.NONE)
            {
                HandleCommand(e.Keyboard.GetCommand());

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
    }
}
