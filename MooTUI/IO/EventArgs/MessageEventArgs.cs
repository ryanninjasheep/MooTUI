using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.IO
{
    internal class MessageEventArgs : EventArgs
    {
        public Message Message { get; }

        public MessageEventArgs(Message message)
        {
            Message = message;
        }
    }
}
