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
    public class Box : MonoContainer
    {
        public TextSpan Text { get; private set; }
        public BoxDrawing LineStyle { get; private set; }

        public static TextAreaEnclosure Enclosure { get; set; } =
            new TextAreaEnclosure("{ ", " }", new ColorPair());

        public Box(Widget w, string text = "", BoxDrawing lineStyle = null)
            : this(w.Bounds.WithRelativeSize(2, 2),
                  w, text, lineStyle)
        { }
        private protected Box(LayoutRect bounds, Widget w, string text, BoxDrawing lineStyle) : base(bounds)
        {
            Text = TextSpan.FromString(text);
            LineStyle = lineStyle ?? BoxDrawing.Default;

            SetContent(w);
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
            Bounds.SetSizes(
                Content.Bounds.WidthData.WithRelativeSize(2),
                Content.Bounds.HeightData.WithRelativeSize(2));
        }

        protected override (int xOffset, int yOffset) GetOffset(Widget child) => (1, 1);

        protected override void Input(InputEventArgs e) { }

        protected override void Resize()
        {
            if (Lock)
                return;

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
