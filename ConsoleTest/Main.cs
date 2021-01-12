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
            c.ReadLine();
        }
    }
}
