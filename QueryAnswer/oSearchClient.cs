using Microsoft.Bond;
using ObjectStoreWireProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QueryAnswer
{
    class oSearchClient
    {
        //static string Namespace = "msnjvfeeds";
        //static string Table = "academicpapersv3";
        //static string IndexTable = "academicpapersv3index";
        //static string QueryURL = @"http://ObjectStoreMulti.Prod.HK.BingInternal.com:83/sds/ObjectStoreQuery/V1";
        //static string SecureQueryURL = @"https://ObjectStoreMulti.Prod.HK.BingInternal.com:446/sds/ObjectStoreQuery/V1";

        static string Namespace = "msnjvfeeds";
        static string Table = "snappsentity";
        static string IndexTable = "snappsentityindex";
        static string QueryURL = @"http://ObjectStoreMulti.Prod.HK.BingInternal.com:83/sds/ObjectStoreQuery/V1";
        static string SecureQueryURL = @"https://ObjectStoreMulti.Prod.HK.BingInternal.com:446/sds/ObjectStoreQuery/V1";

        //static string Namespace = "BilingualDictionary";
        //static string Table = "BingDictionary";
        //static string IndexTable = "BingDictionaryIndex";
        //static string QueryURL = @"http://ObjectStoreMulti.Prod.HK.BingInternal.com:83/sds/ObjectStoreQuery/V1";
        //static string SecureQueryURL = @"https://ObjectStoreMulti.Prod.HK.BingInternal.com:446/sds/ObjectStoreQuery/V1";
        //
        // create TLA query
        //
        static string GetTLAQuery(string namespaceName, string indexTableName, string queryWithAugmentation)
        {
            return string.Format("[tla:mermaidtesthook:{0}.{1}] {2} ", namespaceName, indexTableName, queryWithAugmentation);
        }

        static T DecodeBondObject<T>(byte[] rawBond) where T : IBondSerializable, new()
        {
            using (MemoryStream ms = new MemoryStream(rawBond, false))
            {
                return DecodeBondObject<T>(ms);
            }
        }

        static T DecodeBondObject<T>(Stream st) where T : IBondSerializable, new()
        {
            IProtocolReader reader = new SimpleProtocolReader(st);
            T bondObject = new T();
            bondObject.Read(reader);
            return bondObject;
        }
        

        private static void DoHttpIndexQuery(string indexQueryType, string tlaQuery, uint offSet, uint resultsCount, string outputFilePath)
        {
            //
            // use http query to get data through index
            //
            //IEnumerable<BingDictionary.DictionaryEntity> results;
            IEnumerable<ChinaOpalSearch.SnappsEntity> results;
            //IEnumerable<AcademicPaper.Paper> results;
            IEnumerable<IndexSubResponse> indexSubResponses;
            IndexResponse indexResponse;
            uint latencyBackend;

            // As soon as data is written to object store, it is indexed immediately. However, since the index
            // operation happens asynchronously, there is still a small chance that query happens before index
            // completion. In that case, query will not return the expected results. Here we add wait and retry
            // logic to make sure data is indexed and hence query returns expected results. 5 seconds timeout
            // value is random picked and is super conservative - normally index completes in milliseconds.
            const int timeoutInMs = 5 * 1000; // 5 seconds
            int timeElapsed = 0;

            do
            {
                Microsoft.ObjectStore.ObjectStoreClient.ObjectStoreOSearchRequest.DoQuery(
                    QueryURL, Namespace, Table, indexQueryType, tlaQuery, offSet, resultsCount,
                    out results, out indexSubResponses, out indexResponse, out latencyBackend);

                if (indexResponse.m_IndexQueryStatus != IndexQueryStatus.IndexQueryStatusFailure)
                {
                    // Here we received the expected number of sub responses.
                    break;
                }

                if (timeElapsed > timeoutInMs)
                {
                    Console.WriteLine("IndexResponse query status is not success in {0} ms. IndexQueryStatus:{1}",
                        timeElapsed, indexResponse.m_IndexQueryStatus.ToString());
                    return;
                }
                else
                {
                    Console.WriteLine("IndexResponse query status is not success yet. IndexQueryStatus:{0}. Will retry",
                        indexResponse.m_IndexQueryStatus.ToString());
                }

                Thread.Sleep(2000);
                timeElapsed += 2000;
            } while (true);

            StreamWriter output = new StreamWriter(outputFilePath);
            // Collect index response
            List<string> allKeys = new List<string>();
            ObjectStorePredefinedOperations predefinedOperations = new ObjectStorePredefinedOperations();
            foreach (var indexSubResponse in indexSubResponses)
            {
                //BingDictionary.DictionaryKey key = DecodeBondObject<BingDictionary.DictionaryKey>(indexSubResponse.m_Key.Data.Array);
                ChinaOpalSearch.EntityID key = DecodeBondObject<ChinaOpalSearch.EntityID>(indexSubResponse.m_Key.Data.Array);
                //PaperId key = DecodeBondObject<PaperId>(indexSubResponse.m_Key.Data.Array);
                allKeys.Add(key.Id + ",");
            }

            output.WriteLine(string.Format("backendlatency : {0}ms, totalestimatematchresults: {1}", latencyBackend.ToString(), indexResponse.m_TotalEstimatedMatches.ToString()));
            output.WriteLine(string.Format("output {0} results. traceid is: {1}", allKeys.Count.ToString(), indexResponse.m_TraceID.ToString()));
            output.WriteLine(string.Concat(allKeys));

            output.WriteLine("============================================results=================================================");

            int i = 0;
            foreach (var result in results)
            {
                string resultPage = outputFilePath + "." + "result" + i.ToString() + ".json";
                StreamWriter sw = new StreamWriter(resultPage);

                string json = ClassToJason(result);

                json = json.Replace("\r", "");
                json = json.Replace("\n", "");
                json = json.Replace("><", ">\r\n<");

                sw.Write(json);
                sw.Close();

                output.Write(json);
                output.Flush();

                output.WriteLine("------------------------------------------------------------------------------------------------");
                i++;
            }
        }

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("usage is like: .exe inputqueryaugmentationfile outputfolder");
                return;
            }

            string queryAndaug = "";

            StreamReader sr = new StreamReader(args[0]);
            if (!sr.EndOfStream)
            {
                queryAndaug = sr.ReadLine();
            }
            sr.Close();

            if (string.IsNullOrEmpty(queryAndaug))
            {
                Console.WriteLine("query is empty");
                return;
            }

            string[] tokens = queryAndaug.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 3)
            {
                Console.WriteLine("input is wrong!");
                return;
            }

            uint offSet = uint.Parse(tokens[0]);
            uint resultsCount = uint.Parse(tokens[1]);

            Console.WriteLine("Get oSearch results for queryandaug: {0}", queryAndaug);

            string tlaQuery = GetTLAQuery(Namespace, IndexTable, tokens[2]);
            ObjectStorePredefinedOperations predefinedOperations = new ObjectStorePredefinedOperations();
            DoHttpIndexQuery(predefinedOperations.INDEX_QUERY_WITH_VALUE, tlaQuery, offSet, resultsCount, args[1]);
        }
    }
}
