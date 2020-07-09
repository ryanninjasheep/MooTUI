using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Layout
{
    public class Size
    {
        public int ActualSize { get; protected set; }

        public Size(int size)
        {
            ActualSize = size;
        }

        public virtual void SetActualSize(int size)
        {
            ActualSize = size;
        }

        public virtual Size WithRelativeSize(int difference) => new Size(ActualSize + difference);
    }

    public class FlexSize : Size
    {
        /// <summary>
        /// The preferred length of this size object.  If actual size changes, it will be
        /// proportional to the preferred size.
        /// </summary>
        public int PreferredSize { get; private set; }
        public int Min { get; private set; }

        public FlexSize(int preferredSize, int min) : base(preferredSize)
        {
            PreferredSize = preferredSize;
            Min = min;
        }

        public override void SetActualSize(int size)
        {
            if (size < Min)
                throw new ArgumentOutOfRangeException("size", "Size cannot be less than min");

            base.SetActualSize(size);
        }

        public override Size WithRelativeSize(int difference) => 
            new FlexSize(PreferredSize + difference, Min + difference);
    }
}
