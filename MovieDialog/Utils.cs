//#define LOCAL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MovieDialog
{
    class Utils
    {
        public static string ReadLine()
        {
            string query = "";
            while (string.IsNullOrWhiteSpace(query))
            {
#if (LOCAL)
                query = Console.ReadLine();
#else
                // before bot waiting for http, bot must return all response
                // so http server can fetch data
                DialogHttpServer.bot_response_sem.Release();
                // wait http server get user query
                DialogHttpServer.user_request_sem.WaitOne();
                query = DialogHttpServer.user_query;
                Console.WriteLine(query);
#endif
            }
            return query;
        }

        public static void WriteQuery(string str)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(str);
            Console.ResetColor();
#if LOCAL
#else
            DialogHttpServer.dialog_history.Add($"User: {str}");
#endif
        }

        public static void WriteMachine(string str)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(str);
            Console.ResetColor();
#if LOCAL
#else
            DialogHttpServer.dialog_history.Add($"Bot: {str}");
#endif
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
#if LOCAL
#else
            DialogHttpServer.dialog_history.Add($"Bot-Error: {str}");
#endif
        }
    }
}
