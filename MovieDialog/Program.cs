using System;
using System.Collections.Generic;
using System.IO;
using FanoutSearch;
using FanoutSearch.LIKQ;
using static FanoutSearch.LIKQ.KnowledgeGraph;
using Action = FanoutSearch.Action;
using Trinity;
using FanoutSearch.Protocols.TSL;

namespace MovieDialog
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.ReadKey();
            // test
            //TestParser();
            //TestoSearchAPI();
            //TestQueryFile(@"D:\MovieDomain\MovieDialog\Resources\userquery.txt");
            //TestTranstionStatus();
            //TestSessionFile(@"D:\MovieDomain\MovieDialog\Resources\usersession.txt");
            //TestSession();

            //TestLIKQClient();
            //TestPatternBased(@"D:\MovieDomain\MovieDialog\Resources\QA_pattern_qa.txt", @"D:\MovieDomain\MovieDialog\Resources\QA_pattern_output.txt");
            //TestCNNBased(@"D:\MovieDomain\MovieDialog\Resources\QA_pattern_qa.txt", @"D:\MovieDomain\MovieDialog\Resources\QA_pattern_output.txt");
            //TestGraphEngineQuery();
            //TestGraphEngineQA();

            TestDialogServer();
        }

        #region test jieba
        private static void TestParser()
        {
            //string query_str = @"上世纪香港的刘德华出演了张艺谋和冯小刚2001年的天下无贼一部喜剧片";
            //string query_str = @"我想看周星驰的电影";
            //string query_str = @"我想看洛克";
            //string query_str = @"我想看洛克王国2";
            string query_str = @"我演员大话西游之月光宝盒";
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
            DialogManager dm = new DialogManager();
            session.is_considerd[ParseStatus.Artist] = true;
            ParseStatus res = dm.GetTransitionStatus(session);
            Console.WriteLine(res.ToString());

            session = new Session();
            dm = new DialogManager();
            session.is_considerd[ParseStatus.PublishDate] = true;
            res = dm.GetTransitionStatus(session);
            Console.WriteLine(res.ToString());
        }
        #endregion

        #region test session
        private static void TestSessionFile(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            while (!sr.EndOfStream)
            {
                string query = sr.ReadLine();
                if (!string.IsNullOrEmpty(query))
                {
                    DialogManager test_dialog = new DialogManager();
                    Utils.WriteQuery(query);
                    test_dialog.DialogFlow(query);
                }
            }
        }

        private static void TestSession()
        {
            while (true)
            {
                // start a dialog
                DialogManager movie_dialog = new DialogManager();
                movie_dialog.DialogFlow(null);
            }
        }
        #endregion

        #region test QA parse
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

        private static void TestCNNBased(string question_filename, string output_filename)
        {
            StreamReader sr = new StreamReader(question_filename);
            CNNBased cb = new CNNBased();
            StreamWriter sw = new StreamWriter(output_filename);
            Parser parse = new Parser();
            while (!sr.EndOfStream)
            {
                PatternResponse pr = new PatternResponse();
                string line = sr.ReadLine();
                string question = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)[0];
                Query query = new Query(question);
                parse.PosTagging(ref query);
                if (cb.QuestionClassify(query, out pr))
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
        #endregion

        #region Graph Engine
        private static void TestLIKQClient()
        {
            FanoutSearchModule.ForceRunAsClient(true);
            FanoutSearchModule.RegisterExpressionSerializerFactory(() => new ExpressionSerializer());
            var paths = StartFrom(2391729982219739490, new[] { "Name" });
            foreach (var path in paths)
            {
                foreach (var node in path)
                {
                    Console.WriteLine($"{node.id}: {node["Name"]}");
                }
            }
        }

        private static void TestGraphEngineQuery()
        {
            GraphEngineQuery graphengine_query = new GraphEngineQuery();
            List<object> res = graphengine_query.GetGraphEngineData("墨攻", "PublishDate", 0);
            Console.WriteLine(string.Join(",", res.ToArray()));
            Console.WriteLine();
            res = graphengine_query.GetGraphEngineData("墨攻", "Rating", 0);
            Console.WriteLine(string.Join(",", res.ToArray()));
            Console.WriteLine();
            res = graphengine_query.GetGraphEngineData("墨攻", "Genres", 0);
            Console.WriteLine(string.Join(",", res.ToArray()));
            Console.WriteLine();
            res = graphengine_query.GetGraphEngineData("墨攻", "Country", 0);
            Console.WriteLine(string.Join(",", res.ToArray()));
            Console.WriteLine();
            res = graphengine_query.GetGraphEngineData("墨攻", "Description", 0);
            Console.WriteLine(string.Join(",", res.ToArray()));
            Console.WriteLine();
            res = graphengine_query.GetGraphEngineData("墨攻", "Artists:Name", 1);
            Console.WriteLine(string.Join(",", res.ToArray()));
            Console.WriteLine();
            res = graphengine_query.GetGraphEngineData("墨攻", "Directors:Name", 1);
            Console.WriteLine(string.Join(",", res.ToArray()));
            Console.WriteLine();
            res = graphengine_query.GetGraphEngineData("刘德华", "Act:Name", 1);
            Console.WriteLine(string.Join(",", res.ToArray()));
            Console.WriteLine();
            res = graphengine_query.GetGraphEngineData("张艺谋", "Direct:Name", 1);
            Console.WriteLine(string.Join(",", res.ToArray()));
        }

        private static void TestGraphEngineQA()
        {
            //string question = "肯尼思·洛纳根导演过哪些电影"; // 肯尼思·洛纳根 splited
            //string question = "你的名字是哪个国家拍的"; // 你的名字 in NER, but 你的名字。in CellID
            //string question = "十二怒汉是讲什么的"; // have no 十二怒汉
            //string question = "活着是讲什么的";
            //string question = "你的名字。是讲什么的";
            //string question = "赌神是讲什么的";
            //string question = "天下无贼是谁导演的";
            //string question = "林家栋拍过什么电影";  //拍 act？ direct？
            string question = "大话西游之大圣娶亲是什么时候拍的";
            Parser parser = new Parser();
            GraphEngineQuery graphengine_query = new GraphEngineQuery();
            PatternBased pattern_qa = new PatternBased();
            Query query = new Query(question);
            parser.PosTagging(ref query);
            parser.ParseAllTag(ref query);

            PatternResponse pattern_response;
            if (pattern_qa.QuestionClassify(query, out pattern_response))
            {
                string question_topic = "";
                switch (pattern_response.entity_type)
                {
                    case EntityType.Movie:
                        question_topic = query.carried_movie[0];
                        break;
                    case EntityType.Celebrity:
                        question_topic = (query.carried_artist.Count > 0) ? query.carried_artist[0] : query.carried_director[0];
                        break;
                }
                List<object> res = graphengine_query.GetGraphEngineData(question_topic, pattern_response.property, pattern_response.hop_num);
                Utils.WriteMachine(string.Join(",", res.ToArray()));
            }
        }
        #endregion

        #region Dialog Http Server
        private static void TestDialogServer()
        {
            DialogHttpServer dialog_server = new DialogHttpServer();
            dialog_server.WorkWithMovieDialog();
        }

        #endregion
    }
}