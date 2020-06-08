using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Widgets.Primitives;
using MooTUI.IO;

namespace MooTUI.Widgets
{
    public class Button : ButtonBase
    {
        public Span Span { get; private set; }

        private bool IsHovered { get; set; }

        public Button(int width, int height, string text) : base(width, height) 
        {
            Span = new Span(text, width, HJustification.CENTER, VJustification.CENTER);
            IsHovered = false;
        }
        public Button(string text) : this(text.Length + 2, 1, text) { }

        #region DISPLAY CONSTANTS

        private static readonly ColorFamily Base = new ColorFamily() { "Button_Default", "Default" };
        private static readonly ColorFamily Hover = new ColorFamily() { "Button_Hover", "Hover" };

        #endregion

        protected override void Draw()
        {
            base.Draw();

            if (IsHovered)
                View.FillColorScheme(Style.GetColorScheme(Hover));
            else
                View.FillColorScheme(Style.GetColorScheme(Base));

            View.DrawSpan(Span);
        }

        public void SetText(string text)
        {
            Span.SetText(text);
            Render();
        }

        protected override void OnMouseEnter()
        {
            IsHovered = true;

            Render();
        }
        protected override void OnMouseLeave()
        {
            IsHovered = false;

            Render();
        }
    }
}
