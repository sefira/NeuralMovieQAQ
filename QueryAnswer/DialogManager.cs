﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChinaOpalSearch;

namespace QueryAnswer
{

    class DialogManager
    {
        public static Dictionary<ParseStatus, int> considerd_weight = new Dictionary<ParseStatus, int>()
        {
            { ParseStatus.Movie, 100 },
            { ParseStatus.Artist, 50 },
            { ParseStatus.Director, 50 },
            { ParseStatus.Country, 30 },
            { ParseStatus.Genre, 50 },
            { ParseStatus.PublishDate, 100 },
            { ParseStatus.Rating, 10 },
            { ParseStatus.Duration, 20 }
        };

        private static int end_movie_count = 5;
        private static int end_considerd_count = 100;

        private static string[,] relation_matrix = new string[5, 5]
        {
            ///////////// artist                            director                  country       genre                       publishdate
            /* artist */  { "{0}和{1}有很多合作，想看谁的", "{0}和{1}有很多合作，想看谁的", "X",   "{0}演了很多{1}，想看哪种",   "什么时候拍摄"}, 
            /* director */{ "{0}和{1}有很多合作，想看谁的", "X",                           "X",   "{0}拍了很多{1}，想看哪种", "什么时候拍摄"}, 
            /* country */ { "{0}有一些著名艺人：{1}",       "{0}有一些著名导演：{1}",       "X",    "这个地区的{0}比有名，想看哪种", "什么时候拍摄"}, 
            /* genre */   { "{0}类型有很多著名演员：{1}，想看谁的", "{0}类型有很多著名导演：{1}，想看谁的", "{1}出产了很多{0}类型的电影，想看哪里的","X",    "什么时候拍摄"}, 
            /* publish */ { "X", "X", "X","X", "X"}
        };

        private static double[,] transition_matrix = new double[5, 5] 
        { 
            ///////////// artist director country      genre   publishdate
            /* artist */  { 0.2,    0.2,   0.01,       0.3,    0.3},
            /* director */{ 0.3,    0.01,  0.01,       0.4,    0.3},
            /* country */ { 0.3,    0.3,   0.01,       0.2,    0.2},
            /* genre */   { 0.4,    0.3,   0.2,        0.01,   0.1},
            /* publish */ { 0.1,    0.2,   0.3,        0.4,    0.01}
        };

        private static List<List<double>> roulette_matrix = new List<List<double>>();
        //private Dictionary<string, List<double>> roulette_matrix = new Dictionary<string, List<double>>()
        //{
        //    { "artist", new List<double>() },
        //    { "director", new List<double>() },
        //    { "country", new List<double>() },
        //    { "genre", new List<double>() },
        //    { "publishdate", new List<double>() },
        //};

        public static Dictionary<ParseStatus, int> parsestatus2transmat = new Dictionary<ParseStatus, int>()
        {
            { ParseStatus.Artist, 0 },
            { ParseStatus.Director, 1 },
            { ParseStatus.Country, 2 },
            { ParseStatus.Genre, 3 },
            { ParseStatus.PublishDate, 4 }
        };
        public static Dictionary<int, ParseStatus> transmat2parsestatus = new Dictionary<int, ParseStatus>()
        {
            { 0, ParseStatus.Artist },
            { 1, ParseStatus.Director },
            { 2, ParseStatus.Country },
            { 3, ParseStatus.Genre },
            { 4, ParseStatus.PublishDate }
        };

        private static Random rand = new Random();

        public DialogManager()
        {
            // to build the roulette matrix
            for (int i = 0; i < transition_matrix.GetLength(0); i++)
            {
                double sum = 0;
                for (int j = 0; j < transition_matrix.GetLength(1); j++)
                {
                    sum += transition_matrix[i, j];
                }
                roulette_matrix.Add(new List<double>());
                roulette_matrix[i].Add(transition_matrix[i, 0] / sum);
                for (int j = 1; j < transition_matrix.GetLength(1); j++)
                {
                    roulette_matrix[i].Add(roulette_matrix[i][j - 1] + (transition_matrix[i, j] / sum));
                }
            }
            // print for sure
            //for (int i = 0; i < transition_matrix.GetLength(0); i++)
            //{
            //    for (int j = 0; j < transition_matrix.GetLength(1); j++)
            //    {
            //        Console.WriteLine(roulette_matrix[i][j]);
            //    }
            //}
        }

