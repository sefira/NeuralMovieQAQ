using JiebaNet.Segmenter;
using JiebaNet.Segmenter.PosSeg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieDialog
{
    public class Pattern
    {
        public Regex regex_pattern;

        public KBQAEntityType entity_type;

        public string property;

        public int hop_num;

        public Pattern(string entity_type_str, string property, int hop_num, Regex regex_pattern)
        {
            Enum.TryParse(entity_type_str, out entity_type);
            this.property = property;
            this.hop_num = hop_num;
            this.regex_pattern = regex_pattern;
        }

        // Can parse from line with format: KBQAEntityType \t Property type with constraint \t #hop in knowledge graph \t Pattern RegEx
        // Return a Pattern class 
        // Can be used as a Constructor 
        public static Pattern FromLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }

            string[] parts = line.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 4)
            {
                return new Pattern(parts[0], parts[1], int.Parse(parts[2]), new Regex(parts[3], RegexOptions.Compiled));
            }
            return null;
        }
    }

    class PatternResponse
    {
        public string raw_query = "";

        public string post_query = "";

        public KBQAEntityType entity_type;

        public string property;

        public int hop_num;

        public PatternResponse()
        {
        }

        public PatternResponse(Query tagged_query, Pattern pattern)
        {
            raw_query = tagged_query.raw_query;
            post_query = tagged_query.postagged_query;

            // used for generate database query
            entity_type = pattern.entity_type;
            property = pattern.property;

            hop_num = pattern.hop_num;
        }
    }

    // classify a qurry based on pattern
    class PatternBased
    {
        private List<Pattern> patterns = new List<Pattern>();

        public PatternBased()
        {
            // read pattern
            using (StreamReader sr = new StreamReader(Config.data_path + Config.pattern_filename))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    Pattern temp = Pattern.FromLine(line);
                    if (temp != null)
                    {
                        patterns.Add(temp);
                    }
                }
            }
        }

        // classify a query based on pattern
        public bool QuestionClassify(Query query, out PatternResponse pattern_response)
        {
            pattern_response = new PatternResponse();
            string postagged_query = query.postagged_query;

            foreach (Pattern pattern in patterns)
            {
                Match match = pattern.regex_pattern.Match(postagged_query);
                if (match.Success)
                {
                    pattern_response = new PatternResponse(query, pattern);
                    return true;
                }
            }
            return false;
        }
    }

    class CNNBased
    {
        private Dictionary<int, Pattern> class2pattern = new Dictionary<int, Pattern>()
        {
            {0, new Pattern("Negative", "Negative", 0, null)},
            {1, new Pattern("Movie", "Artists:Name", 1, null)},
            {2, new Pattern("Movie", "Directors:Name", 1, null)},
            {3, new Pattern("Movie", "PublishDate", 0, null)},
            {4, new Pattern("Movie", "Genres", 0, null)},
            {5, new Pattern("Movie", "Country", 0, null)},
            {6, new Pattern("Celebrity", "Act:Name", 1, null)},
            {7, new Pattern("Celebrity", "Direct:Name", 1, null)}
        };

        // classify a query based on CNN
        public bool QuestionClassify(Query query, out PatternResponse pattern_response)
        {
            pattern_response = new PatternResponse();
            string postagged_query = query.postagged_query;
            int tf_class = GetTensorFlowServingResponse(postagged_query);
            if (tf_class != -1 && tf_class != 0)
            {
                pattern_response = new PatternResponse(query, class2pattern[tf_class]);
                return true;
            }
            return false;
        }

        public static int GetTensorFlowServingResponse(string query)
        {
            string query_url = Config.cnnbased_classifier_endpoint + query;
            string result = GetResponse(query_url);
            return int.Parse(result);
        }

        public static string GetResponse(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string data = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet), true);
                }

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            else
            {
                Console.WriteLine($"Cannot fetch the HTML of {url}");
            }
            return data;
        }
    }
}
