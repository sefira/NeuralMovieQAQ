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

namespace testLIKQClient
{
    class Program
    {
        static void Main(string[] args)
        {
            FanoutSearchModule.ForceRunAsClient(true);
            FanoutSearchModule.RegisterExpressionSerializerFactory(() => new ExpressionSerializer());

            foreach (var path in StartFrom(2391729982219739490, new[] { "Name" }))
            {
                foreach (var node in path)
                {
                    Console.WriteLine($"{node.id}: { node["Name"]}");
                }
            }
        }
    }
}
