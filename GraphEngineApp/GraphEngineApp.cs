using System;
using System.Collections.Generic;
using GraphEngine;
using FanoutSearch;
using Trinity;
using FanoutSearch.LIKQ;
using Newtonsoft.Json.Linq;
using Trinity.Core.Lib;
using Trinity.Network;
using static FanoutSearch.LIKQ.KnowledgeGraph;
using Action = FanoutSearch.Action;

namespace GraphEngineApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FanoutSearchModule.EnableExternalQuery(true);
            FanoutSearchModule.SetQueryTimeout(30000);
            FanoutSearchModule.RegisterIndexService(Indexer);
            FanoutSearchModule.RegisterExpressionSerializerFactory(() => new ExpressionSerializer());

            var server = new TrinityServer();
            server.RegisterCommunicationModule<FanoutSearchModule>();
            server.Start();

            TrinityConfig.CurrentRunningMode = RunningMode.Embedded;
            Global.LocalStorage.LoadStorage();
            int count = 0;
            foreach (var item in Global.LocalStorage.Movie_Accessor_Selector())
            {
                count++;
                //Console.WriteLine(item.Name);
            }
            Console.WriteLine(count);

            List<long> name_ids = Index.Person_Name_SubstringQuery("刘德华");
            Console.WriteLine(name_ids.Count);

            foreach (var cellid in name_ids)
            {
                using (var person = Global.LocalStorage.UsePerson(cellid))
                {
                    Console.WriteLine(person.Name + "||" + person.CellID);
                    foreach (var item in person.Act)
                    {
                        using (var movie = Global.LocalStorage.UseMovie(item))
                        { Console.WriteLine(movie.Name); }
                    }
                    Console.WriteLine("===========================");
                    foreach (var item in person.Direct)
                    {
                        using (var movie = Global.LocalStorage.UseMovie(item))
                        { Console.WriteLine(movie.Name); }
                    }
                }
            }
            Console.WriteLine("===========================");

            var res = StartFrom(name_ids[0], select: new[] { "Name" }).
                FollowEdge("Act").
                VisitNode(FanoutSearch.Action.Continue).
                FollowEdge("Directors").
                VisitNode(FanoutSearch.Action.Return,select: new[] { "Name" });
            foreach (var item in res)
            {
                Console.WriteLine(item);
            }
        }

        private static IEnumerable<long> Indexer(object matchobject, string typestring)
        {
            JObject queryObj = (JObject)matchobject;
            string key = queryObj["key"].ToString();
            yield return HashHelper.HashString2Int64(key);
        }
    }
}
