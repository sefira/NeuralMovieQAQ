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
            Console.ReadKey();
            // test
            //TestParser();
            TestoSearchAPI();
            //TestQueryFile(@"D:\MovieDomain\QueryAnswer\resource\userquery.txt");
            //TestTranstionStatus();
            //TestSessionFile(@"D:\MovieDomain\QueryAnswer\resource\usersession.txt");
            //TestSession();
        }

        private static void TestParser()
        {
            //string query_str = @"上世纪香港的刘德华出演了张艺谋和冯小刚2001年的天下无贼一部喜剧片";
            //string query_str = @"我想看周星驰的电影";
            string query_str = @"我想看成龙的电影";
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
                    Utils.WriteQuery(query);
                    test_dialog.TestOneTurnDialog(query);
                }
            }
        }

        private static void TestTranstionStatus()
        {
            Session session = new Session();
            session.is_considerd[ParseStatus.Artist] = true;

            DialogManager dm = new DialogManager();
            ParseStatus res = DialogManager.TestTransitionStatus(session);
            Console.WriteLine(res.ToString());

            session = new Session();
            session.is_considerd[ParseStatus.PublishDate] = true;
            
            res = DialogManager.TestTransitionStatus(session);
            Console.WriteLine(res.ToString());
        }

        private static void TestSessionFile(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            DialogManager test_dialog = new DialogManager();
            while (!sr.EndOfStream)
            {
                string query = sr.ReadLine();
                if (!string.IsNullOrEmpty(query))
                {
                    Utils.WriteQuery(query);
                    test_dialog.TestDialogFlow(query);
                }
            }
        }

        private static void TestSession()
        {
            while (true)
            {
                Utils.WriteMachine("Now, you can talk to me~");

                // start a dialog
                DialogManager movie_dialog = new DialogManager();
                movie_dialog.DialogFlow();
            }
        }
    }
}