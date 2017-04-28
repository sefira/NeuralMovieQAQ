using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryWordBreaker
{
    class Program
    {
        static void Main(string[] args)
        {
            //if(args.Length < 2)
            //{
            //    Console.WriteLine("QueryWordBreaker.exe {input} {output}");
            //    return;
            //}            
            string query = "007之明日帝国";
            Console.WriteLine(query.Length);
            Console.WriteLine(query[1]);
            string brokenquery = WordBreaker.GetInstance().DoBreakOnString(query);
            Console.WriteLine(brokenquery);
            //WordBreaker.GetInstance().DoBreakOnFile(args[0], args[1], Markets.zhCN);
        }
    }
}
