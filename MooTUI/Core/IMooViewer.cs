using System;
using System.Windows;
using MooTUI.Drawing;
using MooTUI.Input;
using System.ComponentModel;
using System.Windows.Navigation;

namespace MooTUI.Core
{
    public interface IMooViewer
    {
        void SetVisual(Visual v);

        event EventHandler<InputEventArgs> InputEventHandler;
    }
}
