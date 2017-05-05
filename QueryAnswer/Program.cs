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
            // start a dialog
            //DialogManager movie_dialog = new DialogManager();
            //movie_dialog.DialogFlow();

            // test
            //TestParser();
            //TestoSearchAPI();
            //TestQueryFile(@"D:\MovieDomain\QueryAnswer\resource\userquery.txt");
            TestTranstionStatus();
        }

        private static void TestParser()
        {
            string query_str = @"上世纪香港的刘德华出演了张艺谋和冯小刚2001年的天下无贼一部喜剧片";
            Query query = new Query(query_str);
            Parser m_Parser = new Parser();
            m_Parser.ParseAll(ref query);
            var a = query;
        }

        private static void TestoSearchAPI()
        {
            oSearchClient.TestQuery();
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

        private static void TestTranstionStatus()
        {
            Session session = new Session();
            session.is_considerd["publishdate"] = true;

            DialogManager dm = new DialogManager();
            string res = DialogManager.TestTranstionStatus(session);
            Console.WriteLine(res);
        }
    }
}