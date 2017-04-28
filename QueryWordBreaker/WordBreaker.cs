using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExternalWordBreaker = WordBreaker;

namespace QueryWordBreaker
{
    public class WordBreaker
    {
        static WordBreaker _instance = null;
        static readonly object _locker = new object();

        private WordBreaker()
        {
            ExternalWordBreaker.Initialize();
        }

        public static WordBreaker GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new WordBreaker();
                    }
                }
            }
            return _instance;
        }

        public string DoBreakOnString(string input, Markets market = Markets.zhCN)
        {
            string mkt = MarketToString(market);
            string wbToken = ExternalWordBreaker.BreakWords(input, mkt);
            return wbToken;
        }

        public void DoBreakOnFile(string inputFile, string outputFile, Markets market, int column = 0)
        {
            string mkt = MarketToString(market);

            using (StreamReader sr = new StreamReader(inputFile))
            {
                PrepareDirectory(outputFile);

                using (StreamWriter sw = new StreamWriter(outputFile))
                {
                    while (!sr.EndOfStream)
                    {
                        string query = sr.ReadLine().Trim();
                        if (string.IsNullOrEmpty(query))
                            continue;

                        string[] tmp = query.Split(new char[] { '\t' });
                        if (column + 1 > tmp.Length)
                            continue;
                        string wbToken = ExternalWordBreaker.BreakWords(tmp[column], mkt);
                        tmp[column] = wbToken;

                        string output = MakeTsvString(tmp);
                        sw.WriteLine(output);
                    }
                }
            }
        }

        private string MakeTsvString(string[] tmp)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(tmp[0]);
            for (int i = 1; i < tmp.Length; i++)
            {
                sb.Append('\t');
                sb.Append(tmp[i]);
            }
            return sb.ToString();
        }

        private string MarketToString(Markets market)
        {
            return market.ToString().ToLower().Insert(2, "-");
        }

        private void PrepareDirectory(string filePath)
        {
            if (Directory.Exists(filePath) || File.Exists(filePath))
            {
                return;
            }

            Directory.CreateDirectory(filePath);
        }
    }
}
