using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Core;
using MooTUI.Widgets.Primitives;

namespace MooTUI.IO
{
    public class MooInterface
    {
        public Widget Content { get; private set; }
        private MooViewer Viewer { get; set; }

        private MouseContext MouseContext { get; set; }
        private KeyboardContext KeyboardContext { get; set; }

        public Widget HoveredWidget { get; private set; }
        public Widget FocusedWidget { get; private set; }

        public MooInterface(MooViewer viewer, Widget content)
        {
            SetContent(content);
            SetViewer(viewer);

            MouseContext = Viewer.MouseContext;
            KeyboardContext = Viewer.KeyboardContext;
        }

        public void SetContent(Widget w)
        {
            if (Content != null)
            {
                Content.Rendered -= Content_RenderEventHandler;
                Content.ClaimFocusEventHandler -= Content_ClaimFocus;
            }

            Content = w;
            Content.SetStyle(Style.SimpleLight, false); // Temp
            Content.Rendered += Content_RenderEventHandler;
            Content.ClaimFocusEventHandler += Content_ClaimFocus;
        }

        private void SetViewer(MooViewer v)
        {
            Viewer = v;
            Viewer.InputEventHandler += Viewer_InputEventHandler;
        }

        private void Content_RenderEventHandler(object sender, EventArgs e)
        {
            Viewer.SetVisual(Content.View.Visual);
            Viewer.InvalidateVisual();
        }

        private void Content_ClaimFocus(object sender, FocusEventArgs e)
        {
            SetFocusedWidget(e.Sender);
        }

        private void Viewer_InputEventHandler(object sender, InputEventArgs e)
        {
            if (e.IsMouseMovement())
            {
                HandleMouseMovement(e);
            }
            else if (e.IsHoverDependent())
            {
                HoveredWidget?.HandleInput(e);
            }
            else if (e.IsFocusDependent())
            {
                FocusedWidget?.HandleInput(e);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void HandleMouseMovement(InputEventArgs e)
        {
            Widget hovered = Content;
            while (hovered is Container c)
            {
                hovered = c.GetHoveredWidget(e.Mouse);
                if (c == hovered)
                    break;
            }
            SetHoveredWidget(hovered);
            HoveredWidget.HandleInput(e.CopyWithNewInputType(InputTypes.MOUSE_MOVE));
        }

        private void SetHoveredWidget(Widget w)
        {
            if (w != HoveredWidget)
            {
                HoveredWidget?.HandleInput(new InputEventArgs(InputTypes.MOUSE_LEAVE, MouseContext, KeyboardContext));
                HoveredWidget = w;
                HoveredWidget?.HandleInput(new InputEventArgs(InputTypes.MOUSE_ENTER, MouseContext, KeyboardContext));
            }
        }

        private void SetFocusedWidget(Widget w)
        {
            if (w != FocusedWidget)
            {
                FocusedWidget?.HandleInput(new InputEventArgs(InputTypes.UNFOCUS, MouseContext, KeyboardContext));
                FocusedWidget = w;
                FocusedWidget?.HandleInput(new InputEventArgs(InputTypes.FOCUS, MouseContext, KeyboardContext));
            }
        }
    }
}
