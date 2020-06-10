using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using MooTUI.Widgets.Primitives;
using MooTUI.Core;
using MooTUI.IO;

namespace MooTUI.Widgets
{
    public class Window : MonoContainer, IPushMessage
    {
        public bool IsMessageEnabled { get; private set; }
        public Message CurrentMessage { get; private set; }

        public Window(int width, int height) : base(width, height)
        {
            IsMessageEnabled = true;
        }

        public new void SetContent(Widget w) => base.SetContent(w);

        protected override void OnChildResize()
        {
            Render();
        }

        public void PushMessage(Message m)
        {
            if (IsMessageEnabled)
            {
                CurrentMessage = m;
                Render();
            }
        }

        public override Widget GetHoveredWidget(MouseContext m)
        {
            if (Content.HitTest(m.Mouse.X, m.Mouse.Y))
                return Content;
            else
                return this;
        }

        #region DISPLAY CONSTANTS

        private static readonly ColorFamily Base = new ColorFamily() { "Window_Default", "Default" };
        private static readonly ColorFamily MessageDisp = new ColorFamily() { "Window_Message", "Message" };
        private static readonly ColorFamily Info = new ColorFamily() { "Window_Info", "Info" };
        private static readonly ColorFamily Warning = new ColorFamily() { "Window_Warning", "Warning" };
        private static readonly ColorFamily Error = new ColorFamily() { "Window_Error", "Error" };

        #endregion

        protected override void Draw()
        {
            base.Draw();

            View.FillColorScheme(Style.GetColorScheme(Base));

            if (Content != null)
                View.Merge(Content.View.Visual, 0, 0);

            if (IsMessageEnabled && CurrentMessage != null)
            {
                DrawMessage(CurrentMessage);
            }
        }

        private void DrawMessage(Message m)
        {
            View v = new View(Width, 1);

            switch (m.Type)
            {
                case Message.MessageType.MESSAGE:
                    v.FillColorScheme(Style.GetColorScheme(MessageDisp));
                    v.SetText("  >  " + m.Text);
                    break;
                case Message.MessageType.INFO:
                    v.FillColorScheme(Style.GetColorScheme(Info));
                    v.SetText(" i>  " + m.Text);
                    break;
                case Message.MessageType.WARNING:
                    v.FillColorScheme(Style.GetColorScheme(Warning));
                    v.SetText(" ?>  " + m.Text);
                    break;
                case Message.MessageType.ERROR:
                    v.FillColorScheme(Style.GetColorScheme(Error));
                    v.SetText(" !>  " + m.Text);
                    break;
            }

            View.Merge(v.Visual, 0, Height - 1);
        }
    }
}
