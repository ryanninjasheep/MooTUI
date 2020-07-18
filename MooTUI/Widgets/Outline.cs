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
    public class Outline : MonoContainer
    {
        public TextSpan Text { get; private set; }
        public BoxDrawingChars LineStyle { get; private set; }

        public static TextSpanEnclosure Enclosure { get; set; } =
            new TextSpanEnclosure("{ ", " }", new ColorPair());

        public Outline(Widget w, string text = "", BoxDrawingChars lineStyle = null)
            : base(new LayoutRect(w.Bounds.WidthData.WithRelativeSize(2), w.Bounds.HeightData.WithRelativeSize(2)))
        {
            SetContent(w);
            Text = TextSpan.FromString(text);
            LineStyle = lineStyle ?? BoxDrawingChars.Default;

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
            Visual.FillChar(LineStyle.LR, 0, 0, Width - 1, 1);
            Visual.FillChar(LineStyle.LR, 0, Height - 1, Width, 1);
            Visual.FillChar(LineStyle.UD, 0, 0, 1, Height - 1);
            Visual.FillChar(LineStyle.UD, Width - 1, 0, 1, Height - 1);

            Visual.SetChar(0, 0, LineStyle.DR);
            Visual.SetChar(Width - 1, 0, LineStyle.DL);
            Visual.SetChar(0, Height - 1, LineStyle.UR);
            Visual.SetChar(Width - 1, Height - 1, LineStyle.UL);
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

    public class BoxDrawingChars
    {
        public char UL { get; private set; }
        public char UD { get; private set; }
        public char UR { get; private set; }
        public char DL { get; private set; }
        public char DR { get; private set; }
        public char LR { get; private set; }

        public static BoxDrawingChars Default = new BoxDrawingChars
        {
            UL = '┘',
            UD = '│',
            UR = '└',
            DL = '┐',
            DR = '┌',
            LR = '─'
        };
        public static BoxDrawingChars Double = new BoxDrawingChars
        {
            UL = '╝',
            UD = '║',
            UR = '╚',
            DL = '╗',
            DR = '╔',
            LR = '═'
        };
        public static BoxDrawingChars Rounded = new BoxDrawingChars
        {
            UL = '╯',
            UD = '│',
            UR = '╰',
            DL = '╮',
            DR = '╭',
            LR = '─'
        };
    }
}
