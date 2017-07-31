using Microsoft.Bond;
using Microsoft.ObjectStore;
using Microsoft.Search.ObjectStore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnTable
{

    public class BaseHelper
    {
        public static string SubmitJob(string scriptFile, string jobName, string vcName = "searchSTC")
        {
            var exe = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScopeSDK", "Scope.exe");
            string cmd =
            "submit -i " + scriptFile
            + @" -vc https://cosmos08.osdinfra.net:443/cosmos/" + vcName + "/ -f " + jobName + " -p 700 ";

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            //p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.AutoFlush = true;

            p.StandardInput.WriteLine(exe + " " + cmd);
            p.StandardInput.WriteLine("exit");

            string strRst = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            p.Close();

            CloseProcess("cmd.exe");
            return strRst;

        }

        public static string SubmitJob(string scriptFile)
        {
            var exe = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScopeSDK", "Scope.exe");
            string cmd =
            "submit -i " + scriptFile
            + @" -vc https://cosmos08.osdinfra.net:443/cosmos/searchSTC/ -f PublishGeospatialData_AutoPipeline -p 700 ";

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            //p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.AutoFlush = true;

            p.StandardInput.WriteLine(exe + " " + cmd);
            p.StandardInput.WriteLine("exit");

            string strRst = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            p.Close();

            CloseProcess("cmd.exe");
            return strRst;

        }
        public static bool CloseProcess(string ProcName)
        {
            bool result = false;
            System.Collections.ArrayList procList = new System.Collections.ArrayList();
            string tempName = "";
            int begpos;
            int endpos;
            foreach (System.Diagnostics.Process thisProc in System.Diagnostics.Process.GetProcesses())
            {
                tempName = thisProc.ToString();
                begpos = tempName.IndexOf("(") + 1;
                endpos = tempName.IndexOf(")");
                tempName = tempName.Substring(begpos, endpos - begpos);
                procList.Add(tempName);
                if (tempName == ProcName)
                {
                    if (!thisProc.CloseMainWindow())
                        thisProc.Kill();
                    result = true;
                }
            }
            return result;
        }
        public static string TsvBase64BondEncode(
               IBondSerializable key,
               IBondSerializable value)
        {
            var keyStream = new MemoryStream();
            key.Write(new SimpleProtocolWriter(keyStream));

            var valueStream = new MemoryStream();
            value.Write(new CompactBinaryProtocolWriter(valueStream));

            return Convert.ToBase64String(keyStream.ToArray()) + "\t" +
                   Convert.ToBase64String(valueStream.ToArray());
        }

        public static string TsvBase64Encode(
            string key)
        {
            var bytes = Encoding.UTF8.GetBytes(key);

            return Convert.ToBase64String(bytes);
        }

        public static bool IngestPointTable(ChinaOpalSearch.EntityID entityId, ChinaOpalSearch.SnappsEntity entity)
        {

            using (var client = Client.Builder<ChinaOpalSearch.EntityID, ChinaOpalSearch.SnappsEntity>(
                        environment: Config.environment,
                        osNamespace: Config.osNamespace,
                        osTable: Config.osTable,
                        timeout: new TimeSpan(0, 0, 0, 500),
                        maxRetries: 3).Create())
            {
                var task = client.Write(new[] {
                    new KeyValuePair<ChinaOpalSearch.EntityID, ChinaOpalSearch.SnappsEntity>(entityId, entity) }).SendAsync();
                try
                {
                    task.Wait();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }

        public static bool IngestColumnTable(IColumnRecord<ChinaOpalSearch.EntityID> record)
        {

            using (var client = Client.Builder<ChinaOpalSearch.EntityID, ChinaOpalSearch.SnappsEntity>(
                        environment: Config.environment,
                        osNamespace: Config.osNamespace,
                        osTable: Config.osTable,
                        timeout: new TimeSpan(0, 0, 0, 500),
                        maxRetries: 3).Create())
            {
                var task = client.ColumnWrite(new List<IColumnRecord<ChinaOpalSearch.EntityID>> { record }).SendAsync();
                try
                {
                    task.Wait();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }
        public static bool IngestMultiEnvTable(ChinaOpalSearch.EntityID entityId, ChinaOpalSearch.SnappsEntity entity)
        {
            bool result = false;
            List<ITableLocation> locations = new List<ITableLocation>
       {
           new Microsoft.ObjectStore.Environment(Config.environment)

       };

            DataLoadConfiguration config = new DataLoadConfiguration(locations, "msnjvfeeds", "SnappsEntity", 20, 20, 2, 10000);
            DataLoader loader = new DataLoader(config);
            List<IDataLoadResult> results;
            object context = new object();
            while (true)
            {

                // Important: key and value objects are cached until flushed. Changing the contents of key or value before the flush will alter the previously added data
                loader.Send(entityId, entity, context);
                results = loader.Receive(false);
                //LogResults(results);
            }
            loader.Flush();
            results = loader.Receive(true);
            //LogResults(results);

            return result;
        }
        public static System.Text.RegularExpressions.Regex IsNumberic = new System.Text.RegularExpressions.Regex("^\\d+$");
        public static System.Text.RegularExpressions.Regex IsDate = new System.Text.RegularExpressions.Regex("^[\\d]{4}\\-[\\d]{2}\\-[\\d]{2}$");
        public static System.Text.RegularExpressions.Regex IsDateRange = new System.Text.RegularExpressions.Regex("^[\\d]{4}\\-[\\d]{4}$");
        public static System.Text.RegularExpressions.Regex IsDateYearMon = new System.Text.RegularExpressions.Regex("^[\\d]{4}\\-[\\d]{2}$");
    }
}
