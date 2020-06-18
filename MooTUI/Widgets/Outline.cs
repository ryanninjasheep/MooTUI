using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Input;
using MooTUI.IO;

namespace MooTUI.Widgets
{
    public class Outline : MonoContainer
    {
        public string Text { get; private set; }
        public bool IsDoubleThick { get; private set; }

        public Outline(Widget w, string text, bool isDoubleThick) : base(w.Width + 2, w.Height + 2)
        {
            Text = text;

            IsDoubleThick = isDoubleThick;

            SetContent(w);
        }
        public Outline(Widget w, string text) : this(w, text, false) { }

        protected override void OnChildResize()
        {
            Resize(Content.Width + 2, Content.Height + 2);

            Render();
        }

        public void SetText(string text)
        {
            Text = text;
            Render();
        }

        #region DISPLAY CONSTANTS

        private static readonly ColorFamily Base = new ColorFamily() { "Outline_Default", "Default" };

        #endregion

        protected override void Draw()
        {
            // ┌┬┐ ─ │ 
            // ├┼┤
            // └┴┘
            //╝ ╗ ╔ ╚ ╣ ╩ ╦ ╠ ═ ║ ╬

            base.Draw();

            View.FillColorScheme(Style.GetColorScheme(Base));

            View.Merge(Content.View, 1, 1);

            if (IsDoubleThick)
            {
                View.FillChar('═', 0, 0, Width - 1, 1);
                View.FillChar('═', 0, Height - 1, Width, 1);
                View.FillChar('║', 0, 0, 1, Height - 1);
                View.FillChar('║', Width - 1, 0, 1, Height - 1);

                View.SetChar(0, 0, '╔');
                View.SetChar(Width - 1, 0, '╗');
                View.SetChar(0, Height - 1, '╚');
                View.SetChar(Width - 1, Height - 1, '╝');
            }
            else
            {
                View.FillChar('─', 0, 0, Width - 1, 1);
                View.FillChar('─', 0, Height - 1, Width, 1);
                View.FillChar('│', 0, 0, 1, Height - 1);
                View.FillChar('│', Width - 1, 0, 1, Height - 1);

                View.SetChar(0, 0, '┌');
                View.SetChar(Width - 1, 0, '┐');
                View.SetChar(0, Height - 1, '└');
                View.SetChar(Width - 1, Height - 1, '┘');
            }

            View.SetText(Text, 1, 0);
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
    }
}
