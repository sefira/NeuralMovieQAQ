using JiebaNet.Segmenter;
using JiebaNet.Analyser;
using JiebaNet.Segmenter.PosSeg;
using System;
using System.Linq;
using QueryAnswer;
using System.Collections.Generic;

namespace C_sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string query = @"香港的刘德华出演了张艺谋和冯小刚2001年的天下无贼一部喜剧片";
            Query q = new Query(query);
            //var result = DateTime.Parse("05/11/2010").Year;
            //Console.WriteLine(result);
            Parser m_Parser = new Parser();
            m_Parser.ParseAll(ref q);
            var a = q;
        }
    }
}