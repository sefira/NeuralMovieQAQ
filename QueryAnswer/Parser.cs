using JiebaNet.Segmenter;
using JiebaNet.Segmenter.PosSeg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryAnswer
{
    class Entity
    {
        private static List<string> entity_list = new List<string>()
        {
            "movie",
            "artist",
            "director",
            "country",
            "genre"
        };

        public static List<string> EntityList
        {
            get { return entity_list; }
        }

        private static Dictionary<string, string> entity_tag = new Dictionary<string, string>()
        {
            { "movie","nmovie" } ,
            { "artist", "nrartist" },
            { "director", "nrdirector" },
            { "country", "ncountry" },
            { "genre", "ngenre" }
        };

        public static Dictionary<string, string> EntityTag
        {
            get { return entity_tag; }
        }
    }

    class EntitySegmenter
    {
        private static readonly string data_path = @"D:\MovieDomain\QueryAnswer\resource\";
        private static readonly string movie_filename = @"movie_name.csv";
        private static readonly string artist_filename = @"artist_name.csv";
        private static readonly string director_filename = @"director_name.csv";
        private static readonly string country_filename = @"country_name.csv";
        private static readonly string genre_filename = @"genre_name.csv";

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
            List<string> lines = new List<string>();
            while (true)
            {
                string line = sr.ReadLine();
                if (line != null && !string.IsNullOrEmpty(line))
                {
                    lines.Add(line);
                }
                else
                {
                    break;
                }
            }
            return new HashSet<string>(lines);
        }

        private static PosSegmenter pos_seg_movie;
        private static PosSegmenter pos_seg_artist;
        private static PosSegmenter pos_seg_director;
        private static PosSegmenter pos_seg_country;
        private static PosSegmenter pos_seg_genre;

        private Dictionary<string, PosSegmenter> pos_segers = new Dictionary<string, PosSegmenter>();

        public Dictionary<string, PosSegmenter> PosSegers
        {
            get { return pos_segers; }
        }

        public EntitySegmenter()
        {
            if (pos_seg_movie == null)
            {
                movie_name = ReadEntityFromFile(data_path + movie_filename);
                artist_name = ReadEntityFromFile(data_path + artist_filename);
                director_name = ReadEntityFromFile(data_path + director_filename);
                country_name = ReadEntityFromFile(data_path + country_filename);
                genre_name = ReadEntityFromFile(data_path + genre_filename);

                JiebaSegmenter segmenter;
                segmenter = new JiebaSegmenter();
                segmenter.LoadUserDict(data_path + movie_filename);
                pos_seg_movie = new PosSegmenter(segmenter);
                pos_segers.Add("movie", pos_seg_movie);

                segmenter = new JiebaSegmenter();
                segmenter.LoadUserDict(data_path + artist_filename);
                pos_seg_artist = new PosSegmenter(segmenter);
                pos_segers.Add("artist", pos_seg_artist);

                segmenter = new JiebaSegmenter();
                segmenter.LoadUserDict(data_path + director_filename);
                pos_seg_director = new PosSegmenter(segmenter);
                pos_segers.Add("director", pos_seg_director);

                segmenter = new JiebaSegmenter();
                segmenter.LoadUserDict(data_path + country_filename);
                pos_seg_country = new PosSegmenter(segmenter);
                pos_segers.Add("country", pos_seg_country);

                segmenter = new JiebaSegmenter();
                segmenter.LoadUserDict(data_path + genre_filename);
                pos_seg_genre = new PosSegmenter(segmenter);
                pos_segers.Add("genre", pos_seg_genre);
            }
        }
    }

    class Parser
    {
        private EntitySegmenter entity_seg = new EntitySegmenter();
        private JiebaSegmenter segmenter = new JiebaSegmenter();

        // for PublishDate
        private static readonly string _old_date = "怀旧,旧,经典,老,复古";
        private static readonly string _new_date = "最近,最新,新,最热,热门";
        private static readonly string _date = "年,年代";
        private static HashSet<string> old_date_tag;
        private static HashSet<string> new_date_tag;
        private static HashSet<string> date_tag;

        // for Rating 
        private static string _high_rating = "最好,有名,好,好看,精彩,最热,热门";
        private static string _low_rating = "";
        private static HashSet<string> high_rating_tag;
        private static HashSet<string> low_rating_tag;

        public Parser()
        {
            string[] tmp = _old_date.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            old_date_tag = new HashSet<string>(tmp);
            tmp = _new_date.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            new_date_tag = new HashSet<string>(tmp);
            tmp = _date.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            date_tag = new HashSet<string>(tmp);

            tmp = _high_rating.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            high_rating_tag = new HashSet<string>(tmp);
            tmp = _low_rating.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            low_rating_tag = new HashSet<string>(tmp);
        }

        // for movie name
        public void ParseMovieName(ref Query query)
        {
            var pos_tags = entity_seg.PosSegers["movie"].Cut(query.RawQuery);
            foreach (var item in pos_tags)
            {
                if (Entity.EntityTag["movie"].Equals(item.Flag))
                {
                    query.is_considerd["movie"] = true;
                    query.carried_movie.Add(item.Word);
                }
            }
        }

        // for artist
        public void ParseArtistName(ref Query query)
        {
            var pos_tags = entity_seg.PosSegers["artist"].Cut(query.RawQuery);
            foreach (var item in pos_tags)
            {
                if (Entity.EntityTag["artist"].Equals(item.Flag))
                {
                    query.is_considerd["artist"] = true;
                    query.carried_artist.Add(item.Word);
                }
            }
        }

        // for Director
        public void ParseDirectorName(ref Query query)
        {
            var pos_tags = entity_seg.PosSegers["director"].Cut(query.RawQuery);
            foreach (var item in pos_tags)
            {
                if (Entity.EntityTag["director"].Equals(item.Flag))
                {
                    query.is_considerd["director"] = true;
                    query.carried_director.Add(item.Word);
                    Console.WriteLine(string.Format("{0}   {1}", item.Word, item.Flag));
                }
            }
        }

        // for Country
        public void ParseCountryName(ref Query query)
        {
            var pos_tags = entity_seg.PosSegers["country"].Cut(query.RawQuery);
            foreach (var item in pos_tags)
            {
                if (Entity.EntityTag["country"].Equals(item.Flag))
                {
                    query.is_considerd["country"] = true;
                    query.carried_country.Add(item.Word);
                }
            }
        }

        // for Genre
        public void ParseGenreName(ref Query query)
        {
            var pos_tags = entity_seg.PosSegers["genre"].Cut(query.RawQuery);
            foreach (var item in pos_tags)
            {
                if (Entity.EntityTag["genre"].Equals(item.Flag))
                {
                    query.is_considerd["genre"] = true;
                    query.carried_genre.Add(item.Word);
                }
            }
        }

        // for PublishDate
        private string ParseDate(Query query, int position)
        {
            if (position > 0)
            {
                string pre_word = query.WordBrokenQuery[position - 1];
                string this_word = query.WordBrokenQuery[position];

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
            if (query.WordBrokenQuery == null)
            {
                query.WordBrokenQuery = new List<string>(segmenter.Cut(query.RawQuery));
            }
            List<string> word_list = query.WordBrokenQuery;
            string date;
            for (int i = 0; i < word_list.Count; i++)
            {
                if (old_date_tag.Contains(word_list[i]))
                {
                    query.carried_publishdate.year = 2010;
                    query.carried_publishdate.type = DateType.before;
                    query.is_considerd["publishdate"] = true;
                    return;
                }
                if (new_date_tag.Contains(word_list[i]))
                {
                    query.carried_publishdate.year = DateTime.Now.Year;
                    query.carried_publishdate.type = DateType.exact;
                    query.is_considerd["publishdate"] = true;
                    return;
                }
                // if there is an exact time, then parse it and return
                if (date_tag.Contains(word_list[i]))
                {
                    date = ParseDate(query, i);
                    if (!string.IsNullOrEmpty(date))
                    {
                        query.carried_publishdate.year = int.Parse(date);
                        query.carried_publishdate.type = DateType.exact;
                        query.is_considerd["publishdate"] = true;
                        return ;
                    }
                }
            }
        }

        // for Rating
        public void ParseRating(ref Query query)
        {
            if (query.WordBrokenQuery == null)
            {
                query.WordBrokenQuery = new List<string>(segmenter.Cut(query.RawQuery));
            }
        }

        // for Duration
        public void ParseDuration(ref Query query)
        {
            if (query.WordBrokenQuery == null)
            {
                query.WordBrokenQuery = new List<string>(segmenter.Cut(query.RawQuery));
            }
        }

        public void ParseAll(ref Query query)
        {
            ParseMovieName(ref query);
            ParseArtistName(ref query);
            ParseDirectorName(ref query);
            ParseCountryName(ref query);
            ParseGenreName(ref query);
            ParsePublishDate(ref query);
            ParseRating(ref query);
            ParseDuration(ref query);
        }
    }
}
