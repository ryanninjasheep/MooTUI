using System.ComponentModel;

namespace MooTUI.Layout
{
    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public enum HJustification
    {
        LEFT,
        CENTER,
        RIGHT
    }

    public enum VJustification
    {
        TOP,
        CENTER,
        BOTTOM
    }

    public static class JustificationHelper
    {
        public static int GetOffset(this HJustification j, int size, int containerSize) => j switch
        {
            HJustification.LEFT => 0,
            HJustification.CENTER => (containerSize - size) / 2,
            HJustification.RIGHT => containerSize - size,
            _ => throw new InvalidEnumArgumentException(),
        };
        public static int GetOffset(this VJustification j, int size, int containerSize) => j switch
        {
            VJustification.TOP => 0,
            VJustification.CENTER => (containerSize - size) / 2,
            VJustification.BOTTOM => containerSize - size,
            _ => throw new InvalidEnumArgumentException(),
        };
    }
}