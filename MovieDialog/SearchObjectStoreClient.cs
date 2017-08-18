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
        static SearchObjectStoreClient()
        {
            using (
                var client =
                Client.Builder<ChinaOpalSearch.EntityID, ChinaOpalSearch.SnappsEntity>(
                environment: Config.environment,
                osNamespace: Config.osNamespace,
                osTable: Config.osColumnTable,
                timeout: new TimeSpan(0, 0, 0, 1000),
                maxRetries: 1).Create())
            { }
        }
        static string GetTLAQuery(string queryWithAugmentation)
        {
            return string.Format("[tla:mermaidtesthook:MsnJVFeeds.SnappsEntityIndex] {0} ", queryWithAugmentation);
        }

        static SnappsEntity ConstructSnappsEntity(IColumnRecord<EntityID> record)
        {
            SnappsEntity res = new SnappsEntity();
            string Name;
            record.GetColumnValue<string>("Name", null, out Name);
            res.Name = Name;
            List<string> Alias;
            record.GetColumnValue<List<string>>("Alias", null, out Alias);
            res.Alias = Alias;
            string KgId;
            record.GetColumnValue<string>("KgId", null, out KgId);
            res.KgId = KgId;
            string Description;
            record.GetColumnValue<string>("Description", null, out Description);
            res.Description = Description;
            List<string> Segments;
            record.GetColumnValue<List<string>>("Segments", null, out Segments);
            res.Segments = Segments;
            List<string> Categories;
            record.GetColumnValue<List<string>>("Categories", null, out Categories);
            res.Categories = Categories;
            Dictionary<string, string> Filters;
            record.GetColumnValue<Dictionary<string, string>>("Filters", null, out Filters);
            res.Filters = Filters;
            List<string> Geographies;
            record.GetColumnValue<List<string>>("Geographies", null, out Geographies);
            res.Geographies = Geographies;
            uint Popularity;
            record.GetColumnValue<uint>("Popularity", null, out Popularity);
            res.Popularity = Popularity;
            uint RatingCount;
            record.GetColumnValue<uint>("RatingCount", null, out RatingCount);
            res.RatingCount = RatingCount;
            uint Rating;
            record.GetColumnValue<uint>("Rating", null, out Rating);
            res.Rating = Rating;
            uint ReviewCount;
            record.GetColumnValue<uint>("ReviewCount", null, out ReviewCount);
            res.ReviewCount = ReviewCount;
            uint VisitCount;
            record.GetColumnValue<uint>("VisitCount", null, out VisitCount);
            res.VisitCount = VisitCount;
            uint Rank;
            record.GetColumnValue<uint>("Rank", null, out Rank);
            res.Rank = Rank;
            uint PublishDate;
            record.GetColumnValue<uint>("PublishDate", null, out PublishDate);
            res.PublishDate = PublishDate;
            uint UpdateDate;
            record.GetColumnValue<uint>("UpdateDate", null, out UpdateDate);
            res.UpdateDate = UpdateDate;
            uint EndDate;
            record.GetColumnValue<uint>("EndDate", null, out EndDate);
            res.EndDate = EndDate;
            uint Length;
            record.GetColumnValue<uint>("Length", null, out Length);
            res.Length = Length;
            uint QueryRank;
            record.GetColumnValue<uint>("QueryRank", null, out QueryRank);
            res.QueryRank = QueryRank;
            Dictionary<string, string> ImageUrls;
            record.GetColumnValue<Dictionary<string, string>>("ImageUrls", null, out ImageUrls);
            res.ImageUrls = ImageUrls;
            Dictionary<string, string> SourceUrls;
            record.GetColumnValue<Dictionary<string, string>>("SourceUrls", null, out SourceUrls);
            res.SourceUrls = SourceUrls;
            string OfficialSite;
            record.GetColumnValue<string>("OfficialSite", null, out OfficialSite);
            res.OfficialSite = OfficialSite;
            string Logo;
            record.GetColumnValue<string>("Logo", null, out Logo);
            res.Logo = Logo;
            Entertainment Entment;
            record.GetColumnValue<Entertainment>("Entment", null, out Entment);
            res.Entment = Entment;

            return res;
        }

        public static async Task<List<EntityID>> IndexQuery(string tlaQuery, uint offSet, uint resultsCount)
        {
            List<EntityID> keys = new List<EntityID>();
            List<OSearchResult<EntityID, SnappsEntity>> oSResults = new List<OSearchResult<EntityID, SnappsEntity>>();
            using (
                var client =
                    Client.Builder<ChinaOpalSearch.EntityID, ChinaOpalSearch.SnappsEntity>(
                        environment: Config.environment,
                        osNamespace: Config.osNamespace,
                        osTable: Config.osSearchTable,
                        timeout: new TimeSpan(0, 0, 0, 1000),
                        maxRetries: 1).Create())
            {
                officialObjectStore::ObjectStoreWireProtocol.IndexQueryRequest req = new officialObjectStore::ObjectStoreWireProtocol.IndexQueryRequest();
                req.m_IndexQuery = tlaQuery;
                req.m_ResultBase = offSet;
                req.m_ResultCount = resultsCount;
                Dictionary<string, string> header = new Dictionary<string, string>();
                string traceId = Guid.NewGuid().ToString("N");
                header.Add("X-TraceId", traceId);

                try
                {
                    keys = await client.IndexQuery(req).GetKeysOnly();

                    //var columninfos = new List<ColumnLocation>();
                    //columninfos.Add(new ColumnLocation("Name", ""));
                    //columninfos.Add(new ColumnLocation("Alias", ""));
                    //columninfos.Add(new ColumnLocation("KgId", ""));
                    //var res = await client.IndexQuery(req).GetKeysWithColumnValues(columninfos);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
            }
            return keys;
        }

        public static async Task<List<SnappsEntity>> ColumnTableQuery(List<EntityID> keys)
        {
            using (
                var client =
                    Client.Builder<ChinaOpalSearch.EntityID, ChinaOpalSearch.SnappsEntity>(
                        environment: Config.environment,
                        osNamespace: Config.osNamespace,
                        osTable: Config.osColumnTable,
                        timeout: new TimeSpan(0, 0, 0, 1000),
                        maxRetries: 1).Create())
            {
                var columninfos = new List<ColumnLocation>();
                columninfos.Add(new ColumnLocation("Name", ""));
                columninfos.Add(new ColumnLocation("Alias", ""));
                columninfos.Add(new ColumnLocation("KgId", ""));
                columninfos.Add(new ColumnLocation("Description", ""));
                columninfos.Add(new ColumnLocation("Segments", ""));
                columninfos.Add(new ColumnLocation("Categories", ""));
                columninfos.Add(new ColumnLocation("IntFilters", ""));
                columninfos.Add(new ColumnLocation("Filters", ""));
                columninfos.Add(new ColumnLocation("Geographies", ""));
                columninfos.Add(new ColumnLocation("Popularity", ""));
                columninfos.Add(new ColumnLocation("RatingCount", ""));
                columninfos.Add(new ColumnLocation("Rating", ""));
                columninfos.Add(new ColumnLocation("ReviewCount", ""));
                columninfos.Add(new ColumnLocation("VisitCount", ""));
                columninfos.Add(new ColumnLocation("Rank", ""));
                columninfos.Add(new ColumnLocation("PublishDate", ""));
                columninfos.Add(new ColumnLocation("UpdateDate", ""));
                columninfos.Add(new ColumnLocation("EndDate", ""));
                columninfos.Add(new ColumnLocation("Length", ""));
                columninfos.Add(new ColumnLocation("QueryRank", ""));
                columninfos.Add(new ColumnLocation("Entment", ""));

                List<IColumnRecord<ChinaOpalSearch.EntityID>> records;
                records = await client.ColumnRead(keys, columninfos).SendAsync();


                //string query_format = @"#:"" _DocType_ChinaEntity"" #:""filmSegments "" AND ({0})";
                //string query_filter = @"rangeconstraint:bt:20160504:20170604:#:"" _PublishDate""";
                //string query = string.Format(query_format, query_filter);
                //string tlaQuery = GetTLAQuery(query);
                //officialObjectStore::ObjectStoreWireProtocol.IndexQueryRequest req = new officialObjectStore::ObjectStoreWireProtocol.IndexQueryRequest();
                //req.m_IndexQuery = tlaQuery;
                //req.m_ResultBase = 0;
                //req.m_ResultCount = 12;
                //Dictionary<string, string> header = new Dictionary<string, string>();
                //string traceId = Guid.NewGuid().ToString("N");
                //header.Add("X-TraceId", traceId);
                //var res = await client.IndexQuery(req).GetKeysWithColumnValues(columninfos);
                List<SnappsEntity> results = new List<SnappsEntity>();
                foreach (var record in records)
                {
                    string name;
                    record.GetColumnValue<string>("Name", null, out name);
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        results.Add(ConstructSnappsEntity(record));
                    }
                }
                return results;
            }
        }

        public static void TestQuery()
        {
            string query_format = @"#:"" _DocType_ChinaEntity"" #:""filmSegments "" AND ({0})";
            //string query_filter = @" #:""刘德华Artists "" AND #:""王宝强Artists "" AND #:""战争Genres """;
            //string query_filter = @"rangeconstraint:bt:20160504:20170604:#:"" _PublishDate"" #:"" _PublishDate"" adjust:1rankmul:#:"" _PublishDate";
            string query_filter = @"rangeconstraint:bt:20160504:20170604:#:"" _PublishDate""";
            //string query_filter = @"((#:""刘德华Artists "") AND (#:""张艺谋Directors ""))";
            //string query_filter = @"(#:""张艺谋Artists "") AND (#:""喜剧Genres "")";
            string query = string.Format(query_format, query_filter);

            uint offSet = 0;
            uint resultsCount = 100;

            Console.WriteLine("Get oSearch results for query: {0}", query);

            string tlaQuery = GetTLAQuery(query);
            List<EntityID> keys = new List<EntityID>();
            List<SnappsEntity> results = new List<SnappsEntity>();
            keys = IndexQuery(tlaQuery, offSet, resultsCount).Result;
            results = ColumnTableQuery(keys).Result;

            foreach (var key in keys)
            {
                Console.WriteLine(key.Id);
            }
            foreach (var result in results)
            {
                Console.WriteLine(string.Join(" ", result.Name));
            }
        }

        public static IEnumerable<ChinaOpalSearch.SnappsEntity> Query(string query_filter, string rank_policy = "rating")
        {
            //string rank_string = " AND #:\" _RatingCount\" adjust:1rankmul:#:\" _RatingCount\"";
            string rank_string = " AND #:\" _VisitCount\" adjust:1rankmul:#:\" _VisitCount\"";
            switch (rank_policy)
            {
                case "rating":
                    rank_string = " AND #:\" _RatingCount\" adjust:1rankmul:#:\" _RatingCount\"";
                    break;
                case "date":
                    rank_string = " AND #:\" _PublishDate\" adjust:1rankmul:#:\" _PublishDate\"";
                    break;
                case "recent":
                    rank_string = " AND rangeconstraint:bt:20160818:20170818:#:\" _PublishDate\"" + rank_string;
                    break;
            }
            string query_format = @"#:"" _DocType_ChinaEntity"" #:""filmSegments "" AND ({0})" + rank_string;
            string query = string.Format(query_format, query_filter);

            uint offSet = 0;
            uint resultsCount = 50;

            Console.WriteLine("Get oSearch results for query: {0}", query);

            string tlaQuery = GetTLAQuery(query);
            List<EntityID> keys = new List<EntityID>();
            List<SnappsEntity> results = new List<SnappsEntity>();

            try
            {
                keys = IndexQuery(tlaQuery, offSet, resultsCount).Result;
                results = ColumnTableQuery(keys).Result;
            }
            catch (Exception e)
            {
                //Console.WriteLine($"Exception caught: {e}");
                Utils.WriteError("Column Table has not this record!");
            }
            return results;
        }
    }
}
