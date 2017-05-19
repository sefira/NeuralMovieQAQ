using JiebaNet.Segmenter;
using JiebaNet.Segmenter.PosSeg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryAnswer
{
    public class Pattern
    {
        public Regex regex_pattern;

        public string entity_type;

        public string property;

        public int hop_num;

        public Pattern()
        {

        }

        public Pattern(string entity_type, string property, int hop_num, Regex regex_pattern)
        {
            this.entity_type = entity_type;
            this.property = property;
            this.hop_num = hop_num;
            this.regex_pattern = regex_pattern;
        }

        // Can parse from line with format: EntityType \t Property type with constraint \t #hop in knowledge graph \t Pattern RegEx
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
        public Tuple<string, string> tagged_entity;

        public string raw_query = "";

        public string post_query = "";

        public string entity_type;

        public string property;

        public int hop_num;

        public PatternResponse()
        {
        }

        public PatternResponse(Query tagged_query, Pattern pattern)
        {
            this.raw_query = tagged_query.raw_query;
            this.post_query = tagged_query.postagged_query;

            // used for generate database query
            this.entity_type = pattern.entity_type;
            this.property = pattern.property;

            this.hop_num = pattern.hop_num;
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

        // classify a qurry based on pattern
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
}
