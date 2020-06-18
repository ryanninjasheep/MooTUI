using MooTUI.Widgets.Primitives;
using MooTUI.Input;
using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.IO;

namespace MooTUI.Widgets
{
    public class CheckBox : ButtonBase
    {
        private bool IsMouseOver { get; set; }

        public bool IsChecked { get; private set; }
        public string Text { get; private set; }

        public CheckBox(int width, int height, string text, bool isChecked) : base(width, height)
        {
            IsMouseOver = false;

            IsChecked = isChecked;
            SetText(text);
        }

        public void SetText(string text)
        {
            Text = text;
            Render();
        }

        public void Check()
        {
            if (!IsChecked)
            {
                IsChecked = true;
                Render();
            }
        }
        public void Uncheck()
        {
            if (IsChecked)
            {
                IsChecked = false;
                Render();
            }
        }

        #region DISPLAY CONSTANTS

        private static readonly ColorFamily Base = new ColorFamily() { "CheckBox_Default", "Default" };
        private static readonly ColorFamily Hover = new ColorFamily() { "CheckBox_Hover", "Hover" };

        #endregion

        protected override void Draw()
        {
            base.Draw();

            View.FillColorScheme(Style.GetColorScheme(Base));

            if (IsMouseOver)
                View.SetColorScheme(2, 0, Style.GetColorScheme(Hover));

            View.SetText(" [" + (IsChecked ? "X" : " ") + "] " + Text);
        }

        protected override void OnMouseEnter()
        {
            IsMouseOver = true;

            Render();
        }
        protected override void OnMouseLeave()
        {
            IsMouseOver = false;

            Render();
        }

        protected override void OnLeftClick(InputEventArgs e)
        {
            if (IsChecked)
                Uncheck();
            else
                Check();

            OnClick(EventArgs.Empty);

            e.Handled = true;
        }
    }
}
