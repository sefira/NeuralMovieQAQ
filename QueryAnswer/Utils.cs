using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryAnswer
{
    class Utils
    {
        public static void WriteQuery(string str)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(str);
            Console.ResetColor();
        }

        public static void WriteMachine(string str)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(str);
            Console.ResetColor();
        }

        public static void WriteResult(string str)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(str);
            Console.ResetColor();
        }
        public static void WriteError(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red ;
            Console.WriteLine(str);
            Console.ResetColor();
        }
    }
}
