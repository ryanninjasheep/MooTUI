using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using MooTUI.Widgets.Primitives;
using MooTUI.Core;
using MooTUI.IO;

namespace MooTUI.Widgets
{
    public class TextBlock : Widget
    {
        protected Span Span { get; private set; }
        public string Text { get => Span.Text; }

        public TextBlock(int width, int height, string text) : base(width, height) 
        {
            Span = new Span(text, width);
        }

        public void SetText(string text)
        {
            Span.SetText(text);
            Render();
        }

        #region DISPLAY CONSTANTS

        private static readonly ColorFamily Base = new ColorFamily() { "TextBlock_Default", "Default" };

        #endregion

        protected override void Draw()
        {
            base.Draw();

            View.FillColorScheme(Style.GetColorScheme(Base));

            View.DrawSpan(Span);
        }
    }
}
