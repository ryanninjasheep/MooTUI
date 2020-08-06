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
            protected set
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
        private int _min;

        public int Min 
        { 
            get => _min;
            private set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than 0.");

                _min = value;
            }
        }

        public override int ActualSize
        {
            get => base.ActualSize;
            protected set
            {
                if (value < Min)
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than min.");

                base.ActualSize = value;
            }
        }

        public FlexSize(int min) : this(min, min) { }
        public FlexSize(int min, int actualSize) : base(actualSize)
        {
            Min = min;
        }

        public override Size WithRelativeSize(int difference) =>
            new FlexSize(Min + difference);
        public override Size Clone() => new FlexSize(Min, ActualSize);

        public void TryResize(int newSize) => ActualSize = Math.Max(Min, newSize);

        public void Reset() => ActualSize = Min;

        public void SetMin(int newMin)
        {
            Min = newMin;
            Reset();
        }
    }
}
