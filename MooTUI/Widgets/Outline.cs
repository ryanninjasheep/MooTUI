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
        public SingleLineTextSpan Text { get; private set; }
        public BoxDrawingChars LineStyle { get; private set; }

        public static TextSpanEnclosure Enclosure { get; set; } =
            new TextSpanEnclosure("{ ", " }", new ColorPair());

        public Outline(Widget w, SingleLineTextSpan text = null, BoxDrawingChars lineStyle = null)
            : base(new LayoutRect(w.Bounds.WidthData.WithRelativeSize(2), w.Bounds.HeightData.WithRelativeSize(2)))
        {
            SetContent(w);
            Text = text ?? new SingleLineTextSpan();
            LineStyle = lineStyle ?? BoxDrawingChars.Default;

            RefreshVisuals();
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

        protected override void Draw()
        {
            View.Merge(Content.View, 1, 1);
        }

        protected override void OnChildResize(Widget child) =>
            Resize(new LayoutRect(
                child.Bounds.WidthData.WithRelativeSize(2), 
                child.Bounds.HeightData.WithRelativeSize(2)
                ));

        protected override void Input(InputEventArgs e) { }

        private void RefreshVisuals()
        {
            View.ClearText();
            View.FillColorScheme(Style.GetColorPair("Default"));

            DrawOutline();
            DrawText();
            Draw();
        }

        private void DrawOutline()
        {
            View.FillChar(LineStyle.LR, 0, 0, Width - 1, 1);
            View.FillChar(LineStyle.LR, 0, Height - 1, Width, 1);
            View.FillChar(LineStyle.UD, 0, 0, 1, Height - 1);
            View.FillChar(LineStyle.UD, Width - 1, 0, 1, Height - 1);

            View.SetChar(0, 0, LineStyle.DR);
            View.SetChar(Width - 1, 0, LineStyle.DL);
            View.SetChar(0, Height - 1, LineStyle.UR);
            View.SetChar(Width - 1, Height - 1, LineStyle.UL);
        }

        private void DrawText()
        {
            View.Merge(Enclosure.DrawEnclosure(Text), 1, 0);
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
