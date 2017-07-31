using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnTable
{
    class Config
    {
        public static readonly string environment = "ObjectStoreMulti.Prod.HK.BingInternal.com:83/sds";
        public static readonly string osNamespace = "MsnJVFeeds";
        public static readonly string osTable = "SnappsEntityColumn";

        public static readonly string dataPath = @"D:\oSearch\data\Entity.tsv";
        public static readonly string logPath = @"D:\oSearch\data\log\log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
    }
}
