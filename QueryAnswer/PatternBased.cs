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
    public enum EntityType { Movie, Person };

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

    public class TaggedQuery
    {
        public Tuple<string, string> tagged_entity;
        public string ori_query = "";
        public string post_query = "";
    }

    public class PatternResponse
    {
        public Tuple<string, string> tagged_entity;

        public string ori_query = "";

        public string post_query = "";

        public string entity_type;

        public string property;

        public int hop_num;

        public PatternResponse()
        {
        }

        public PatternResponse(TaggedQuery tagged_query, Pattern pattern)
        {
            this.ori_query = tagged_query.ori_query;
            this.post_query = tagged_query.post_query;

            // used for generate database query
            this.tagged_entity = tagged_query.tagged_entity;
            this.entity_type = pattern.entity_type;
            this.property = pattern.property;

            this.hop_num = pattern.hop_num;
        }
    }

    // classify a qurry based on pattern
    class PatternBased
    {
        private PosSegmenter entity_tagger = new PosSegmenter();

        private List<Pattern> patterns= new List<Pattern>();

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

            JiebaSegmenter segmenter = new JiebaSegmenter();
            segmenter.LoadUserDict(Config.data_path + Config.movie_filename);
            segmenter.LoadUserDict(Config.data_path + Config.artist_filename);
            segmenter.LoadUserDict(Config.data_path + Config.director_filename);
            //segmenter.LoadUserDict(Config.data_path + Config.country_filename);
            //segmenter.LoadUserDict(Config.data_path + Config.genre_filename);

            entity_tagger = new PosSegmenter(segmenter);
        }

        private bool EntityNameReplace(string ori_query, out TaggedQuery tagged_query)
        {
            bool is_relevance = false;
            tagged_query = new TaggedQuery();
            tagged_query.ori_query = ori_query;

            var pos_tags = entity_tagger.Cut(ori_query);
            foreach (var item in pos_tags)
            {
                if (Entity.EntityTag[ParseStatus.Movie].Equals(item.Flag) || 
                    Entity.EntityTag[ParseStatus.Artist].Equals(item.Flag) ||
                    Entity.EntityTag[ParseStatus.Director].Equals(item.Flag))
                {
                    tagged_query.tagged_entity = (new Tuple<string, string>(item.Word, item.Flag));
                    tagged_query.post_query += "<Entity>";
                    is_relevance = true;
                }
                else
                {
                    tagged_query.post_query += item.Word;
                }
            }
            return is_relevance;
        }

        // classify a qurry based on pattern
        public bool QuestionClassify(string query, out PatternResponse pattern_response)
        {
            pattern_response = new PatternResponse();
            TaggedQuery tagged_query;
            bool is_relevance = EntityNameReplace(query, out tagged_query);

            foreach (Pattern pattern in patterns)
            {
                Match match = pattern.regex_pattern.Match(tagged_query.post_query);
                if (match.Success)
                {
                    pattern_response = new PatternResponse(tagged_query, pattern);
                    return true;
                }
            }
            return false;
        }
    }
}
