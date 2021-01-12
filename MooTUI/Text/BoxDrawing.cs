using MooTUI.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Text
{
    public class BoxDrawing
    {
        public char UL { get; private set; }
        public char UD { get; private set; }
        public char UR { get; private set; }
        public char DL { get; private set; }
        public char DR { get; private set; }
        public char LR { get; private set; }

        public static BoxDrawing Default;

        public static readonly BoxDrawing Square = new BoxDrawing
        {
            UL = '┘',
            UD = '│',
            UR = '└',
            DL = '┐',
            DR = '┌',
            LR = '─'
        };
        public static readonly Lazy<BoxDrawing> Double = new Lazy<BoxDrawing>(() => new BoxDrawing
        {
            UL = '╝',
            UD = '║',
            UR = '╚',
            DL = '╗',
            DR = '╔',
            LR = '═'
        });
        public static readonly Lazy<BoxDrawing> Rounded = new Lazy<BoxDrawing>(() => new BoxDrawing
        {
            UL = '╯',
            UD = '│',
            UR = '╰',
            DL = '╮',
            DR = '╭',
            LR = '─'
        });
        public static readonly Lazy<BoxDrawing> Leaf = new Lazy<BoxDrawing>(() => new BoxDrawing
        {
            UL = '╯',
            UD = '│',
            UR = '└',
            DL = '┐',
            DR = '╭',
            LR = '─'
        });
        public static readonly Lazy<BoxDrawing> BubbleLeft = new Lazy<BoxDrawing>(() => new BoxDrawing
        {
            UL = '╯',
            UD = '│',
            UR = '└',
            DL = '╮',
            DR = '╭',
            LR = '─'
        });
        public static readonly Lazy<BoxDrawing> BubbleRight = new Lazy<BoxDrawing>(() => new BoxDrawing
        {
            UL = '┘',
            UD = '│',
            UR = '╰',
            DL = '╮',
            DR = '╭',
            LR = '─'
        });

        static BoxDrawing()
        {
            Default = Square;
        }

        public void DrawBox(Visual v, int width, int height, int xOffset = 0, int yOffset = 0)
        {
            v.FillChar(LR, xOffset + 1,         yOffset,              width - 2, 1);
            v.FillChar(UD, xOffset,             yOffset + 1,          1,         height - 2);
            v.FillChar(LR, xOffset + 1,         yOffset + height - 1, width - 2, 1);
            v.FillChar(UD, xOffset + width - 1, yOffset + 1,          1,         height - 2);

            v.SetChar(xOffset,             yOffset,              DR);
            v.SetChar(xOffset + width - 1, yOffset,              DL);
            v.SetChar(xOffset,             yOffset + height - 1, UR);
            v.SetChar(xOffset + width - 1, yOffset + height - 1, UL);
        }
    }
}
