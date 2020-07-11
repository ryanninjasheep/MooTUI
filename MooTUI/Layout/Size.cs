using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Layout
{
    public class Size
    {
        public int ActualSize { get; set; }

        public Size(int size)
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

        public FlexSize(int preferredSize, int min) : this(preferredSize, min, preferredSize) { }

        private FlexSize(int preferredSize, int min, int actualSize) : base(actualSize)
        {
            PreferredSize = preferredSize;
            Min = min;
        }

        public FlexSize WithActualSize(int size)
        {
            if (size < Min)
                throw new ArgumentOutOfRangeException("size", "Size cannot be less than min");

            return new FlexSize(PreferredSize, Min, size);
        }

        public override Size WithRelativeSize(int difference) => 
            new FlexSize(PreferredSize + difference, Min + difference);
    }
}
