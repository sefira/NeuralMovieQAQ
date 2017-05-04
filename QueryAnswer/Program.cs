using JiebaNet.Segmenter;
using JiebaNet.Analyser;
using JiebaNet.Segmenter.PosSeg;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace QueryAnswer
{
    class Program
    {
        static void Main(string[] args)
        {
            //string query = @"上世纪香港的刘德华出演了张艺谋和冯小刚2001年的天下无贼一部喜剧片";
            //Query q = new Query(query);
            //Parser m_Parser = new Parser();
            //m_Parser.ParseAll(ref q);
            //var a = q;
            //oSearchClient.TestQuery();

            //DialogManager movie_dialog = new DialogManager();
            //movie_dialog.DialogFlow();
            TestQueryFile(@"D:\MovieDomain\QueryAnswer\resource\userquery.txt");
        }
        private static void TestQueryFile(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            DialogManager test_dialog = new DialogManager();
            while (!sr.EndOfStream)
            {
                string query = sr.ReadLine();
                if (!string.IsNullOrEmpty(query))
                {
                    Console.WriteLine(query);
                    test_dialog.DialogFlow(query);
                }
            }
        }
    }
}