        #region Dialog Logic
        // judge is this dialog should be ended
        public bool isDialogEnd(Session session)
        {
            int considerd_score = 0;
            considerd_score += (session.is_considerd[ParseStatus.Movie] ? considerd_weight[ParseStatus.Movie] : 0);
            considerd_score += (session.is_considerd[ParseStatus.Artist] ? considerd_weight[ParseStatus.Artist] * session.carried_artist.Count : 0);
            considerd_score += (session.is_considerd[ParseStatus.Director] ? considerd_weight[ParseStatus.Director] *
                session.carried_director.Count: 0);
            considerd_score += (session.is_considerd[ParseStatus.Country] ? considerd_weight[ParseStatus.Country] * session.carried_country.Count: 0);
            considerd_score += (session.is_considerd[ParseStatus.Genre] ? considerd_weight[ParseStatus.Genre] * session.carried_genre.Count: 0);
            considerd_score += (session.is_considerd[ParseStatus.PublishDate] ? considerd_weight[ParseStatus.PublishDate] : 0);
            considerd_score += (session.is_considerd[ParseStatus.Rating] ? considerd_weight[ParseStatus.Rating] : 0);
            considerd_score += (session.is_considerd[ParseStatus.Duration] ? considerd_weight[ParseStatus.Duration] : 0);
            //foreach (string entity in Entity.EntityList)
            //{
            //    if (session.is_considerd[entity])
            //    {
            //        considerd_score += considerd_weight[entity];
            //    }
            //}

            // too few movies to go on, so end
            if (session.candidate_movies.Count <= end_movie_count)
            {
                return true;
            }

            // enough information, so end
            if (considerd_score >= end_considerd_count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DialogFlow()
        {
            // begin
            Session session = new Session();
            Parser parser = new Parser();
            session.parse_status = ParseStatus.All;

            while (true)
            {
                // get query
                string query_str = Console.ReadLine();
                Query query = new Query(query_str);

                // movie recommendation trigger
                if (!parser.isAboutMovie(query))
                {
                    Console.WriteLine(new string('=', 24));
                    Console.WriteLine("\n");
                    continue;
                }

                // query parse according to parse status
                switch (session.parse_status)
                {
                    case ParseStatus.All:
                        parser.ParseAll(ref query);
                        break;
                    case ParseStatus.Movie:
                        parser.ParseMovieName(ref query);
                        break;
                    case ParseStatus.Artist:
                        parser.ParseArtistName(ref query);
                        break;
                    case ParseStatus.Director:
                        parser.ParseDirectorName(ref query);
                        break;
                    case ParseStatus.Country:
                        parser.ParseCountryName(ref query);
                        break;
                    case ParseStatus.Genre:
                        parser.ParseGenreName(ref query);
                        break;
                    case ParseStatus.PublishDate:
                        parser.ParsePublishDate(ref query);
                        break;
                    case ParseStatus.Rating:
                        parser.ParseRating(ref query);
                        break;
                    case ParseStatus.Duration:
                        parser.ParseDuration(ref query);
                        break;
                    default:
                        Console.WriteLine("error parse status!");
                        break;
                }

                // refresh session status using user query
                session.RefreshSessionStatus(query);
                DealArtistDirectorDuplicate(ref session);

                // refresh session movie candidate status 
                GetAllResult(ref session);

                // is end
                if (isDialogEnd(session))
                {
                    // if it is end, then get and show the final query result
                    List<string> movies = new List<string>();
                    int i = 0;
                    foreach (var item in session.candidate_movies)
                    {
                        if (i++ == 5)
                        {
                            break;
                        }
                        movies.Add(item.name);
                    }
                    Console.WriteLine(String.Join(", ", movies.ToArray()));
                    Console.WriteLine("\n");
                    break;
                }
                else
                {
                    // using current status to compute the next status, a transtion matrix requeried.
                    ParseStatus nextturn_status = GetTranstionStatus(session);
                    // response according to the nextturn_status we just chosen.
                    switch (nextturn_status)
                    {
                        case ParseStatus.Artist:
                            AnalyseArtistName(session);
                            break;
                        case ParseStatus.Director:
                            AnalyseDirectorName(session);
                            break;
                        case ParseStatus.Country:
                            AnalyseCountryName(session);
                            break;
                        case ParseStatus.Genre:
                            AnalyseGenreName(session);
                            break;
                        case ParseStatus.PublishDate:
                            AnalysePublishDate(session);
                            break;
                        default:
                            Console.WriteLine("error turn status!");
                            break;
                    }
                }
            }
        }

        public void TestDialogFlow(string query_str)
        {
            // begin
            Parser parser = new Parser();
            
            // get query
            Query query = new Query(query_str);
            Session session = new Session();

            // movie recommendation trigger
            if (!parser.isAboutMovie(query))
            {
                Console.WriteLine(new string('=', 24));
                Console.WriteLine("\n");
                return;
            }
            // query parse 
            parser.ParseAll(ref query);

            // refresh session status using user query
            session.RefreshSessionStatus(query);
            DealArtistDirectorDuplicate(ref session);

            // refresh session movie candidate status 
            GetAllResult(ref session);

            // end
            List<string> movies = new List<string>();
            int i = 0;
            foreach (var item in session.candidate_movies)
            {
                if (i++ == 5)
                {
                    break;
                }
                movies.Add(item.name);
            }
            Console.WriteLine(String.Join(", ", movies.ToArray()));
            Console.WriteLine("\n");
        }
        #endregion

        #region Make Transtion Status Decision 
        // using current status to compute the next status, a transtion matrix requeried.
        private ParseStatus GetTranstionStatus(Session session)
        {
            ParseStatus current_parsestatus = ParseStatus.All;
            foreach (var item in session.is_considerd)
            {
                if (item.Value)
                {
                    current_parsestatus = item.Key;
                    break;
                }
            }
            if (current_parsestatus == ParseStatus.All)
            {
                return ParseStatus.All;
            }
            double selecter = rand.NextDouble();
            List<double> current_type_trans = roulette_matrix[parsestatus2transmat[current_parsestatus]];
            int lower_bound = 0;
            foreach (double item in current_type_trans)
            {
                if (item > selecter)
                {
                    return transmat2parsestatus[lower_bound];
                }
                lower_bound++;
            }
            return ParseStatus.All;
        }

        public static ParseStatus TestTranstionStatus(Session session)
        {
            ParseStatus current_parsestatus = ParseStatus.All;
            foreach (var item in session.is_considerd)
            {
                if (item.Value)
                {
                    current_parsestatus = item.Key;
                    break;
                }
            }
            if (current_parsestatus == ParseStatus.All)
            {
                return ParseStatus.All;
            }
            double selecter = rand.NextDouble();
            List<double> current_type_trans = roulette_matrix[parsestatus2transmat[current_parsestatus]];
            Console.WriteLine(string.Join(", ", current_type_trans.ToArray()));
            int lower_bound = 0;
            foreach (double item in current_type_trans)
            {
                if (item > selecter)
                {
                    return transmat2parsestatus[lower_bound];
                }
                lower_bound++;
            }
            return ParseStatus.All;
        }
        #endregion

        #region Deal Artist Director Duplicate
        //wangbaoqiang as artist and director
        private static void DealArtistDirectorDuplicate(ref Session session)
        {
            List<string> duplicate_name = new List<string>();
            foreach (string art in session.carried_artist)
            {
                foreach (string dir in session.carried_director)
                {
                    if (art.Equals(dir))
                    {
                        duplicate_name.Add(art);
                    }
                }
            }
            if (duplicate_name.Count != 0)
            {
                foreach (string name in duplicate_name)
                {
                    List<string> osearch_query = OsearchQueryGenerator.GenerateSingleArtDirQuery(name);
                    var as_an_art = oSearchClient.Query(osearch_query[0]);
                    var as_a_dir = oSearchClient.Query(osearch_query[1]);
                    bool is_an_art = (as_an_art.Count() > as_a_dir.Count()) ? true : false;
                    if (is_an_art)
                    {
                        session.carried_director.Remove(name);
                    }
                    else
                    {
                        session.carried_artist.Remove(name);
                    }
                }
                if (session.carried_artist.Count == 0)
                {
                    session.is_considerd[ParseStatus.Artist] = false;
                }
                if (session.carried_director.Count == 0)
                {
                    session.is_considerd[ParseStatus.Director] = false;
                }
            }
        }
        #endregion

        #region Analyse Candidate Movies to Decide How We Show Machine's Answer
        private void AnalyseArtistName(Session session)
        {
            Dictionary<string, int> artist_appear = new Dictionary<string, int>();
            foreach (var movie in session.candidate_movies)
            {
                foreach(var artist in movie.artist)
                {
                    if (artist_appear.ContainsKey(artist))
                    {
                        artist_appear[artist]++;
                    }
                    else
                    {
                        artist_appear[artist] = 1;
                    }
                }
            }
            var m_list = artist_appear.ToList();
            m_list.Sort((first, second) => second.Value.CompareTo(first.Value));

        }

        private void AnalyseDirectorName(Session session)
        {
            Dictionary<string, int> director_appear = new Dictionary<string, int>();
            foreach (var movie in session.candidate_movies)
            {
                foreach (var director in movie.director)
                {
                    if (director_appear.ContainsKey(director))
                    {
                        director_appear[director]++;
                    }
                    else
                    {
                        director_appear[director] = 1;
                    }
                }
            }
            var m_list = director_appear.ToList();
            m_list.Sort((first, second) => second.Value.CompareTo(first.Value));
        }

        private void AnalyseCountryName(Session session)
        {
            Dictionary<string, int> country_appear = new Dictionary<string, int>();
            foreach (var movie in session.candidate_movies)
            {
                foreach (var country in movie.country)
                {
                    if (country_appear.ContainsKey(country))
                    {
                        country_appear[country]++;
                    }
                    else
                    {
                        country_appear[country] = 1;
                    }
                }
            }
            var m_list = country_appear.ToList();
            m_list.Sort((first, second) => second.Value.CompareTo(first.Value));
        }

        private void AnalyseGenreName(Session session)
        {
            Dictionary<string, int> genre_appear = new Dictionary<string, int>();
            foreach (var movie in session.candidate_movies)
            {
                foreach (var genre in movie.genre)
                {
                    if (genre_appear.ContainsKey(genre))
                    {
                        genre_appear[genre]++;
                    }
                    else
                    {
                        genre_appear[genre] = 1;
                    }
                }
            }
            var m_list = genre_appear.ToList();
            m_list.Sort((first, second) => second.Value.CompareTo(first.Value));
        }

        private void AnalysePublishDate(Session session)
        {
            Dictionary<string, int> publishDate_appear = new Dictionary<string, int>();
            foreach (var movie in session.candidate_movies)
            {

            }
        }
        #endregion

        #region Get All Result For Current Session Status, Update the Candidte Movie List
        // for final result show, when session is end
        private string GenerateAllOsearchQuery(Session session)
        {
            // if considerd movie name, then it is no necessary to conside the others
            if (session.is_considerd[ParseStatus.Movie])
            {
                string movie_query = string.Format("({0})", OsearchQueryGenerator.GenerateMovieNameQuery(session));
                return movie_query;
            }

            // conside the other filters
            List<string> osearch_query_list = new List<string>();
            if (session.is_considerd[ParseStatus.Artist])
            {
                string artist_query = string.Format("({0})", OsearchQueryGenerator.GenerateTypeQuery(session, "artist"));
                osearch_query_list.Add(artist_query);
            }
            if (session.is_considerd[ParseStatus.Director])
            {
                string director_query = string.Format("({0})", OsearchQueryGenerator.GenerateTypeQuery(session, "director"));
                osearch_query_list.Add(director_query);
            }
            if (session.is_considerd[ParseStatus.Country])
            {
                string country_query = string.Format("({0})", OsearchQueryGenerator.GenerateTypeQuery(session, "country"));
                osearch_query_list.Add(country_query);
            }
            if (session.is_considerd[ParseStatus.Genre])
            {
                string genre_query = string.Format("({0})", OsearchQueryGenerator.GenerateTypeQuery(session, "genre"));
                osearch_query_list.Add(genre_query);
            }
            if (session.is_considerd[ParseStatus.PublishDate])
            {
                string publishdate_query = string.Format("({0})", OsearchQueryGenerator.GeneratePublishDateQuery(session));
                osearch_query_list.Add(publishdate_query);
            }
            if (session.is_considerd[ParseStatus.Rating])
            {
                string rating_query = string.Format("({0})", OsearchQueryGenerator.GenerateRatingQuery(session));
                osearch_query_list.Add(rating_query);
            }
            if (session.is_considerd[ParseStatus.Duration])
            {
                string duration_query = string.Format("({0})", OsearchQueryGenerator.GenerateDurationQuery(session));
                osearch_query_list.Add(duration_query);
            }
            string[] osearch_query_arr = osearch_query_list.ToArray();
            string osearch_query = string.Join(" AND ", osearch_query_arr);
            return osearch_query;
        }

        private void GetAllResult(ref Session session)
        {
            string final_query = GenerateAllOsearchQuery(session);
            var results = oSearchClient.Query(final_query);
            List<MovieEntity> format_results = new List<MovieEntity>();
            foreach (var item in results)
            {
                format_results.Add(new MovieEntity(item));
            }
            session.candidate_movies = format_results.Distinct().ToList();
            session.candidate_movies.Sort();
        }
        #endregion
    }

    class OsearchQueryGenerator
    {
        public static Dictionary<string, string> type_osearchIndex = new Dictionary<string, string>()
        {
            { "movie", "Name" },
            { "artist", "Artists" },
            { "director", "Directors" },
            { "country", "Geographies" },
            { "genre", "Genres" },
            { "publishdate", "_PublishDate" },
            { "rating", "Rating" },
            { "duration", "Length" }
        };

        // query for single artist and director
        public static List<string> GenerateSingleArtDirQuery(string people_name)
        {
            List<string> ret = new List<string>();
            ret.Add(string.Format(@"#:""{0}Artists """, people_name));
            ret.Add(string.Format(@"#:""{0}Directors """, people_name));
            return ret;
        }

        // movie is different from the others, due to OR
        public static string GenerateMovieNameQuery(Session session)
        {
            List<string> filters_list = new List<string>();
            foreach (string item in session.carried_movie)
            {
                filters_list.Add(string.Format(@"#:""{0}{1} """, item, type_osearchIndex["movie"]));
            }
            string[] filters_arr = filters_list.ToArray();
            return string.Format("{0}", String.Join(" OR ", filters_arr));
        }

        // those type that can be concated by AND
        // include artist director country genre
        public static string GenerateTypeQuery(Session session, string type)
        {
            List<string> filters_list = new List<string>();
            List<string> carried_info;
            switch (type)
            {
                case "artist":
                    carried_info = session.carried_artist;
                    break;
                case "director":
                    carried_info = session.carried_director;
                    break;
                case "country":
                    carried_info = session.carried_country;
                    break;
                case "genre":
                    carried_info = session.carried_genre;
                    break;
                default:
                    Console.WriteLine("error type status!");
                    return "";
            }
            foreach (string item in carried_info)
            {
                filters_list.Add(string.Format(@"#:""{0}{1} """, item, type_osearchIndex[type]));
            }
            string[] filters_arr = filters_list.ToArray();
            return string.Format("{0}", String.Join(" AND ", filters_arr));
        }

        // for publishdate
        public static string GeneratePublishDateQuery(Session session)
        {
            int from = session.carried_publishdate.from;
            int to = session.carried_publishdate.to;
            string query = "";
            query = string.Format(@"rangeconstraint:bt:{0}:{1}:#:"" {2}""", from, to, type_osearchIndex["publishdate"]);
            return query;
        }

        // for rating
        public static string GenerateRatingQuery(Session session)
        {
            int low = session.carried_rating - 10;
            int high = session.carried_rating + 10;
            string query = string.Format(@"rangeconstraint:bt:{0}:{1}:#:"" {2}""",low,high,type_osearchIndex["rating"]);
            return query;
        }

        // for duration
        public static string GenerateDurationQuery(Session session)
        {
            int low = session.carried_rating - 30;
            int high = session.carried_rating + 10;
            string query = string.Format(@"rangeconstraint:bt:{0}:{1}:#:"" {2}""", low, high, type_osearchIndex["duration"]);
            return query;
        }
    }
}
