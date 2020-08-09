﻿using System;
using System.Windows;
using System.Windows.Media;
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
