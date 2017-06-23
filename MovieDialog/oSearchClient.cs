using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using Microsoft.Bond;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization;

// HTTP Interface - see HttpManagedClient project
using Microsoft.ObjectStore.HTTPInterface;
using Microsoft.ObjectStore.OneBox;
using ObjectStoreClient;
using ObjectStoreWireProtocol;

// See Project Properties => Pre-Build Event script
using ChinaOpalSearch;
using Microsoft.ObjectStore.OSearchClient;

namespace MovieDialog
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
        
        static string ClassToJason(ChinaOpalSearch.SnappsEntity entity)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(ChinaOpalSearch.SnappsEntity));

            System.IO.MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, entity);
            System.IO.StreamReader reader = new StreamReader(ms);
            ms.Position = 0;
            string strRes = reader.ReadToEnd();
            reader.Close();
            ms.Close();

            return strRes;
        }

        private static IEnumerable<ChinaOpalSearch.SnappsEntity> DoHttpIndexQuery(string indexQueryType, string tlaQuery, uint offSet, uint resultsCount)
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
                    return results;
                }
                else
                {
                    Console.WriteLine("IndexResponse query status is not success yet. IndexQueryStatus:{0}. Will retry",
                        indexResponse.m_IndexQueryStatus.ToString());
                }

                Thread.Sleep(2000);
                timeElapsed += 2000;
            } while (true);

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
            return results;
        }

        public static void TestQuery()
        {
            string query_format = @"#:"" _DocType_ChinaEntity"" #:""filmSegments "" AND ({0})";
            //string query_filter = @" #:""刘德华Artists "" AND #:""王宝强Artists "" AND #:""战争Genres """;
            //string query_filter = @"rangeconstraint:bt:20160504:20170604:#:"" _PublishDate"" #:"" _PublishDate"" adjust:1rankmul:#:"" _PublishDate";
            //string query_filter = @"rangeconstraint:bt:20160504:20170604:#:"" _PublishDate""";
            //string query_filter = @"((#:""刘德华Artists "") AND (#:""张艺谋Directors ""))";
            //string query_filter = @"((#:""头发Name ""))";
            string query_filter = @"(#:""张艺谋Artists "") AND (#:""喜剧Genres "")";
            string query = string.Format(query_format, query_filter);

            uint offSet = 0;
            uint resultsCount = 10;

            Console.WriteLine("Get oSearch results for query: {0}", query);

            string tlaQuery = GetTLAQuery(Namespace, IndexTable, query);
            ObjectStorePredefinedOperations predefinedOperations = new ObjectStorePredefinedOperations();
            var results = DoHttpIndexQuery(predefinedOperations.INDEX_QUERY_WITH_VALUE, tlaQuery, offSet, resultsCount);

            foreach (var result in results)
            {
                Console.WriteLine(result.Name);
                //Console.WriteLine(string.Join(" ",result.Entment.Genres));
            }
        }
        
        public static IEnumerable<ChinaOpalSearch.SnappsEntity> Query(string query_filter)
        {
            string query_format = @"#:"" _DocType_ChinaEntity"" #:""filmSegments "" AND ({0})" + " AND #:\" _RatingCount\" adjust:1rankmul:#:\" _RatingCount\"";
            string query = string.Format(query_format, query_filter);

            uint offSet = 0;
            uint resultsCount = 100;

            Console.WriteLine("Get oSearch results for query: {0}", query);

            string tlaQuery = GetTLAQuery(Namespace, IndexTable, query);
            ObjectStorePredefinedOperations predefinedOperations = new ObjectStorePredefinedOperations();
            var results = DoHttpIndexQuery(predefinedOperations.INDEX_QUERY_WITH_VALUE, tlaQuery, offSet, resultsCount);

            return results;
        }
    }
}
