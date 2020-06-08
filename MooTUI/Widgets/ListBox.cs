using MooTUI.Core;
using MooTUI.IO;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.RightsManagement;
using System.Text;
using System.Windows.Navigation;

namespace MooTUI.Widgets
{
    public class ListBox : Container
    {
        private ScrollViewer ScrollViewer { get; set; }
        private ListBoxContent Content { get; set; }

        public int SelectedIndex { get => Content.SelectedIndex; }
        public IList<string> Items => Content.GetItems();

        public ListBox(int width, int height) : base(width, height)
        {
            Content = new ListBoxContent(width - 1, height);
            ScrollViewer = new ScrollViewer(width, height, Content, 
                ScrollViewer.ScrollBarVisibility.DISABLED, ScrollViewer.ScrollBarVisibility.AUTO);
            LinkChild(ScrollViewer);
            Content.SelectionChanged += Content_SelectionChanged;
        }

        public event EventHandler SelectionChanged;
        protected void OnSelectionChanged()
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
        private void Content_SelectionChanged(object sender, EventArgs e)
        {
            OnSelectionChanged();
        }

        public void Add(string item) => Insert(item, Items.Count);
        public void Insert(string item, int index) => Content.Add(item, index);

        public void Remove(string item)
        {
            int index = Items.IndexOf(item);

            if (index != -1)
                RemoveAt(index);
        }

        public void RemoveAt(int index) => Content.RemoveAt(index);

        public void SetSelectedIndex(int index) => Content.SetSelectedIndex(index);

        public bool IsValidIndex(int index) => Content.IsValidIndex(index);

        public override Widget GetHoveredWidget(MouseContext m) => ScrollViewer;

        protected override void SetChildStyle(Style style, bool overrideDefault) =>
            ScrollViewer.SetStyle(style, overrideDefault);

        protected override void OnChildResize() => Render(); // This shouldn't happen

