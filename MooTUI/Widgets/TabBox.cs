using MooTUI.Core;
using MooTUI.Input;
using MooTUI.Layout;
using MooTUI.Text;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets
{
    public class TabBox : Container
    {
        private static int _contentOffsetX = 1;
        private static int _contentOffsetY = 3;

        public BoxDrawing LineStyle { get; private set; }

        public int MaxContentWidth => Width - _contentOffsetX - 1;
        public int MaxContentHeight => Height - _contentOffsetY - 1;

        private List<Tab> Tabs { get; set; }
        private Tab CurrentTab { get; set; }

        public TabBox(LayoutRect bounds, BoxDrawing lineStyle = null) : base(bounds)
        {
            Tabs = new List<Tab>();
            LineStyle = lineStyle ?? BoxDrawing.Default;
        }

        public void AddTab(Widget content, TextSpan label)
        {
            Tab tab = new Tab(
                new LayoutRect(
                    new FlexSize(MaxContentWidth),
                    new FlexSize(MaxContentHeight)),
                content,
                label);

            LinkChild(tab);
            Tabs.Add(tab);
        }

        public override Widget GetHoveredWidget((int x, int y) relativeMouseLocation)
        {
            (int x, int y) = relativeMouseLocation;

            if (x >= _contentOffsetX && y >= _contentOffsetY
                && CurrentTab != null && CurrentTab.HitTest(x - _contentOffsetX, y - _contentOffsetY))
            {
                return CurrentTab;
            }

            return this;
        }

        protected override void DrawChild(Widget child)
        {
            if (CurrentTab != null && child == CurrentTab)
                Visual.Merge(CurrentTab.Visual, _contentOffsetX, _contentOffsetY);
        }

        protected override IEnumerable<Widget> GetLogicalChildren() => Tabs;

        protected internal override (int xOffset, int yOffset) GetChildOffset(Widget child) => 
            (_contentOffsetX, _contentOffsetY);

        protected override void Input(InputEventArgs e)
        {
            if (e is MouseClickInputEventArgs m && m.Button == MouseClickInputEventArgs.MouseButton.LEFT)
            {
                (int x, int y) = m.Location;
                if (y >= _contentOffsetY - 1)
                    return;
                int xOffset = 1;
                foreach (Tab t in Tabs)
                {
                    int width = t.LabelWidth + 2;
                    if (x >= xOffset && x <= xOffset + width)
                    {
                        CurrentTab = t;
                        RefreshVisual();
                        Render();
                        e.Handled = true;
                        return;
                    }
                    else
                    {
                        xOffset += width;
                    }
                }
            }
        }

        protected override void OnChildResized(Widget child) { } // Should never happen

        protected override void RefreshVisual()
        {
            Visual.FillCell(new Cell(' ', Style.GetColorPair("Default")));

            DrawTabs();
            LineStyle.DrawBox(Visual, Width, Height - 2, 0, 2);
            ConnectCurrentTab();
            DrawChild(CurrentTab);
        }

        protected override void Resize()
        {
            Lock = true;
            foreach(Tab t in Tabs)
            {
                t.Bounds.TryResize(MaxContentWidth, MaxContentHeight);
            }
            Lock = false;
        }

        private void DrawTabs()
        {
            int xOffset = 1;
            foreach(Tab t in Tabs)
            {
                Visual label = t.DrawLabel();
                LineStyle.DrawBox(Visual, label.Width + 2, 3, xOffset, 0);
                Visual.Merge(label, xOffset + 1, 1);
                xOffset += label.Width + 2;
            }
        }

        private void ConnectCurrentTab()
        {
            int xOffset = 1;
            foreach (Tab t in Tabs)
            {
                int width = t.LabelWidth + 2;
                if (t == CurrentTab)
                {
                    Visual.SetChar(xOffset, _contentOffsetY - 1, BoxDrawing.Default.UL);
                    Visual.SetChar(xOffset + width - 1, _contentOffsetY - 1, BoxDrawing.Default.UR);
                    Visual.FillChar(' ', xOffset + 1, _contentOffsetY - 1, width - 2, 1);
                    return;
                }
                else
                {
                    xOffset += width;
                }
            }
        }

        private class Tab : MonoContainer
        {
            private static TextAreaEnclosure _enclosure = new TextAreaEnclosure(" ", " ");

            public TextSpan Label { get; set; }

            public int LabelWidth => Label.Length + _enclosure.TotalWidth;

            public Tab(LayoutRect bounds, Widget content, TextSpan label) : base(bounds)
            {
                Label = label;
                SetContent(content);
                TryStretchContent();
            }

            public Visual DrawLabel() => _enclosure.DrawEnclosure(Label);

            public override Widget GetHoveredWidget((int x, int y) relativeMouseLocation)
            {
                if (Content.HitTest(relativeMouseLocation.x, relativeMouseLocation.y))
                    return Content;
                else
                    return this;
            }

            protected override void DrawChild(Widget child)
            {
                Visual.Merge(child.Visual);
            }

            protected override void OnChildResized(Widget child) { }

            protected internal override (int xOffset, int yOffset) GetChildOffset(Widget child) => (0, 0);

            protected override void RefreshVisual() 
            {
                DrawChild(Content);
            }

            protected override void Input(InputEventArgs e) { }

            protected override void Resize()
            {
                TryStretchContent();
            }

            private void TryStretchContent()
            {
                Content.Bounds.TryResize(Width, Height);
            }
        }
    }
}
