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

    public static class VisualShaderExtensions
    {
        /// <summary>
        /// Applies a particular function to all cells in the View.
        /// </summary>
        public static void ApplyShader(this Visual v, Func<Color, int, int, Color> shader,
            bool affectFore, bool affectBack) =>
            v.ApplyShader(shader, affectFore, affectBack, 0, 0, v.Width, v.Height);
        /// <summary>
        /// Applies a particular function to a certain range of cells in the View.
        /// </summary>
        public static void ApplyShader(this Visual v, Func<Color, int, int, Color> shader,
            bool affectFore, bool affectBack,
            int xStart, int yStart, int width, int height)
        {
            for (int i = xStart; i < v.Width && i - xStart < width; i++)
            {
                for (int j = yStart; j < v.Height && j - yStart < height; j++)
                {
                    Cell shaded = v[i, j];

                    if (affectFore)
                        shaded = shaded.WithFore(shader(shaded.Fore, i, j));

                    if (affectBack)
                        shaded = shaded.WithBack(shader(shaded.Back, i, j));

                    v[i, j] = shaded;
                }
            }
        }
    }
}