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
            M.Console c = new M.Console(120, 40, new TextSpan("CONSOLE"));

            while (true)
            {
            string s = c.ReadLine();
                c.WriteLineParse(s);
            }
        }
    }
}
