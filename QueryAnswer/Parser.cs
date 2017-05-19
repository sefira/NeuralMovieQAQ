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

    class EntitySegmenter
    {
        private static readonly string data_path = Config.data_path;
        private static readonly string movie_filename = Config.movie_filename;
        private static readonly string artist_filename = Config.artist_filename;
        private static readonly string director_filename = Config.director_filename;
        private static readonly string country_filename = Config.country_filename;
        private static readonly string genre_filename = Config.genre_filename;

        private static HashSet<string> movie_name;
        private static HashSet<string> artist_name;
        private static HashSet<string> director_name;
        private static HashSet<string> country_name;
        private static HashSet<string> genre_name;

        public static HashSet<string> Movie
        {
            get { return movie_name; }
        }
        public static HashSet<string> Artist
        {
            get { return artist_name; }
        }
        public static HashSet<string> Director
        {
            get { return director_name; }
        }
        public static HashSet<string> Country
        {
            get { return country_name; }
        }
        public static HashSet<string> Genre
        {
            get { return genre_name; }
        }

        /// <summary>
        /// read entity from entity file to fill the HashSet
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public HashSet<string> ReadEntityFromFile(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            List<string> entities = new List<string>();
            while (true)
            {
                string line = sr.ReadLine();
                string entity = "";
                if (line != null && !string.IsNullOrEmpty(line))
                {
                    entity = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    entities.Add(entity);
                }
                else
                {
                    break;
                }
            }
            return new HashSet<string>(entities);
        }

        public PosSegmenter pos_tagger;

        public EntitySegmenter()
        {
            if (pos_tagger == null)
            {
                movie_name = ReadEntityFromFile(data_path + movie_filename);
                artist_name = ReadEntityFromFile(data_path + artist_filename);
                director_name = ReadEntityFromFile(data_path + director_filename);
                country_name = ReadEntityFromFile(data_path + country_filename);
                genre_name = ReadEntityFromFile(data_path + genre_filename);

                // NOTE:
                // it seems the later PosSegmenter will overlap the former one, i.e. director
                // will overlay artist when the artist have the same name with director
                // even we use "new PosSegment(segment_xxx)".
                // this issue is caused by the static _wordTagTab in PosSegmenter.cs in Jieba.NET
                JiebaSegmenter segmenter = new JiebaSegmenter();
                segmenter.LoadUserDict(data_path + movie_filename);
                segmenter.LoadUserDict(data_path + artist_filename);
                segmenter.LoadUserDict(data_path + director_filename);
                segmenter.LoadUserDict(data_path + country_filename);
                segmenter.LoadUserDict(data_path + genre_filename);
                pos_tagger = new PosSegmenter(segmenter);
            }
        }
    }

    class Parser
    {
        private EntitySegmenter entity_seg = new EntitySegmenter();
        private JiebaSegmenter segmenter = new JiebaSegmenter();

        // for parse person name 
        private static readonly List<string> _artist_pattern = new List<string>(new string[] { "<nrcelebrity>(主|扮)?演" });
        private static readonly List<string> _director_pattern = new List<string>(new string[] { "<nrcelebrity>(导|导演|拍摄)" });
        private static readonly List<Regex> artist_pattern = new List<Regex>();
        private static readonly List<Regex> director_pattern = new List<Regex>();
        
        // for PublishDate
        private static readonly string _old_date = "怀旧,旧,经典,老,复古,旧电影,经典电影,老电影";
        private static readonly string _new_date = "最近,最新,新,最,热,热门";
        private static readonly string _date = "年,年代";
        private static HashSet<string> old_date_tag;
        private static HashSet<string> new_date_tag;
        private static HashSet<string> date_tag;

        // for Rating 
        private static string _high_rating = "最好,有名,好,好看,精彩,最,热,热门";
        private static string _low_rating = "";
        private static HashSet<string> high_rating_tag;
        private static HashSet<string> low_rating_tag;

        // for isAboutMovie
        private static string _intent = "想看,推荐,有什么,有没有,来一部,来部";
        private static string _must = "电影,影片,片子";
        private static HashSet<string> intent_word_tag;
        private static HashSet<string> must_word_tag;

        public Parser()
        {
            // for PersonName
            foreach (string str in _artist_pattern)
            {
                artist_pattern.Add(new Regex(str, RegexOptions.Compiled));
            }
            foreach (string str in _director_pattern)
            {
                director_pattern.Add(new Regex(str, RegexOptions.Compiled));
            }

            // for PublishDate
            string[] tmp = _old_date.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            old_date_tag = new HashSet<string>(tmp);
            tmp = _new_date.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            new_date_tag = new HashSet<string>(tmp);
            tmp = _date.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            date_tag = new HashSet<string>(tmp);

            // for Rating 
            tmp = _high_rating.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            high_rating_tag = new HashSet<string>(tmp);
            tmp = _low_rating.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            low_rating_tag = new HashSet<string>(tmp);

            // for isAboutMovie
            tmp = _intent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            intent_word_tag = new HashSet<string>(tmp);
            tmp = _must.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            must_word_tag = new HashSet<string>(tmp);
        }

        #region Parse Entity, such as Movie, Artist, Director, Country, Genre, PublishDate, Rating and Duration
        private void EntityNameReplace(Query to_be_tagged_query)
        {
            foreach (var item in to_be_tagged_query.postag_pair)
            {
                if (Entity.PosTagType.Contains(item.Flag))
                {
                    to_be_tagged_query.postagged_query += "<" + item.Flag + ">";
                }
                else
                {
                    to_be_tagged_query.postagged_query += item.Word;
                }
            }
        }

        public void PosTagging(ref Query query)
        {
            query.postag_pair = (List<Pair>)entity_seg.pos_tagger.Cut(query.raw_query);
            EntityNameReplace(query);
        }

        public void ParseAllTag(ref Query query)
        {
            ParseMovieName(ref query);
            ParsePersonName(ref query);
            ParseCountryName(ref query);
            ParseGenreName(ref query);
            ParsePublishDate(ref query);
            ParseRating(ref query);
            ParseDuration(ref query);
        }

        // for movie name
        public void ParseMovieName(ref Query query)
        {
            foreach (var item in query.postag_pair)
            {
                if (Entity.EntityTag[ParseStatus.Movie].Equals(item.Flag))
                {
                    query.is_considerd[ParseStatus.Movie] = true;
                    query.carried_movie.Add(item.Word);
                }
            }
        }

        // parse person name when we didn't know what we want
        public void ParsePersonName(ref Query query)
        {
            foreach(var item in query.postag_pair)
            {
                if(Entity.EntityTag[ParseStatus.Artist].Equals(item.Flag) || Entity.EntityTag[ParseStatus.Director].Equals(item.Flag))
                {
                    bool is_artist = false;
                    bool is_director = false;
                    // discriminate artist and director by Contains
                    // 李连杰can discri 宫崎骏can discri 张艺谋can't discri 王宝强can't discri
                    is_artist = EntitySegmenter.Artist.Contains(item.Word);
                    is_director = EntitySegmenter.Director.Contains(item.Word);
                    if(!(is_artist ^ is_director))
                    {
                        // discriminate artist and director by Regex
                        // 张艺谋演\导的can discri 张艺谋的can't discri
                        foreach (Regex pattern in artist_pattern)
                        {
                            if (pattern.IsMatch(query.raw_query))
                            {
                                is_artist = true;
                                break;
                            }
                        }
                        foreach (Regex pattern in director_pattern)
                        {
                            if (pattern.IsMatch(query.raw_query))
                            {
                                is_director = true;
                                break;
                            }
                        }
                    }
                    if(is_artist)
                    {
                        query.is_considerd[ParseStatus.Artist] = true;
                        query.carried_artist.Add(item.Word);
                    }
                    if (is_director)
                    {
                        query.is_considerd[ParseStatus.Director] = true;
                        query.carried_director.Add(item.Word);
                    }
                }
            }
        }

        // for Artist, when we know we just need an artist, we can evade ParsePersonName
        public void ParseArtistName(ref Query query)
        {
            foreach (var item in query.postag_pair)
            {
                if (Entity.EntityTag[ParseStatus.Artist].Equals(item.Flag))
                {
                    query.is_considerd[ParseStatus.Artist] = true;
                    query.carried_artist.Add(item.Word);
                }
            }
        }

        // for Director, when we know we just need a director, we can evade ParsePersonName
        public void ParseDirectorName(ref Query query)
        {
            foreach (var item in query.postag_pair)
            {
                if (Entity.EntityTag[ParseStatus.Director].Equals(item.Flag))
                {
                    query.is_considerd[ParseStatus.Director] = true;
                    query.carried_director.Add(item.Word);
                    Console.WriteLine(string.Format("{0}   {1}", item.Word, item.Flag));
                }
            }
        }

        // for Country
        public void ParseCountryName(ref Query query)
        {
            foreach (var item in query.postag_pair)
            {
                if (Entity.EntityTag[ParseStatus.Country].Equals(item.Flag))
                {
                    query.is_considerd[ParseStatus.Country] = true;
                    query.carried_country.Add(item.Word);
                }
            }
        }

        // for Genre
        public void ParseGenreName(ref Query query)
        {
            foreach (var item in query.postag_pair)
            {
                if (Entity.EntityTag[ParseStatus.Genre].Equals(item.Flag))
                {
                    query.is_considerd[ParseStatus.Genre] = true;
                    query.carried_genre.Add(item.Word);
                }
            }
        }

        // for PublishDate
        private string ParseYear(Query query, int position)
        {
            if (position > 0)
            {
                string pre_word = query.wordbroken_query[position - 1];
                string this_word = query.wordbroken_query[position];

                int year = 0;
                try
                {
                    switch (pre_word.Length)
                    {
                        case 2:
                            pre_word = "19" + pre_word;
                            break;
                        case 4:
                            break;
                        default:
                            return null;
                    }
                    year = Int32.Parse(pre_word);
                    if (year < 1900 || year > 2100)
                    {
                        return null;
                    }
                    else
                    {
                        return year.ToString();
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public void ParsePublishDate(ref Query query)
        {
            if (query.wordbroken_query == null)
            {
                query.wordbroken_query = new List<string>(segmenter.Cut(query.raw_query));
            }
            List<string> word_list = query.wordbroken_query;
            string date;
            for (int i = 0; i < word_list.Count; i++)
            {
                if (old_date_tag.Contains(word_list[i]))
                {
                    date = DateTime.Now.AddYears(-70).ToString("yyyyMMdd");
                    query.carried_publishdate.from = int.Parse(date);
                    date = DateTime.Now.AddMonths(-15).ToString("yyyyMMdd");
                    query.carried_publishdate.to = int.Parse(date);
                    query.is_considerd[ParseStatus.PublishDate] = true;
                    return;
                }
                if (new_date_tag.Contains(word_list[i]))
                {
                    date = DateTime.Now.AddYears(-1).ToString("yyyyMMdd");
                    query.carried_publishdate.from = int.Parse(date);
                    date = DateTime.Now.AddMonths(1).ToString("yyyyMMdd");
                    query.carried_publishdate.to = int.Parse(date);
                    query.is_considerd[ParseStatus.PublishDate] = true;
                    return;
                }
                // if there is an exact time, then parse it and return
                if (date_tag.Contains(word_list[i]))
                {
                    string year = ParseYear(query, i);
                    if (!string.IsNullOrEmpty(year))
                    {
                        date = new DateTime(int.Parse(year), 1, 1).ToString("yyyyMMdd");
                        query.carried_publishdate.from = int.Parse(date);
                        date = new DateTime(int.Parse(year), 12, 31).ToString("yyyyMMdd");
                        query.carried_publishdate.to = int.Parse(date);
                        query.is_considerd[ParseStatus.PublishDate] = true;
                        return ;
                    }
                }
            }
        }

        // for Rating
        public void ParseRating(ref Query query)
        {
            if (query.wordbroken_query == null)
            {
                query.wordbroken_query = new List<string>(segmenter.Cut(query.raw_query));
            }
        }

        // for Duration
        public void ParseDuration(ref Query query)
        {
            if (query.wordbroken_query == null)
            {
                query.wordbroken_query = new List<string>(segmenter.Cut(query.raw_query));
            }
        }

        #endregion

        public bool isAboutMovie(Query ori_query)
        {
            Query query = new Query(ori_query.raw_query);
            query.postag_pair = ori_query.postag_pair;
            // I want to watch an excat movie or an exact genre type movie
            ParseMovieName(ref query);
            ParseGenreName(ref query);
            if (query.carried_movie.Count != 0 || query.carried_genre.Count != 0)
            {
                foreach (string intent_item in intent_word_tag)
                {
                    if (query.raw_query.Contains(intent_item))
                    {
                        return true;
                    }
                }
            }

            // I want to watch a movie / film / etc.
            foreach (string intent_item in intent_word_tag)
            {
                foreach (string must_item in must_word_tag)
                {
                    if (query.raw_query.Contains(intent_item) && query.raw_query.Contains(must_item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
