using MooTUI.Layout;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets
{
    public class MultilineTextInput : Primitives.TextInput
    {
        public int? MinHeight { get; private set; }

        public MultilineTextInput(LayoutRect bounds,
            int? charLimit = null, bool canExpand = false,
            string promptText = "")
            : base(bounds, charLimit, promptText)
        {
            if (canExpand)
            {
                MinHeight = Height;
                TextArea.HeightChanged += TextArea_HeightChanged;
            }
        }

        protected override bool HandleEnter()
        {
            Write("\n");
            return true;
        }

        private void TextArea_HeightChanged(object sender, EventArgs e)
        {
            if (MinHeight is int min)
            {
                Bounds.TryResize(Width, Math.Max(TextArea.Draw().Height + 1, min));
            }
        }
    }
}
