using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Layout
{
    public class Size
    {
        private int _actualSize;

        public virtual int ActualSize
        {
            get => _actualSize;
            set
            {
                _actualSize = value;
                OnActualSizeChanged(EventArgs.Empty);
            }
        }

        public Size(int size)
        {
            _actualSize = size;
        }

        public event EventHandler ActualSizeChanged;

        public virtual Size WithRelativeSize(int difference) => new Size(ActualSize + difference);

        private void OnActualSizeChanged(EventArgs e)
        {
            EventHandler handler = ActualSizeChanged;
            handler?.Invoke(this, e);
        }
    }

    public class FlexSize : Size
    {
        /// <summary>
        /// The preferred length of this size object.  If actual size changes, it will be
        /// proportional to the preferred size.
        /// </summary>
        public int PreferredSize { get; private set; }
        public int Min { get; private set; }

        public override int ActualSize 
        {
            get => base.ActualSize;
            set
            {
                if (value < Min)
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than min.");

                base.ActualSize = value;
            }
        }

        public FlexSize(int preferredSize, int min) : this(preferredSize, min, preferredSize) { }
        public FlexSize(int preferredSize) : this(preferredSize, preferredSize) { }

        private FlexSize(int preferredSize, int min, int actualSize) : base(actualSize)
        {
            PreferredSize = preferredSize;
            Min = min;
        }

        public void Reset() => ActualSize = PreferredSize;

        public override Size WithRelativeSize(int difference) => 
            new FlexSize(PreferredSize + difference, Min + difference);
    }
}
