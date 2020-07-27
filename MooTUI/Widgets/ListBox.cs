using MooTUI.Core;
using MooTUI.Input;
using MooTUI.Layout;
using MooTUI.Text;
using MooTUI.Widgets.Primitives;
using Sys = System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MooTUI.Widgets
{
    public class ListBox : MonoContainer
    {
        public int CursorIndex { get; private set; } = -1;
        public object CursorObject => CursorElement?.Object;

        public int Count => Container.Children.Count;

        public bool IsSelectionEnabled { get; }

        private LayoutContainer Container => (Content as ScrollBox).Content as LayoutContainer;

        private ListBoxElement CursorElement => 
            CursorIndex > -1 && CursorIndex < Count
                ? Container.Children[CursorIndex] as ListBoxElement
                : null;

        public int SelectionStart => Math.Min(SelectionFrom, SelectionTo);
        public int SelectionEnd => Math.Max(SelectionFrom, SelectionTo);
        public bool IsSelectionActive => IsSelectionEnabled && SelectionFrom != SelectionTo;

        private int SelectionFrom { get; set; }
        private int SelectionTo { get; set; }

        public ListBox(LayoutRect bounds, string title = "", 
            bool selectionEnabled = false, BoxDrawing lineStyle = null) : base(bounds)
        {
            LayoutContainer container = new LayoutContainer(
                new LayoutRect(
                    new FlexSize(bounds.Width - 2),
                    new FlexSize(1)),
                Orientation.Vertical,
                LayoutContainer.MainAxisJustification.FIT);

            SetContent(new ScrollBox(
                bounds.Clone(),
                container,
                text: title,
                lineStyle: lineStyle));

            IsSelectionEnabled = selectionEnabled;
        }

        public event EventHandler CursorMoved;

        public override Widget GetHoveredWidget(MouseContext m) => Content;

        public void Add(string text, object reference = null)
        {
            Container.AddChild(ListBoxElement.Generate(text, Width - 2, reference, this));
        }
        public void Insert(int index, string text, object reference = null)
        {
            Container.InsertChild(ListBoxElement.Generate(text, Width - 2, reference, this), index);
        }
        public void RemoveElementUnderCursor()
        {
            Container.RemoveChild(CursorElement);
            SetCursorIndex(CursorIndex);
        }

        public void SetCursorIndex(int index, bool selectionActive = false)
        {
            if (index < -1 || index >= Count)
            {
                SetCursorIndex(-1);
                return;
            }

            if (CursorElement != null)
            {
                CursorElement.IsCursor = false;
            }
            CursorIndex = index;
            if (CursorElement != null)
            {
                CursorElement.IsCursor = true;
            }

            if (CursorIndex == -1)
            {
                SelectionFrom = 0;
                SelectionTo = 0;
            }
            else if (IsSelectionEnabled)
            {
                if (!selectionActive)
                    SelectionFrom = CursorIndex;
                SelectionTo = CursorIndex;
            }

            RefreshSelection();

            OnCursorMoved(EventArgs.Empty);
        }

        protected override void DrawChild(Widget child)
        {
            Visual.Merge(Content.Visual);
        }

        protected override void OnChildResized(Widget child)
        {
            // SHOULD NEVER HAPPEN
        }

        protected override (int xOffset, int yOffset) GetOffset(Widget child) => (0, 0);

        protected override void Resize()
        {
            Lock = true;
            Content.Bounds.SetSizes(Bounds.WidthData.Clone(), Bounds.HeightData.Clone());
            Lock = false;
        }

        protected override void RefreshVisual()
        {
            Visual.Merge(Content.Visual);
        }

        protected override void Input(InputEventArgs e) { }

        private void SetCursorElement(ListBoxElement element, bool shift)
        {
            SetCursorIndex(Container.Children.IndexOf(element), shift);
        }

        private List<ListBoxElement> GetSelectedElements()
        {
            return Container.Children
                .ConvertAll((w) => w as ListBoxElement)
                .Where((e) => e.IsSelected)
                .ToList();
        }

        private void RefreshSelection()
        {
            if (!IsSelectionEnabled)
                return;

            foreach (ListBoxElement e in GetSelectedElements())
            {
                e.IsSelected = false;
            }

            if (IsSelectionActive)
            {
                for (int i = SelectionStart; i <= SelectionEnd; i++)
                {
                    (Container.Children[i] as ListBoxElement).IsSelected = true;
                }
            }
        }

        private bool TryMoveCursorUp(bool shift)
        {
            if (CursorIndex > 0)
            {
                SetCursorIndex(CursorIndex - 1, shift);
                return true;
            }
            return false;
        }
        private bool TryMoveCursorDown(bool shift)
        {
            if (CursorIndex < Count - 1)
            {
                SetCursorIndex(CursorIndex + 1, shift);
                return true;
            }
            return false;
        }

        private void OnCursorMoved(EventArgs e)
        {
            EventHandler handler = CursorMoved;
            handler?.Invoke(this, e);
        }

        private class ListBoxElement : Widget
        {
            // Controlled circular reference ok
            private ListBox _parent;

            private bool _isCursor;
            private bool _isSelected;
            private bool _isMouseOver;

            public TextArea Text { get; }
            public object Object { get; }

            public bool IsCursor
            {
                get => _isCursor;
                set
                {
                    _isCursor = value;
                    Render();
                    EnsureRegionVisible(0, 0, Width, Height);
                }
            }
            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    _isSelected = value;
                    Render();
                }
            }
            public bool IsMouseOver
            {
                get => _isMouseOver;
                set
                {
                    _isMouseOver = value;
                    Render();
                }
            }

            private ListBoxElement(LayoutRect bounds, TextArea text, object reference, ListBox parent) : base(bounds)
            {
                Text = text;
                Object = reference ?? Text.Text;
                _parent = parent;
            }
            public static ListBoxElement Generate(string text, int width, object reference, ListBox parent)
            {
                TextArea span = TextArea.FromString(text, width);
                LayoutRect bounds = new LayoutRect(
                    new FlexSize(span.Width),
                    new FlexSize(span.Draw().Height));

                return new ListBoxElement(bounds, span, reference, parent);
            }

            protected override void Input(InputEventArgs e)
            {
                switch (e.InputType)
                {
                    case InputTypes.MOUSE_ENTER:
                        IsMouseOver = true;
                        break;
                    case InputTypes.MOUSE_LEAVE:
                        IsMouseOver = false;
                        break;
                    case InputTypes.LEFT_CLICK:
                        ClaimFocus();
                        _parent.SetCursorElement(this, e.Keyboard.Shift);
                        e.Handled = true;
                        break;
                    case InputTypes.KEY_DOWN:
                        switch (e.Keyboard.LastKeyPressed)
                        {
                            case Sys.Key.Up:
                                e.Handled = _parent.TryMoveCursorUp(e.Keyboard.Shift);
                                break;
                            case Sys.Key.Down:
                                e.Handled = _parent.TryMoveCursorDown(e.Keyboard.Shift);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        // ok, just do nothing.
                        break;
                }
            }

            protected override void RefreshVisual() { }

            protected override void Draw()
            {
                if (IsCursor)
                    Visual.FillCell(new Cell(' ', Style.GetColorPair("Cursor")));
                else if (IsSelected)
                    Visual.FillCell(new Cell(' ', Style.GetColorPair("Selection")));
                else if (IsMouseOver)
                    Visual.FillCell(new Cell(' ', Style.GetColorPair("Hover")));
                else
                    Visual.FillCell(new Cell(' ', Style.GetColorPair("Default")));

                Visual.DrawTextArea(Text);
            }
        }
    }
}
