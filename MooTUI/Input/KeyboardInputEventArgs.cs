using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MooTUI.Input
{
    public class KeyboardInputEventArgs : InputEventArgs
    {
        public Key Key { get; }

        public char? Char { get; }

        public KeyboardInputEventArgs(Key key)
        {
            Key = key;
            Char = TryGetChar(Key);
        }

        public char? TryGetChar(Key key)
        {
            if (Ctrl || Alt)
                return null;

            bool capLetter = Shift ^ Caps;
            switch (key)
            {
                case Key.D0:
                    if (Shift)
                        return ')';
                    else
                        return '0';
                case Key.D1:
                    if (Shift)
                        return '!';
                    else
                        return '1';
                case Key.D2:
                    if (Shift)
                        return '@';
                    else
                        return '2';
                case Key.D3:
                    if (Shift)
                        return '#';
                    else
                        return '3';
                case Key.D4:
                    if (Shift)
                        return '$';
                    else
                        return '4';
                case Key.D5:
                    if (Shift)
                        return '%';
                    else
                        return '5';
                case Key.D6:
                    if (Shift)
                        return '^';
                    else
                        return '6';
                case Key.D7:
                    if (Shift)
                        return '&';
                    else
                        return '7';
                case Key.D8:
                    if (Shift)
                        return '*';
                    else
                        return '8';
                case Key.D9:
                    if (Shift)
                        return '(';
                    else
                        return '9';
                case Key.A:
                    if (capLetter)
                        return 'A';
                    else
                        return 'a';
                case Key.B:
                    if (capLetter)
                        return 'B';
                    else
                        return 'b';
                case Key.C:
                    if (capLetter)
                        return 'C';
                    else
                        return 'c';
                case Key.D:
                    if (capLetter)
                        return 'D';
                    else
                        return 'd';
                case Key.E:
                    if (capLetter)
                        return 'E';
                    else
                        return 'e';
                case Key.F:
                    if (capLetter)
                        return 'F';
                    else
                        return 'f';
                case Key.G:
                    if (capLetter)
                        return 'G';
                    else
                        return 'g';
                case Key.H:
                    if (capLetter)
                        return 'H';
                    else
                        return 'h';
                case Key.I:
                    if (capLetter)
                        return 'I';
                    else
                        return 'i';

                case Key.J:
                    if (capLetter)
                        return 'J';
                    else
                        return 'j';
                case Key.K:
                    if (capLetter)
                        return 'K';
                    else
                        return 'k';
                case Key.L:
                    if (capLetter)
                        return 'L';
                    else
                        return 'l';
                case Key.M:
                    if (capLetter)
                        return 'M';
                    else
                        return 'm';
                case Key.N:
                    if (capLetter)
                        return 'N';
                    else
                        return 'n';
                case Key.O:
                    if (capLetter)
                        return 'O';
                    else
                        return 'o';
                case Key.P:
                    if (capLetter)
                        return 'P';
                    else
                        return 'p';
                case Key.Q:
                    if (capLetter)
                        return 'Q';
                    else
                        return 'q';
                case Key.R:
                    if (capLetter)
                        return 'R';
                    else
                        return 'r';
                case Key.S:
                    if (capLetter)
                        return 'S';
                    else
                        return 's';
                case Key.T:
                    if (capLetter)
                        return 'T';
                    else
                        return 't';
                case Key.U:
                    if (capLetter)
                        return 'U';
                    else
                        return 'u';
                case Key.V:
                    if (capLetter)
                        return 'V';
                    else
                        return 'v';
                case Key.W:
                    if (capLetter)
                        return 'W';
                    else
                        return 'w';
                case Key.X:
                    if (capLetter)
                        return 'X';
                    else
                        return 'x';
                case Key.Y:
                    if (capLetter)
                        return 'Y';
                    else
                        return 'y';
                case Key.Z:
                    if (capLetter)
                        return 'Z';
                    else
                        return 'z';
                case Key.NumPad0:
                    return '0';
                case Key.NumPad1:
                    return '1';
                case Key.NumPad2:
                    return '2';
                case Key.NumPad3:
                    return '3';
                case Key.NumPad4:
                    return '4';
                case Key.NumPad5:
                    return '5';
                case Key.NumPad6:
                    return '6';
                case Key.NumPad7:
                    return '7';
                case Key.NumPad8:
                    return '8';
                case Key.NumPad9:
                    return '9';
                case Key.OemPlus:
                    if (Shift)
                        return '+';
                    else
                        return '=';
                case Key.OemComma:
                    if (Shift)
                        return '<';
                    else
                        return ',';
                case Key.OemMinus:
                    if (Shift)
                        return '_';
                    else
                        return '-';
                case Key.OemPeriod:
                    if (Shift)
                        return '>';
                    else
                        return '.';
                case Key.OemQuestion:
                    if (Shift)
                        return '?';
                    else
                        return '/';
                case Key.OemTilde:
                    if (Shift)
                        return '~';
                    else
                        return '`';
                case Key.OemPipe:
                    if (Shift)
                        return '|';
                    else
                        return '\\';
                case Key.OemCloseBrackets:
                    if (Shift)
                        return '}';
                    else
                        return ']';
                case Key.OemOpenBrackets:
                    if (Shift)
                        return '{';
                    else
                        return '[';
                case Key.OemQuotes:
                    if (Shift)
                        return '\"';
                    else
                        return '\'';
                case Key.OemSemicolon:
                    if (Shift)
                        return ':';
                    else
                        return ';';
                case Key.Space:
                    return ' ';
                default:
                    return null;
            }
        }
    }
}
