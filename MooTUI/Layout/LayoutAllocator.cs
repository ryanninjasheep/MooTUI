using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MooTUI.Layout
{
    static class LayoutAllocator
    {
        public static void AllocateSizes(int containerSize, Orientation orientation, IEnumerable<LayoutRect> objects)
        {
            AllocateSizes(containerSize, objects.Select(
                (o) => orientation switch
                {
                    Orientation.Horizontal => o.WidthData,
                    Orientation.Vertical => o.HeightData,
                    _ => throw new NotImplementedException(),
                }
            ));
        }

        private static void AllocateSizes(int containerSize, IEnumerable<Size> sizes)
        {
            if (!AssertMinSizesFit(containerSize, sizes))
                throw new InvalidOperationException("The given objects cannot fit!");

            List<FlexSize> flexible = sizes.OfType<FlexSize>().ToList();

            float totalPreferred = flexible.Sum((f) => f.PreferredSize);

            int freeSpace = sizes.Sum((s) => s.ActualSize);
            do
            {
                if (freeSpace < 0)
                    flexible.Where((f) => f.ActualSize > f.Min);

                foreach (FlexSize f in flexible)
                {
                    float sizeRatio = f.PreferredSize / totalPreferred;
                    float growth = sizeRatio * freeSpace;
                    int newSize = Math.Max(f.ActualSize + (int)Math.Round(growth), f.Min);
                    f.SetActualSize(newSize);
                }

                freeSpace = sizes.Sum((s) => s.ActualSize);
            } while (freeSpace != 0 && flexible.Count > 0);
        }

        private static bool AssertMinSizesFit(int containerSize, IEnumerable<Size> sizes)
        {
            int min = 0;
            foreach (Size s in sizes)
            {
                if (s is FlexSize f)
                    min += f.Min;
                else
                    min += s.ActualSize;
            }

            return min <= containerSize;
        }
    }
}