        protected override void Draw()
        {
            base.Draw();

            View.Merge(ScrollViewer.View, 0, 0);
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Internal Private Classes
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private class ListBoxContent : Container, IEnsureCursorVisible
        {
            private List<ListBoxItem> Items { get; set; }
            public int SelectedIndex { get; set; }

            public ListBoxContent(int width, int height) : base(width, height) 
            {
                Items = new List<ListBoxItem>();
                SelectedIndex = -1;
            }

            public IList<string> GetItems()
            {
                List<string> toReturn = new List<string>();

                foreach (ListBoxItem i in Items)
                {
                    toReturn.Add(i.Text);
                }

                return toReturn;
            }

            public void Add(string s, int index)
            {
                AddChild(ListBoxItem.Factory(s, Width, this), index);
                RefreshHeight();
            }

            public void RemoveAt(int index)
            {
                Items.RemoveAt(index);
                SetSelectedIndex(Math.Min(SelectedIndex, Items.Count - 1));
                RefreshHeight();
            }

            public void ClaimSelection(ListBoxItem i)
            {
                int selectedIndex = Items.IndexOf(i);

                OnClaimFocus(new FocusEventArgs(this));

                SetSelectedIndex(selectedIndex);
            }

            public void SetSelectedIndex(int index)
            {
                if (!IsValidIndex(index) && index != -1)
                    throw new ArgumentOutOfRangeException("index", index, "The specified index is not within bounds!");

                if (SelectedIndex >= 0 && IsValidIndex(SelectedIndex))
                    Items[SelectedIndex].Unselect();

                SelectedIndex = index;

                if (SelectedIndex >= 0)
                    Items[SelectedIndex].Select();

                OnEnsureCursorVisible();

                OnSelectionChanged();
            }

            public bool IsValidIndex(int index) => (index >= 0 && index < Items.Count);

            public event EventHandler<CursorRegionEventArgs> EnsureCursorVisible;
            protected void OnEnsureCursorVisible()
            {
                if (SelectedIndex < 0 || SelectedIndex > Items.Count)
                    return;

                (int x, int y, int width, int height) = GetSelectedRegion();

                CursorRegionEventArgs e = new CursorRegionEventArgs(x, y, width, height);

                EventHandler<CursorRegionEventArgs> handler = EnsureCursorVisible;
                handler?.Invoke(this, e);
            }

            public event EventHandler SelectionChanged;
            protected void OnSelectionChanged()
            {
                EventHandler handler = SelectionChanged;
                handler?.Invoke(this, EventArgs.Empty);
            }

            private (int x, int y, int width, int height) GetSelectedRegion()
            {
                if (SelectedIndex < 0 || SelectedIndex > Items.Count)
                    throw new IndexOutOfRangeException();

                int total = 0;
                for (int i = 0; i < SelectedIndex; i++)
                {
                    total += Items[i].Height;
                }

                int width = Items[SelectedIndex].Width;
                int height = Items[SelectedIndex].Height;

                return (0, total, width, height);
            }

            private void AddChild(ListBoxItem i, int index)
            {
                Items.Insert(index, i);
                LinkChild(i);
            }

            private void RefreshHeight()
            {
                Resize(Width, GetContentHeight());
            }

            private int GetContentHeight()
            {
                int toReturn = 0;

                foreach (ListBoxItem i in Items)
                {
                    toReturn += i.Height;
                }

                return toReturn;
            }

            public override Widget GetHoveredWidget(MouseContext m)
            {
                (int x, int y) = m.Mouse;

                int yIndex = 0;
                foreach (Widget w in Items)
                {
                    if (yIndex + w.Height > y)
                    {
                        if (x < w.Width)
                        {
                            m.SetRelativeMouse(0, -yIndex);

                            return w;
                        }
                        break;
                    }

                    yIndex += w.Height;
                }

                // if nothing is hovered over
                return this;
            }

            protected override void OnChildResize()
            {
                // This shouldn't happen????
                Render();
            }

            protected override void SetChildStyle(Style style, bool overrideDefault)
            {
                foreach (Widget w in Items)
                {
                    w.SetStyle(style, overrideDefault);
                }
            }

            protected override void Draw()
            {
                base.Draw();

                View.FillColorScheme(Style.GetColorScheme("Default"));

                int yIndex = 0;
                for (int i = 0; i < Items.Count; i++)
                {
                    View.Merge(Items[i].View, 0, yIndex);

                    yIndex += Items[i].Height;
                }
            }

            protected override void Input(InputEventArgs e)
            {
                if (e.InputType == InputTypes.KEY_DOWN)
                {
                    OnKeyDown(e);
                }
            }

            private void OnKeyDown(InputEventArgs e)
            {
                if (e.Keyboard.LastKeyPressed == System.Windows.Input.Key.Up && SelectedIndex > 0)
                {
                    SetSelectedIndex(SelectedIndex - 1);
                    e.Handled = true;
                    Render();
                }
                else if (e.Keyboard.LastKeyPressed == System.Windows.Input.Key.Down && SelectedIndex < Items.Count - 1)
                {
                    SetSelectedIndex(SelectedIndex + 1);
                    e.Handled = true;
                    Render();
                }
            }
        }

        private class ListBoxItem : Widget
        {
            private Span Span { get; set; }
            public string Text { get => Span.Text; }

            private bool IsHovered { get; set; }
            private bool IsSelected { get; set; }

            private ListBoxContent Parent { get; }

            public static ListBoxItem Factory(string text, int width, ListBoxContent parent)
            {
                Span s = new Span(text, width);
                return new ListBoxItem(width, s.Height, s, parent);
            }

            private ListBoxItem(int width, int height, Span span, ListBoxContent parent) : base(width, height)
            {
                Span = span;
                Parent = parent;

                IsHovered = false;
                IsSelected = false;
            }

            public void Select()
            {
                IsSelected = true;
                Render();
            }
            public void Unselect()
            {
                IsSelected = false;
                Render();
            }

            #region DISPLAY CONSTANTS

            private static readonly ColorFamily Base = new ColorFamily() { "ListBoxItem_Default", "Default" };
            private static readonly ColorFamily Hover = new ColorFamily() { "ListBoxItem_Hover", "Hover" };
            private static readonly ColorFamily Selection = new ColorFamily() { "ListBoxItem_Selection", "Cursor" };

            #endregion

            protected override void Draw()
            {
                base.Draw();

                View.DrawSpan(Span);

                if (IsSelected)
                    View.FillColorScheme(Style.GetColorScheme(Selection));
                else if (IsHovered)
                    View.FillColorScheme(Style.GetColorScheme(Hover));
                else
                    View.FillColorScheme(Style.GetColorScheme(Base));
            }

            protected override void Input(InputEventArgs e)
            {
                switch (e.InputType)
                {
                    case InputTypes.MOUSE_ENTER:
                        IsHovered = true;
                        Render();
                        break;
                    case InputTypes.MOUSE_LEAVE:
                        IsHovered = false;
                        Render();
                        break;
                    case InputTypes.LEFT_CLICK:
                        Parent.ClaimSelection(this);
                        break;
                    default:
                        //ok
                        break;
                }
            }
        }
    }
}
