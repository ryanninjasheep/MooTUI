using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Core;
using MooTUI.Widgets.Primitives;
using MooTUI.Input;
using MooTUI.Control;

namespace MooTUI.Core
{
    public class MooInterface
    {
        public Widget Content { get; }
        private IMooViewer Viewer { get; }

        public Widget? HoveredWidget { get; private set; }
        public Widget? FocusedWidget { get; private set; }

        private (int, int) LastMouseLocation { get; set; }

        public MooInterface(IMooViewer viewer, Widget content)
        {
            content.Bind();
            Content = content;
            Content.BubbleEvent += Content_BubbleEvent;

            Viewer = viewer;
            Viewer.InputEventHandler += Viewer_InputEventHandler;
        }

        private void Content_BubbleEvent(object sender, BubblingEventArgs e)
        {
            switch (e)
            {
                case RenderEventArgs r:
                    Viewer.SetVisual(r.Origin!.Visual);
                    break;
                case ClaimFocusEventArgs f:
                    SetFocusedWidget(f.Origin!);
                    break;
                case LayoutUpdatedEventArgs _:
                    Content_LayoutUpdated();
                    break;
            }
        }

        private void Content_LayoutUpdated()
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
            HoveredWidget?.HandleInput(m);
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

        private void SetHoveredWidget(Widget? w)
        {
            if (w != HoveredWidget)
            {
                HoveredWidget?.HandleInput(new MouseLeaveInputEventArgs());
                HoveredWidget = w;
                HoveredWidget?.HandleInput(new MouseEnterInputEventArgs());
            }
        }

        private void SetFocusedWidget(Widget? w)
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
