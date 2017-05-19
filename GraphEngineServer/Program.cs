using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FanoutSearch;
using Trinity;
using FanoutSearch.LIKQ;
using Newtonsoft.Json.Linq;
using Trinity.Core.Lib;
using Trinity.Network;
using static FanoutSearch.LIKQ.KnowledgeGraph;
using Action = FanoutSearch.Action;

namespace GraphEngineServer
{
    class Program
    {
        static void Main(string[] args)
        {
            FanoutSearchModule.EnableExternalQuery(true);
            FanoutSearchModule.SetQueryTimeout(30000);
            FanoutSearchModule.RegisterIndexService(Indexer);
            FanoutSearchModule.RegisterExpressionSerializerFactory(() => new ExpressionSerializer());
            LambdaDSL.SetDialect("MyGraph", "StartFrom", "VisitNode", "FollowEdge", "Action");

            TrinityConfig.HttpPort = 80;
            var server = new TrinityServer();
            server.RegisterCommunicationModule<FanoutSearchModule>();
            server.Start();

            if (!Trinity.Global.LocalStorage.LoadStorage() || !Trinity.Global.LocalStorage.Movie_Accessor_Selector().Any())
            {
                Trinity.Global.LocalStorage.LoadStorage();
                ImportMovieData(@"D:\MovieDomain\GraphEngineServer\bin\Debug\");
                Trinity.Global.LocalStorage.SaveStorage();
            }
            else
            {
                Console.WriteLine("=============================Movie had been imported once. Skipping this turn.");
            }
            TestMovieData(@"D:\MovieDomain\GraphEngineServer\bin\Debug\");

            ImportToyData();
        }

        private static IEnumerable<long> Indexer(object matchobject, string typestring)
        {
            JObject queryObj = (JObject)matchobject;
            string key = queryObj["CellId"].ToString();
            yield return long.Parse(key);
        }

        private static void ImportMovieData(string path)
        {
            MovieEntityImport movie_entity_import = new MovieEntityImport(path);
            string filename = @"data\Movie.csv";
            movie_entity_import.ImportMovie(filename);
            Console.WriteLine();
        }

        private static void TestMovieData(string path)
        {
            Global.LocalStorage.LoadStorage();
            Console.WriteLine(Global.LocalStorage.Movie_Accessor_Selector().Count());

            MovieEntityImport movie_entity_import = new MovieEntityImport(path);
            List<long> name_ids = new List<long>();
            name_ids.Add(movie_entity_import.celebrity_cellid["刘德华"]);
            //List<long> name_ids = Index.Person_Name_SubstringQuery("刘德华");
            //Console.WriteLine(name_ids.Count);

            foreach (var cellid in name_ids)
            {
                using (var person = Global.LocalStorage.UseCelebrity(cellid))
                {
                    Console.WriteLine(person.Name + "||" + person.CellID);
                    //foreach (var item in person.Act)
                    //{
                    //    using (var movie = Global.LocalStorage.UseMovie(item))
                    //    { Console.WriteLine(movie.Name); }
                    //}
                    //Console.WriteLine("===========================");
                    //foreach (var item in person.Direct)
                    //{
                    //    using (var movie = Global.LocalStorage.UseMovie(item))
                    //    { Console.WriteLine(movie.Name); }
                    //}
                }
            }
            Console.WriteLine("====== test multi hop ========");
            var desc = StartFrom(name_ids[0], new[] { "Name" }).FollowEdge("Act").VisitNode(Action.Continue, new[] { "Name" }).FollowEdge("Directors").VisitNode(Action.Return, new[] { "Name" });
            foreach (var res in desc)
            {
                Console.WriteLine(res);
            }
            Console.WriteLine("========== test lambda ===========");
            desc = StartFrom(name_ids[0], new[] { "Name" }).FollowEdge("Act").VisitNode(v => v.continue_if(v.GetField<string>("Name").Contains("无间道")), new[] { "Name" }).FollowEdge("Directors").VisitNode(Action.Return, new[] { "Name" }); ;
            foreach (var res in desc)
            {
                Console.WriteLine(res);
            }
            Console.WriteLine("======= test Accessor_Selector ========");
            var result = from node in Global.LocalStorage.Movie_Accessor_Selector()
                         where node.PublishDate > 20000101 && node.Rating > 93
                         select node.Name;
            foreach (var res in result)
            {
                Console.WriteLine(res);
            }
        }

        private static void ImportToyData()
        {
            Celebrity p1 = new Celebrity(233, 100, 0, TheType: DataImport.EntityType.Celebrity.ToString());
            Global.LocalStorage.SaveCelebrity(p1);
            Celebrity p2 = new Celebrity(-234, 200, p1.CellID, TheType: DataImport.EntityType.Celebrity.ToString());
            Global.LocalStorage.SaveCelebrity(p2);
            p2.Parent = 1;
            //var desc = StartFrom(234, new[] { "Age" }).FollowEdge("Parent").VisitNode(Action.Return, new[] { "Age" });
            var desc = StartFrom(234, new[] { "Age" }).VisitNode(Action.Return, new[] { "Age" });
            foreach (var path in desc)
            {
                Console.WriteLine(path);
            }
        }
    }
}
