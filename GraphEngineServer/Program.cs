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


            ImportMovieData(@"D:\MovieDomain\GraphEngineServer\data\");
            TestMovieData();

            //ImportToyData();
        }

        private static IEnumerable<long> Indexer(object matchobject, string typestring)
        {
            JObject queryObj = (JObject)matchobject;
            //string key = queryObj["key"].ToString();

            string key = queryObj["CellId"].ToString();
            yield return long.Parse(key);
        }

        private static void ImportMovieData(string path)
        {
            MovieEntityImport movie_entity_import = new MovieEntityImport(path);
            string filename = @"Movie.csv";
            movie_entity_import.ImportMovie(filename);
            foreach (var movie in Global.LocalStorage.Movie_Accessor_Selector())
            {
                Console.WriteLine(movie.Name);
                Console.WriteLine(movie.CellID);
            }
            Console.WriteLine();
            Global.LocalStorage.SaveStorage();
        }

        private static void TestMovieData()
        {
            Global.LocalStorage.LoadStorage();
            int count = 0;
            foreach (var item in Global.LocalStorage.Movie_Accessor_Selector())
            {
                count++;
            }
            Console.WriteLine(count);

            List<long> name_ids = Index.Person_Name_SubstringQuery("刘德华");
            Console.WriteLine(name_ids.Count);

            foreach (var cellid in name_ids)
            {
                using (var person = Global.LocalStorage.UsePerson(cellid))
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
            //Console.WriteLine("===========================");
            //var desc = StartFrom(name_ids[0], new[] { "Name" }).FollowEdge("Act").VisitNode(Action.Return, new[] { "Name" }).FollowEdge("Directors").VisitNode(Action.Return, new[] { "Name" });
            //foreach (var path in desc)
            //{
            //    Console.WriteLine(path);
            //}
        }

        private static void ImportToyData()
        {
            Person p1 = new Person(233, 100, 0);
            Global.LocalStorage.SavePerson(p1);
            Person p2 = new Person(-234, 200, p1.CellID);
            Global.LocalStorage.SavePerson(p2);
            p2.parent = 1;
            //var desc = StartFrom(234, new[] { "age" }).FollowEdge("parent").VisitNode(Action.Return, new[] { "age" });
            var desc = StartFrom(234, new[] { "age" }).VisitNode(Action.Return, new[] { "age" });
            foreach (var path in desc)
            {
                Console.WriteLine(path);
            }
        }
    }
}
