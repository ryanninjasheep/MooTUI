using MooTUI.Layout;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Widgets
{
    public class SimpleTextInput : Primitives.TextInput
    {
        public int? MinWidth { get; private set; }

        public SimpleTextInput(Size width, 
            int? charLimit = null, bool canExpand = false, 
            string promptText = "")
            : base(new LayoutRect(width, new Size(1)), charLimit, promptText)
        {
            if (canExpand)
            {
                MinWidth = Width;
                TextArea.TextChanged += TextArea_TextChanged;
            }
        }

        public event EventHandler Submit;

        protected override bool HandleEnter()
        {
            OnSubmit(EventArgs.Empty);
            return true;
        }

        private void TextArea_TextChanged(object sender, EventArgs e)
        {
            if (Bounds.WidthData is FlexSize && MinWidth is int min)
            {
                Bounds.SetSizes(
                    new FlexSize(Math.Max(Text.Length + 1, min)),
                    Bounds.HeightData);
            }
        }

        private void OnSubmit(EventArgs e)
        {
            EventHandler handler = Submit;
            handler?.Invoke(this, e);
        }
    }
}
