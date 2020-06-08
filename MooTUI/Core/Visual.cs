using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MooTUI.Core
{
    public class Visual
    {
        public int Width { get; }
        public int Height { get; }

        protected Color[,] ForeColors { get; set; }
        protected Color[,] BackColors { get; set; }
        protected char[,] Chars { get; set; }

        public Visual(int width, int height)
        {
            Width = width;
            Height = height;

            ForeColors = new Color[Width, Height];
            BackColors = new Color[Width, Height];
            Chars = new char[Width, Height];
        }

        public void FillForeColor(Color fill)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    ForeColors[i, j] = fill;
                }
            }
        }
        public void FillBackColor(Color fill)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    BackColors[i, j] = fill;
                }
            }
        }
        public void FillChar(char fill)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Chars[i, j] = fill;
                }
            }
        }

        public void SetForeColor(int x, int y, Color c)
        {
            if (AssertPointWithinBounds(x, y))
                ForeColors[x, y] = c;
        }
        public void SetBackColor(int x, int y, Color c)
        {
            if (AssertPointWithinBounds(x, y))
                BackColors[x, y] = c;
        }
        public void SetChar(int x, int y, char c)
        {
            if (AssertPointWithinBounds(x, y))
                Chars[x, y] = c;
        }

        public Color GetForeColor(int x, int y)
        {
            AssertPointWithinBounds(x, y);
            return ForeColors[x, y];
        }
        public Color GetBackColor(int x, int y)
        {
            AssertPointWithinBounds(x, y);
            return BackColors[x, y];
        }
        public char GetChar(int x, int y)
        {
            AssertPointWithinBounds(x, y);
            return Chars[x, y];
        }

        public Color[,] GetForeColors() => ForeColors;
        public Color[,] GetBackColors() => BackColors;
        public char[,] GetChars() => Chars;

        private bool AssertPointWithinBounds(int x, int y)
        {
            if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
            {
                return true;
            }
            else
            {
                throw new IndexOutOfRangeException("That index is not present in the Visual");
            }
        }
    }
}
