using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Widgets.Primitives;

namespace MooTUI.IO
{
    /// <summary>
    /// Used for bubbling messages to parent window.
    /// </summary>
    public class Message
    {
        public enum MessageType
        {
            MESSAGE,
            INFO,
            WARNING,
            ERROR,
        }

        public MessageType Type { get; private set; }
        public string Text { get; private set; }
        public Widget Sender { get; private set; }

        public Message(MessageType type, string message, Widget sender)
        {
            Type = type;
            Text = message;
            Sender = sender;
        }
    }
}
