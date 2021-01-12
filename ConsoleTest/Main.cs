using MooTUI.Drawing;
using MooTUI.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M = MooTUI.Console;

namespace ConsoleTest
{
    class ConsoleTest
    {
        public static void Main(string[] args)
        {
            M.Console c = new M.Console(80, 20);

            string s = c.ReadLine();
            c.WriteLine(new TextSpan(s, new ColorPair(Color.Red, Color.None)));
            c.ReadLine();
            c.WriteLineParse("{green/}Hello, {/altcyan}World!");
        }
    }
}
