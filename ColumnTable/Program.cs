using Microsoft.Search.ObjectStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChinaOpalSearch;
using ObjectStoreWireProtocol;

namespace ColumnTable
{
    class Program
    {
        static void Main(string[] args)
        {
            //Writer.WriteDataColumnTable();
            ReadDataColumnTable().Wait();
        }


        public static async Task ReadDataColumnTable()
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
                var keys = new List<ChinaOpalSearch.EntityID> {
                    new ChinaOpalSearch.EntityID { Id = BaseHelper.TsvBase64Encode("Leon") },
                    new ChinaOpalSearch.EntityID { Id = BaseHelper.TsvBase64Encode("! [ai-ou]") },
                    new ChinaOpalSearch.EntityID { Id = "ISBbYWktb3Vd" }
                };

                var columninfos = new List<ColumnLocation>();
                columninfos.Add(new ColumnLocation("Name", ""));
                columninfos.Add(new ColumnLocation("Description", ""));

                List<IColumnRecord<ChinaOpalSearch.EntityID>> records;
                records = await client.ColumnRead(keys, columninfos).SendAsync();

                OSColumnOperationResultType result;
                foreach (var item in records)
                {
                    string Name;
                    result = item.GetColumnValue<string>("Name", null, out Name);
                    Console.WriteLine("Name: " + Name);

                    string Description;
                    result = item.GetColumnValue<string>("Description", null, out Description);
                    Console.WriteLine("Description: " + Description);
                }
            }
        }
    }
}
