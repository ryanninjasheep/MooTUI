using System;
using MooTUI.Drawing;
using System.Windows;
using Media = System.Windows.Media;
using SystemInput = System.Windows.Input;
using System.ComponentModel;
using System.Windows.Navigation;
using MooTUI.Input;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace MooTUI.Core.WPF
{
    /// <summary>
    /// Intermediary between WPF and MooTUI
    /// </summary>
    public class WPFBitmapViewer : WPFMooViewer
    {
        private Dictionary<char, byte[]> glyphTypeface;
        private int rawStride;

        private static Media.PixelFormat pixelFormat = Media.PixelFormats.Indexed1;

        //  ┌─┐  ┬  ╔═╗  ╤ 	 ╦   ╷
        //	│ │ ├┼┤ ║ ║ ╟┼╢ ╠╬╣ ╶┼╴  \0
        //  └─┘  ┴  ╚═╝  ╧   ╩   ╵
        private static readonly char[] MasterIndexList = new char[]
        {
            ' ',  '☺',  '☻',  '♥',  '♦',  '♣',  '♠',  '●',  '○',  '◆',  '◇',  '˃',  '˂',  '˄',  '˅',  '⛭',
            '\0', '…', '↕',  '‼',  '⁋',  '§',  '|',  '↨',  '↑',  '↓',  '←',  '→',  '\0',  '↔',  '\0', '\0',
            '\0', '!',  '"',  '#',  '$',  '%',  '&',  '\'', '(',  ')',  '*',  '+',  ',',  '-',  '.',  '/',
            '0',  '1',  '2',  '3',  '4',  '5',  '6',  '7',  '8',  '9',  ':',  ';',  '<',  '=',  '>',  '?',
            '@',  'A',  'B',  'C',  'D',  'E',  'F',  'G',  'H',  'I',  'J',  'K',  'L',  'M',  'N',  'O',
            'P',  'Q',  'R',  'S',  'T',  'U',  'V',  'W',  'X',  'Y',  'Z',  '[',  '\\',  ']',  '^',  '_',
            '`',  'a',  'b',  'c',  'd',  'e',  'f',  'g',  'h',  'i',  'j',  'k',  'l',  'm',  'n',  'o',
            'p',  'q',  'r',  's',  't',  'u',  'v',  'w',  'x',  'y',  'z',  '{',  '¦',  '}',  '~',  '⌂',
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0', '\0',  '\0',
            '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '¿',  '\0',  '\0',  '½',  '¼',  '¡',  '«',  '»',
            '░',  '▒',  '▓',  '│',  '┤',  '╡',  '╢',  '╖',  '╕',  '╣',  '║',  '╗',  '╝',  '╜',  '╛',  '┐',
            '└',  '┴',  '┬',  '├',  '─',  '┼',  '╞',  '╟',  '╚',  '╔',  '╩',  '╦',  '╠',  '═',  '╬',  '╧',
            '╨',  '╤',  '╥',  '╙',  '╘',  '╒',  '╓',  '╫',  '╪',  '┘',  '┌',  '█',  '▄',  '▌',  '▐',  '▀',
            '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0', '\0',  '\0',
            '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0',  '\0', '\0',  '\0',
            '\0',
        };

        private static Dictionary<char, byte[]> GenerateTypeface(Uri bitmapPath, 
            int glyphWidth, int glyphHeight, int rawStride)
        {
            Dictionary<char, byte[]> toReturn = new Dictionary<char, byte[]>();

            // Assumes bitmap is of proper pixelformat (Indexed1)
            PngBitmapDecoder b = new PngBitmapDecoder(bitmapPath, 
                BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
            BitmapFrame sheet = b.Frames[0];
            int imageIndex = 0;
            byte[] rawImage = new byte[rawStride * glyphHeight];
            for (int j = 0; j < sheet.PixelHeight; j += glyphHeight)
            {
                for (int i = 0; i < sheet.PixelWidth; i += glyphWidth)
                {
                    sheet.CopyPixels(new Int32Rect(i, j, glyphWidth, glyphHeight), rawImage, rawStride, 0);
                    toReturn[MasterIndexList[imageIndex]] = (byte[])rawImage.Clone();
                    imageIndex++;
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Width and height in terms of CELLS, not pixels.
        /// </summary>
        public WPFBitmapViewer(int width, int height, Theme theme) : base(width, height, theme)
        {
            glyphTypeface = LoadTypeface(
                new Uri("C:\\Users\\ryann\\source\\repos\\MooTUI\\MooTUI_Test\\12x16.png", 
                UriKind.Relative), 
                12, 16, width, height);
        }

        private Dictionary<char, byte[]> LoadTypeface(Uri bitmapPath, int glyphWidth, int glyphHeight, int width, int height)
        {
            CellWidth = glyphWidth;
            CellHeight = glyphHeight;

            SetSize(width, height);
            rawStride = (CellWidth * pixelFormat.BitsPerPixel + 7) / 8;
            return GenerateTypeface(bitmapPath, CellWidth, CellHeight, rawStride);
        }

        protected override void Render(Media.DrawingContext dc)
        {
            if (Visual is null)
                return;

            for (int j = 0; j < Visual.Height; j++) // Go row by row
            {
                for (int i = 0; i < Visual.Width; i++)
                {
                    DrawGlyph(i, j, Visual[i, j], dc);
                }
            }
        }
        private void DrawGlyph(int x, int y, Cell c, Media.DrawingContext dc)
        {
            List<Media.Color> palette = new List<Media.Color>();
            palette.Add(GetColor(c.Back));
            palette.Add(GetColor(c.Fore));
            BitmapPalette p = new BitmapPalette(palette);

            BitmapSource bitmap = BitmapSource.Create(CellWidth, CellHeight,
                96, 96, pixelFormat, p,
                glyphTypeface[c.Char ?? ' '], rawStride);

            dc.DrawImage(bitmap, new Rect(x * CellWidth, y * CellHeight, CellWidth, CellHeight));
        }
    }
}
