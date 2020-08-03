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

        private void TextArea_HeightChanged(object sender, EventArgs e)
        {
            if (Bounds.HeightData is FlexSize && MinHeight is int min)
            {
                Bounds.SetSizes(
                    Bounds.WidthData,
                    new FlexSize(Math.Max(TextArea.Draw().Height + 1, min)));
            }
        }
    }
}
