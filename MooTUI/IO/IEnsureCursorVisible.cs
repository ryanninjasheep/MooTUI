using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.IO
{
    public interface IEnsureCursorVisible
    {
        public event EventHandler<RegionEventArgs> EnsureCursorVisible;
    }

    
}
