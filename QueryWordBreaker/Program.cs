using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryWordBreaker
{
    class Program
    {
        public static bool IsChinese(string input_string)
        {
            Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
            return cjkCharRegex.IsMatch(input_string[0].ToString());
        }

        static void Main(string[] args)
        {
            string query = "三傻大闹宝莱坞";
            Console.WriteLine(IsChinese(query));
            Console.WriteLine(IsChinese(('1' + query)));
            Console.WriteLine(query[0]);
            string brokenquery = WordBreaker.GetInstance().DoBreakOnString(query);
            Console.WriteLine(brokenquery);
            //WordBreaker.GetInstance().DoBreakOnFile(args[0], args[1], Markets.zhCN);
        }
    }
}
