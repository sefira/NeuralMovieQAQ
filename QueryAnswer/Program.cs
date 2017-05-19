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
            //Console.ReadKey();
            // test
            //TestParser();
            //TestoSearchAPI();
            //TestQueryFile(@"D:\MovieDomain\QueryAnswer\resource\userquery.txt");
            //TestTranstionStatus();
            //TestSessionFile(@"D:\MovieDomain\QueryAnswer\resource\usersession.txt");
            //TestSession();

            TestPatternBased(@"D:\MovieDomain\QueryAnswer\resource\QA_pattern_qa.txt", @"D:\MovieDomain\QueryAnswer\resource\QA_pattern_output.txt");
        }

        #region test jieba
        private static void TestParser()
        {
            string query_str = @"上世纪香港的刘德华出演了张艺谋和冯小刚2001年的天下无贼一部喜剧片";
            //string query_str = @"我想看周星驰的电影";
            //string query_str = @"我想看成龙的电影";
            Query query = new Query(query_str);
            Parser m_Parser = new Parser();
            m_Parser.PosTagging(ref query);
            m_Parser.ParseAllTag(ref query);
            foreach (var item in query.postag_pair)
            {
                Console.WriteLine(item.Word + item.Flag);
            }
        }
        #endregion

        #region test oSearch
        private static void TestoSearchAPI()
        {
            oSearchClient.TestQuery();
        }
        #endregion

        #region test single turn query
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
        #endregion

        #region test markov chain
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
        #endregion

        #region test session
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
        #endregion

        private static void TestPatternBased(string question_filename, string output_filename)
        {
            StreamReader sr = new StreamReader(question_filename);
            PatternBased pb = new PatternBased();
            StreamWriter sw = new StreamWriter(output_filename);
            Parser parse = new Parser();
            while (!sr.EndOfStream)
            {
                PatternResponse pr = new PatternResponse();
                string line = sr.ReadLine();
                string question = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)[0];
                Query query = new Query(question);
                parse.PosTagging(ref query);
                if (pb.QuestionClassify(query, out pr))
                {
                    sw.Write(question);
                    sw.Write('\t');
                    sw.WriteLine(pr.property);
                }
                else
                {
                    sw.WriteLine(question);
                }
            }
            sw.Flush();
            sw.Close();
        }
    }
}