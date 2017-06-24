using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FanoutSearch;
using Trinity;
using FanoutSearch.LIKQ;
using Trinity.Core.Lib;
using Trinity.Network;
using static FanoutSearch.LIKQ.KnowledgeGraph;
using Action = FanoutSearch.Action;
using System.Collections;

namespace MovieDialog
{
    class GraphEngineQuery
    {
        public Dictionary<string, long> celebrity_cellid = new Dictionary<string, long>();
        public Dictionary<string, long> movie_cellid = new Dictionary<string, long>();

        RestClient client = new RestClient(Config.rest_client_address);
        
        public GraphEngineQuery()
        {
            string line = "";
            try
            {
                using (StreamReader sr = new StreamReader(Config.path + Config.celebrity_cellid_dict_filename))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] line_arr = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        celebrity_cellid[line_arr[0]] = long.Parse(line_arr[1]);
                    }
                }
                using (StreamReader sr = new StreamReader(Config.path + Config.movie_cellid_dict_filename))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] line_arr = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        movie_cellid[line_arr[0]] = long.Parse(line_arr[1]);
                    }
                }
            }
            catch { Console.WriteLine("Dicionaries don't exist!"); }
        }

        public IRestResponse ExecuteRequest(string body)
        {
            //var desc = StartFrom(2391729982219739490, new[] { "Name" }).FollowEdge("Act").VisitNode(Action.Continue, new[] { "Name" }).FollowEdge("Directors").VisitNode(Action.Return, new[] { "Name" });
            //foreach (var res in desc)
            //{
            //    Console.WriteLine(res);
            //}
            if (string.IsNullOrWhiteSpace(body))
            {
                body = "{  \"path\": \"/person\",  \"person\" :   {\t\"match\": { \"CellId\": 2391729982219739490 },\t\"select\": [ \"Name\" ]  }}\n";
            }
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "8306c504-35ea-e4e1-e492-53f1272b4378");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("undefined", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }

        public List<object> GetGraphEngineData(string from_which, string edge, int hop_num)
        {
            List<object> ret = new List<object>();
            // from which cell
            long from_cellid = 0;
            if (celebrity_cellid.ContainsKey(from_which))
            {
                from_cellid = celebrity_cellid[from_which];
            }
            if (movie_cellid.ContainsKey(from_which))
            {
                from_cellid = movie_cellid[from_which];
            }

            try
            {
                // query path according to hop_num
                switch (hop_num)
                {
                    case 0:
                        string property = edge;
                        ret = QueryZeroHopData(from_cellid, property, hop_num);
                        break;
                    case 1:
                        ret = QueryOneHopData(from_cellid, edge, hop_num);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught: {e}");
                Utils.WriteError("Graph Engine Server hasn't been started!");
            }

            return ret;
        }

        private List<object> QueryZeroHopData(long from_cellid, string property, int hop_num)
        {
            List<object> ret = new List<object>();

            string query_path = "\"/entity1\"";

            string request_body = "{ \"path\": " + query_path + ",  \"entity1\" : {\n\t\"match\": { \"CellId\": " + from_cellid + " },\"select\": [ \""  + property + "\" ]  }}";
            var json_response = ExecuteRequest(request_body);
            dynamic response = JsonConvert.DeserializeObject(json_response.Content);
            Newtonsoft.Json.Linq.JArray response_res = response.Results;
            var paths = response_res.ToObject<List<List<Dictionary<string, object>>>>();
            foreach (var ress in paths)
            {
                ret.Add(ress[hop_num][property]);
            }
            return ret;
        }

        private List<object> QueryOneHopData(long from_cellid, string info, int hop_num)
        {
            List<object> ret = new List<object>();

            //Artists:Name(Performance:1&Gender:male
            string[] line = info.Split(new char[] { '(' }, StringSplitOptions.RemoveEmptyEntries);
            string edge_property = line[0];
            string constrint = (line.Count() == 2) ? line[1] : null;
            string[] edge_property_arr = edge_property.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            string edge = edge_property_arr[0];
            string property = edge_property_arr[1];

            string query_path = "\"/entity1/" + edge + "/entity2\"";

            string request_body = "{ \"path\": " + query_path + ",  \"entity1\" : {\n\t\"match\": { \"CellId\": " + from_cellid + " }  },  \"entity2\" : {\"select\": [ \"" + property + "\" ]  },}";
            var json_response = ExecuteRequest(request_body);
            dynamic response = JsonConvert.DeserializeObject(json_response.Content);
            Newtonsoft.Json.Linq.JArray response_res = response.Results;
            var paths = response_res.ToObject<List<List<Dictionary<string, object>>>>();
            foreach (var ress in paths)
            {
                ret.Add(ress[hop_num]["Name"]);
            }
            return ret;
        }
    }
}
