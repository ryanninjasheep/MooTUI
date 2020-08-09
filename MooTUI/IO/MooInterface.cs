using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Core;
using MooTUI.Widgets.Primitives;
using MooTUI.Input;

namespace MooTUI.IO
{
    public class MooInterface
    {
        public Widget Content { get; private set; }
        private IMooViewer Viewer { get; set; }

        public Widget HoveredWidget { get; private set; }
        public Widget FocusedWidget { get; private set; }

        private (int, int) LastMouseLocation { get; set; }

        public MooInterface(IMooViewer viewer, Widget content)
        {
            SetContent(content);
            SetViewer(viewer);
        }

        public void SetContent(Widget w)
        {
            if (Content != null)
            {
                Content.Release();

                Content.Rendered -= Content_RenderEventHandler;
                Content.BubbleFocus -= Content_ClaimFocus;
                Content.LayoutUpdated -= Content_LayoutUpdated;
            }

            w.Bind();

            Content = w;
            Content.Rendered += Content_RenderEventHandler;
            Content.BubbleFocus += Content_ClaimFocus;
            Content.LayoutUpdated += Content_LayoutUpdated;
        }

        private void SetViewer(IMooViewer v)
        {
            Viewer = v;
            Viewer.InputEventHandler += Viewer_InputEventHandler;
        }

        private void Content_RenderEventHandler(object sender, System.EventArgs e)
        {
            Viewer.SetVisual(Content.Visual);
        }

        private void Content_ClaimFocus(object sender, EventArgs.FocusEventArgs e)
        {
            SetFocusedWidget(e.Sender);
        }

        private void Content_LayoutUpdated(object sender, System.EventArgs e)
        {
            SetHoveredWidget(GetHoveredWidget(new MouseMoveInputEventArgs(LastMouseLocation)));
            SetFocusedWidget(null);
        }

        private void Viewer_InputEventHandler(object sender, InputEventArgs e)
        {
            if (e is MouseInputEventArgs m)
            {
                HandleMouseMovement(m);
            }
            else if (e is MouseLeaveInputEventArgs l)
            {
                HoveredWidget?.HandleInput(l);
                SetHoveredWidget(null);
            }
            else if (e is KeyboardInputEventArgs k)
            {
                FocusedWidget?.HandleInput(k);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void HandleMouseMovement(MouseInputEventArgs m)
        {
            SetHoveredWidget(GetHoveredWidget(m));
            HoveredWidget.HandleInput(m);
        }

        private Widget GetHoveredWidget(MouseInputEventArgs m)
        {
            Widget hovered = Content;
            while (hovered is Container c)
            {
                hovered = c.GetHoveredWidget(m.Location);
                if (c == hovered)
                    break;
                (int x, int y) offset = c.GetChildOffset(hovered);
                m.SetRelativeMouse((-offset.x, -offset.y));
            }

            return hovered;
        }

        private void SetHoveredWidget(Widget w)
        {
            if (w != HoveredWidget)
            {
                HoveredWidget?.HandleInput(new MouseLeaveInputEventArgs());
                HoveredWidget = w;
                HoveredWidget?.HandleInput(new MouseEnterInputEventArgs());
            }
        }

        private void SetFocusedWidget(Widget w)
        {
            if (w != FocusedWidget)
            {
                FocusedWidget?.HandleInput(new UnfocusInputEventArgs());
                FocusedWidget = w;
                FocusedWidget?.HandleInput(new FocusInputEventArgs());
            }
        }
    }
}
