using System;
using System.Collections.Generic;
using System.Text;
using MooTUI.Core;

namespace MooTUI.IO
{
    public static class Shaders
    {
        ///// <summary>
        ///// Factor < 1 darkens, > 1 lightens.
        ///// </summary>
        //public static Func<Cell[,], int, int, Cell> Lighten(double factor)
        //{
        //    if (factor < 0)
        //    {
        //        throw new ArgumentOutOfRangeException("Factor must be greater than 0!");
        //    }
        //    else
        //    {
        //        return (cells, x, y) =>
        //        {
        //            Color fore = cells[x, y].Fore ?? Colors.Transparent;

        //            double r = fore.R * factor > 255 ? 255 : fore.R * factor;
        //            double g = fore.G * factor > 255 ? 255 : fore.G * factor;
        //            double b = fore.B * factor > 255 ? 255 : fore.B * factor;

        //            fore = Color.FromRgb((byte)r, (byte)g, (byte)b);

        //            Color back = cells[x, y].Back ?? Colors.Transparent;

        //            r = back.R * factor > 255 ? 255 : back.R * factor;
        //            g = back.G * factor > 255 ? 255 : back.G * factor;
        //            b = back.B * factor > 255 ? 255 : back.B * factor;

        //            back = Color.FromRgb((byte)r, (byte)g, (byte)b);

        //            return new Cell(cells[x, y].Char, fore, back);
        //        };
        //    }
        //}

        /// <summary>
        /// Fills with one color.
        /// </summary>
        public static Func<Color, int, int, Color> Fill(Color fill) => 
            (cells, x, y) => fill;
    }
}