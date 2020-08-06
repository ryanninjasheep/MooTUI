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
        public virtual Size Clone() => new Size(ActualSize);

        private void OnActualSizeChanged(EventArgs e)
        {
            EventHandler handler = ActualSizeChanged;
            handler?.Invoke(this, e);
        }
    }

    public class FlexSize : Size
    {
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

        public FlexSize(int min) : this(min, min) { }

        private FlexSize(int min, int actualSize) : base(actualSize)
        {
            Min = min;
        }

        public void Reset() => ActualSize = Min;

        public override Size WithRelativeSize(int difference) => 
            new FlexSize(Min + difference);
        public override Size Clone() => new FlexSize(Min, ActualSize);
    }
}
