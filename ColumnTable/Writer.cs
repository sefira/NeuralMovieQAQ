using Microsoft.Search.ObjectStore;
using ObjectStoreWireProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnTable
{
    class Writer
    {
        public static void WriteDataColumnTable()
        {
            using (
                var client =
                    Client.Builder<ChinaOpalSearch.EntityID, ChinaOpalSearch.SnappsEntity>(
                        environment: Config.environment,
                        osNamespace: Config.osNamespace,
                        osTable: Config.osTable,
                        timeout: new TimeSpan(0, 0, 0, 1000),
                        maxRetries: 1).Create())
            {

                var record = client.CreateColumnRecord(new ChinaOpalSearch.EntityID { Id = BaseHelper.TsvBase64Encode("Leon") });

                OSColumnOperationResultType result;
                result = record.SetColumnValue<string>("Name", null, "Leon");
                if (result != OSColumnOperationResultType.OSColumnResultSuccess)
                {
                    //error handling
                }

                result = record.SetColumnValue<string>("KgId", null, "0");
                if (result != OSColumnOperationResultType.OSColumnResultSuccess)
                {
                    //error handling
                }

                result = record.SetColumnValue<string>("Description", null, "an assassin and a girl");
                if (result != OSColumnOperationResultType.OSColumnResultSuccess)
                {
                    //error handling
                }

                var res = BaseHelper.IngestColumnTable(record);

                /*
                 * oSearch-like ingest is not work for Column Table
                 * 
                */
                //ChinaOpalSearch.EntityID key = new ChinaOpalSearch.EntityID();
                //key.Id = "1";
                //ChinaOpalSearch.SnappsEntity value = new ChinaOpalSearch.SnappsEntity();
                //value.Name = "007";
                //value.KgId = "1";
                //value.Description = "from russia with love";
                //var task2 = client.Write(new[] {
                //    new KeyValuePair<ChinaOpalSearch.EntityID, ChinaOpalSearch.SnappsEntity>(key, value) }).SendAsync();
                //try
                //{
                //    task2.Wait();
                //}
                //catch (Exception e)
                //{
                //    // One or more errors occurred.
                //    Console.WriteLine(e.Message);
                //}
                IngestData();
            }
        }

        public static void IngestData()
        {
            string dataPath = Config.dataPath;
            string logPath = Config.logPath;

            #region  Ingest Data
            if (!System.IO.File.Exists(logPath))
            {
                File.Create(logPath).Dispose();
            }
            using (
                var client =
                Client.Builder<ChinaOpalSearch.EntityID, ChinaOpalSearch.SnappsEntity>(
                environment: Config.environment,
                osNamespace: Config.osNamespace,
                osTable: Config.osTable,
                timeout: new TimeSpan(0, 0, 0, 1000),
                maxRetries: 1).Create())
            {
                using (StreamReader sr = new StreamReader(dataPath))
                {
                    using (StreamWriter sw = new StreamWriter(logPath))
                    {
                        int i = 0;
                        string line = string.Empty;
                        while (!sr.EndOfStream)
                        {
                            line = sr.ReadLine();
                            if (string.IsNullOrWhiteSpace(line)) { break; }
                            i++;
                            string[] term = line.Split(new char[] { '\t' });

                            ChinaOpalSearch.SnappsEntity value = new ChinaOpalSearch.SnappsEntity();
                            OSColumnOperationResultType result;
                            ChinaOpalSearch.EntityID key = new ChinaOpalSearch.EntityID();
                            value.KgId = term[1];
                            key.Id = value.KgId.Substring("http://kg.microsoft.com/".Length);
                            var record = client.CreateColumnRecord(new ChinaOpalSearch.EntityID { Id = key.Id });
                            result = record.SetColumnValue<string>("KgId", null, value.KgId);

                            #region init

                            System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]+$");
                            value.Alias = term[11].Split('|').Where(m => !string.IsNullOrWhiteSpace(m) && m.Length > 1
                                ).Where(m => regx.IsMatch(m) && m.Length > 10 || regx.IsMatch(m)).Distinct().ToList();
                            result = record.SetColumnValue<List<string>>("Alias", null, value.Alias);

                            value.Categories = term[14].Split('|').Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList();
                            result = record.SetColumnValue<List<string>>("Categories", null, value.Categories);

                            value.Description = term[12];
                            result = record.SetColumnValue<string>("Description", null, value.Description);

                            #region Entertainment
                            value.Entment = new ChinaOpalSearch.Entertainment();

                            value.Entment.Artists = term[3].Split('|').Select(m => m.ToLower().Trim().Replace("•", "·")).Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList();

                            value.Entment.Directors = term[4].Split('|').Select(m => m.ToLower().Trim().Replace("•", "·")).Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList();

                            value.Entment.Channels = term[8].Split('|').Select(m => m.ToLower().Trim().Replace("•", "·")).Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList();

                            value.Entment.Albums = term[9].Split('|').Select(m => m.Trim().Replace("•", "·")).Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList();

                            value.Entment.Characters = term[5].Split('|').Select(m => m.ToLower().Trim().Replace("•", "·")).Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList();

                            value.Entment.Distributors = term[7].Split('|').Select(m => m.ToLower().Trim().Replace("•", "·")).Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList();

                            value.Entment.Genres = term[2].Split('|').Select(m => m.ToLower().Trim().Replace("•", "·").Trim()).Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList();

                            value.Entment.Performance = new Dictionary<string, string>();

                            value.AnswerFeedName = "QuickBingWikiCard.SnappMovieForPartner";
                            value.AnswerScenario = "ModuleList";
                            value.AnswerServiceName = "MsnJVDataAnswerV2";
                            value.AnswerVSName = "";
                            value.UxHit = "GenericKif";
                            value.UxSchema = "GenericKif";
                            //value.g
                            if (term[6] != null)
                            {
                                foreach (var v in term[6].Split('|').Select(m => m.Trim()).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)))
                                {
                                    if (v.IndexOf(":") > 0)
                                    {
                                        var vs = v.Split(':');
                                        if (!value.Entment.Performance.ContainsKey(vs[0]))
                                        {
                                            value.Entment.Performance.Add(vs[0], vs[1]);
                                        }
                                        //else
                                        //{
                                        //    if (value.Entment.Performance[vs[1]].Contains(vs[0])) { continue; }
                                        //    value.Entment.Performance[vs[1]] = value.Entment.Performance[vs[1]] + ";" + vs[0];
                                        //}
                                    }
                                }
                            }
                            result = record.SetColumnValue<ChinaOpalSearch.Entertainment>("Entment", null, value.Entment);

                            #endregion
                            value.Filters = new Dictionary<string, string>();
                            if (term[25] != null)
                            {
                                value.Filters.Add("Language", term[25]);
                            }
                            result = record.SetColumnValue<Dictionary<string, string>>("Filters", null, value.Filters);


                            value.Geographies = term[24].Split('|').Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList();
                            result = record.SetColumnValue<List<string>>("Geographies", null, value.Geographies);

                            value.ImageUrls = new Dictionary<string, string>();
                            if (!string.IsNullOrWhiteSpace(term[27]))
                            {
                                foreach (var v in term[27].Split('|').Select(m => m.Trim()).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)))
                                {
                                    if (v.IndexOf(":") > 0)
                                    {

                                        if (v.IndexOf(":") > 0)
                                        {
                                            int index = v.IndexOf(':');
                                            if (value.ImageUrls.ContainsKey(v.Substring(0, index))) { continue; }
                                            value.ImageUrls.Add(v.Substring(0, index), v.Substring(index + 1));
                                        }
                                        //var vs = v.Split(':');
                                        //value.ImageUrls.Add(vs[0], vs[1]);
                                    }
                                }
                            }
                            result = record.SetColumnValue<Dictionary<string, string>>("ImageUrls", null, value.ImageUrls);
                            
                            uint length = 0;
                            if (!string.IsNullOrWhiteSpace(term[23]) && BaseHelper.IsNumberic.IsMatch(term[23]) &&
                                uint.TryParse(term[23], out length))
                            {
                                value.Length = length;
                            }
                            result = record.SetColumnValue<uint>("Length", null, value.Length);

                            value.Logo = term[30];
                            result = record.SetColumnValue<string>("Logo", null, value.Logo);

                            value.Name = term[10];
                            result = record.SetColumnValue<string>("Name", null, value.Name);

                            value.OfficialSite = term[28];
                            result = record.SetColumnValue<string>("OfficialSite", null, value.OfficialSite);


                            uint Popularity = 0;
                            if (!string.IsNullOrWhiteSpace(term[16]) && BaseHelper.IsNumberic.IsMatch(term[16]) &&
                                uint.TryParse(term[16], out Popularity))
                            {
                                value.Popularity = Popularity;
                            }
                            result = record.SetColumnValue<uint>("Popularity", null, value.Popularity);

                            uint PublishDate = 0;
                            if (!string.IsNullOrWhiteSpace(term[22]) && BaseHelper.IsNumberic.IsMatch(term[22]) &&
                                uint.TryParse(term[22], out PublishDate))
                            {
                                PublishDate = PublishDate * 10000 + 101;
                            }
                            else
                            {
                                DateTime date;
                                if (BaseHelper.IsDate.IsMatch(term[22]) && DateTime.TryParse(term[22], out date))
                                {
                                    PublishDate = UInt32.Parse(date.ToString("yyyyMMdd"));
                                }
                                else if (BaseHelper.IsDateRange.IsMatch(term[22].Trim()))
                                {
                                    var sPublishdate = term[22].Trim().Split('-');
                                    if (uint.TryParse(sPublishdate[1], out PublishDate))
                                        PublishDate = PublishDate * 10000 + 101;
                                }
                                else if (BaseHelper.IsDateYearMon.IsMatch(term[22].Trim()))
                                {
                                    var sPublishdate = term[22].Trim().Split('-');
                                    uint.TryParse(sPublishdate[0] + sPublishdate[1] + "01", out PublishDate);
                                }
                            }
                            value.PublishDate =
                                PublishDate.ToString().Length == 4 ? (PublishDate * 10000 + 101) :
                                    PublishDate.ToString().Length == 6 ? (PublishDate * 100 + 1) : PublishDate;
                            result = record.SetColumnValue<uint>("PublishDate", null, value.PublishDate);

                            value.UpdateDate = UInt32.Parse(DateTime.Now.ToString("yyyyMMdd"));
                            result = record.SetColumnValue<uint>("UpdateDate", null, value.UpdateDate);

                            uint Rank = 0;
                            if (BaseHelper.IsNumberic.IsMatch(term[21]) &&
                                uint.TryParse(term[21], out Rank))
                            {
                                value.Rank = Rank;
                            }
                            result = record.SetColumnValue<uint>("Rank", null, value.Rank);

                            uint Rating = 0;
                            if (BaseHelper.IsNumberic.IsMatch(term[17]) &&
                                uint.TryParse(term[17], out Rating))
                            {
                                value.Rating = Rating;
                            }
                            result = record.SetColumnValue<uint>("Rating", null, value.Rating);

                            uint RatingCount = 0;
                            if (BaseHelper.IsNumberic.IsMatch(term[18]) &&
                                uint.TryParse(term[18], out RatingCount))
                            {
                                value.RatingCount = RatingCount;
                            }
                            result = record.SetColumnValue<uint>("RatingCount", null, value.RatingCount);

                            uint ReviewCount = 0;
                            if (BaseHelper.IsNumberic.IsMatch(term[19]) &&
                                uint.TryParse(term[19], out ReviewCount))
                            {
                                value.ReviewCount = ReviewCount;
                            }
                            result = record.SetColumnValue<uint>("ReviewCount", null, value.ReviewCount);

                            value.Segments = term[13].Split('|').Where(m => !string.IsNullOrWhiteSpace(m)).Select(m =>
                            {
                                if (m.LastIndexOf('.') > 0)
                                {
                                    return m.Substring(m.LastIndexOf('.')).Trim('.');
                                }
                                return m;
                            }).Distinct().ToList();
                            result = record.SetColumnValue<List<string>>("Segments", null, value.Segments);

                            value.SourceUrls = new Dictionary<string, string>();
                            if (!string.IsNullOrWhiteSpace(term[26]))
                            {
                                foreach (var v in term[26].Split('|').Select(m => m.Trim()).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)))
                                {
                                    if (v.IndexOf(":") > 0)
                                    {
                                        int index = v.IndexOf(':');
                                        if (value.SourceUrls.ContainsKey(v.Substring(0, index))) { continue; }
                                        value.SourceUrls.Add(v.Substring(0, index), v.Substring(index + 1));
                                    }
                                }
                            }
                            result = record.SetColumnValue<Dictionary<string, string>>("SourceUrls", null, value.SourceUrls);

                            //value.UpdateDate= value.
                            uint VisitCount = 0;
                            if (BaseHelper.IsNumberic.IsMatch(term[20]) &&
                                uint.TryParse(term[20], out VisitCount))
                            {
                                value.VisitCount = VisitCount;
                            }
                            result = record.SetColumnValue<uint>("VisitCount", null, value.VisitCount);

                            uint queryRank = 0;
                            if (BaseHelper.IsNumberic.IsMatch(term[term.Length - 1]) &&
                                uint.TryParse(term[term.Length - 1], out queryRank))
                            {
                                value.QueryRank = queryRank;
                            }
                            result = record.SetColumnValue<uint>("queryRank", null, value.QueryRank);

                            var deviceInfo = term[0];
                            deviceInfo = deviceInfo.Substring("SnappMovieForPartner:".Length);
                            if (deviceInfo.StartsWith("device_mobile_"))
                            {
                                value.Clients.Add("mobile");
                            }
                            result = record.SetColumnValue<List<string>>("Clients", null, value.Clients);

                            #endregion

                            var res = BaseHelper.IngestColumnTable(record);
                            sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", term[0], term[10], key.Id, res));
                            Console.WriteLine(term[0] + "\t" + term[10] + "\t" + res + "\t" + i);
                            System.Threading.Thread.Sleep(10);

                        }
                        Console.WriteLine(string.Format("Injested complteted, totally ingest {0} papers.", i));
                        sw.WriteLine(string.Format("Injested complteted, totally ingest {0} papers.", i));
                    }
                }
                #endregion
            }
        }
    }
}
