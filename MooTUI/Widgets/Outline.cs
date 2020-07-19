﻿using MooTUI.Core;
using MooTUI.Input;
using MooTUI.Layout;
using MooTUI.Text;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets
{
    public class Outline : MonoContainer
    {
        public TextSpan Text { get; private set; }
        public BoxDrawing LineStyle { get; private set; }

        public static TextSpanEnclosure Enclosure { get; set; } =
            new TextSpanEnclosure("{ ", " }", new ColorPair());

        public Outline(Widget w, string text = "", BoxDrawing lineStyle = null)
            : base(new LayoutRect(w.Bounds.WidthData.WithRelativeSize(2), w.Bounds.HeightData.WithRelativeSize(2)))
        {
            SetContent(w);
            Text = TextSpan.FromString(text);
            LineStyle = lineStyle ?? BoxDrawing.Default;

            RefreshVisual();
        }

        public override Widget GetHoveredWidget(MouseContext m)
        {
            (int x, int y) = m.Mouse;

            if (Content.HitTest(x - 1, y - 1))
            {
                m.SetRelativeMouse(-1, -1);
                return Content;
            }
            else
            {
                return this;
            }
        }

        protected override void DrawChild(Widget child)
        {
            Visual.Merge(child.Visual, 1, 1);
        }

        protected override void OnChildResized(Widget child)
        {
            Bounds.WithSize(Orientation.Horizontal, Content.Bounds.WidthData.WithRelativeSize(2));
            Bounds.WithSize(Orientation.Vertical, Content.Bounds.HeightData.WithRelativeSize(2));
        }

        protected override void Input(InputEventArgs e) { }

        protected override void Resize()
        {
            Lock = true;

            Content.Bounds.TryResize(Width - 2, Height - 2);

            Lock = false;
        }

        protected override void RefreshVisual()
        {
            Visual.FillCell(new Cell(' ', Style.GetColorPair("Default")));

            DrawChild(Content);

            DrawOutline();
            DrawText();
        }

        private void DrawOutline()
        {
            LineStyle.DrawBox(Visual, Width, Height);
        }

        private void DrawText()
        {
            if (Text.Text.Length > 0)
                Visual.Merge(Enclosure.DrawEnclosure(Text), 
                    1, 0, 
                    0, 0,
                    Width, 1);
        }
    }
}
