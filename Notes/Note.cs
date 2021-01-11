using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Text;

namespace Notes
{
    public class Note
    {
        private bool isExpanded = true;

        private TextSpan text { get; set; }
        private List<Note> children { get; set; }

        public bool HasChildren => children.Count > 0;

        public bool IsExpanded => HasChildren ? isExpanded : false;
    }
}
