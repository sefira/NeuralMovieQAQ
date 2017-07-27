extern alias officialObjectStore;

using officialObjectStore::Microsoft.Search.ObjectStore;
using officialObjectStore::ObjectStoreWireProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChinaOpalSearch;

namespace MovieDialog
{
    class SearchObjectStoreClient
    {
        public static async Task IndexQuery()
        {
            using (
                var client =
                    officialObjectStore::Microsoft.Search.ObjectStore.Client.Builder<ChinaOpalSearch.EntityID, ChinaOpalSearch.SnappsEntity>(
                        //environment: "ObjectStoreMulti-Prod-HK2",
                        environment: "ObjectStoreMulti.Prod.HK.BingInternal.com:83/sds",
                        osNamespace: "MsnJVFeeds",
                        osTable: "SnappsEntity",
                        timeout: new TimeSpan(0, 0, 0, 1000),
                        maxRetries: 1).Create())
            {
                officialObjectStore::ObjectStoreWireProtocol.IndexQueryRequest req = new officialObjectStore::ObjectStoreWireProtocol.IndexQueryRequest();
                req.m_IndexQuery = "[tla:mermaidtesthook:MsnJVFeeds.SnappsEntityIndex] #:\" _DocType_ChinaEntity\" ";
                //req.m_ResultBase = 0;  // Offset of result to be returned
                req.m_ResultCount = 1;  // Number of result to be returned
                Dictionary<string, string> header = new Dictionary<string, string>();
                string traceId = Guid.NewGuid().ToString("N");
                header.Add("X-TraceId", traceId);

                try
                {
                    var keys = await client.IndexQuery(req).GetKeysOnly();
                    var oSResults = await client.IndexQuery(req).GetKeysWithValues();
                    var kFull = await client.IndexQuery(req).WithHttpHeaders(header).WithDebugInfoEnabled().GetKeysOnlyFullResponse();
                    var kvFull = await client.IndexQuery(req).WithHttpHeaders(header).WithDebugInfoEnabled().GetKeysWithValuesFullResponse();

                    List<ColumnLocation> column_list = new List<ColumnLocation>();
                    column_list.Add(new ColumnLocation("Name"));
                    //column_list.Add(new ColumnLocation("PublishDate"));
                    //column_list.Add(new ColumnLocation("Description"));
                    //column_list.Add(new ColumnLocation("Popularity"));
                    //var res = await client.IndexQuery(req).GetKeysWithColumnValues(column_list);
                    var records = await client.ColumnRead(keys, column_list).SendAsync();
                    string Name;
                    records[0].GetColumnValue<string>("Name", null, out Name);
                    foreach (var key in keys)
                    {
                        var yourkey = key;
                        Console.WriteLine(yourkey);
                    }
                    foreach (var result in oSResults)
                    {
                        var key = result.Key;
                        var value = result.Value;
                        var valueStatus = result.ValueStatus;
                    }
                    var responseDetail = kFull.ResponseDetail;
                    foreach (var result in kFull.ResultList)
                    {
                        var invertedIndexResultDetail = result.InvertedIndexResultDetail;
                        var key = result.Key;
                    }
                    responseDetail = kvFull.ResponseDetail;
                    foreach (var result in kvFull.ResultList)
                    {
                        var invertedIndexResultDetail = result.InvertedIndexResultDetail;
                        var key = result.Key;
                        var value = result.Value;
                        var valueResultDetail = result.ValueResultDetail;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
            }
        }

        public static void TestQuery()
        {
            IndexQuery().Wait();
        }
    }
}
