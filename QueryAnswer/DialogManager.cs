using System;
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
            { ParseStatus.Country, 50 },
            { ParseStatus.Genre, 50 },
            { ParseStatus.PublishDate, 100 },
            { ParseStatus.Rating, 10 },
            { ParseStatus.Duration, 20 }
        };

        private static int end_movie_count = 5;
        private static int end_considerd_count = 100;
        private static int condidate_show_number = 3;

        private static double[,] transition_matrix = new double[6, 6]
        { 
            ///////////// all       artist director  country      genre   publishdate
            /* all */     { 0.001,   0.1,    0.1,    0.2,         0.3,    0.3},
            /* artist */  { 0.001,   0.2,    0.2,    0.001,       0.3,    0.3},
            /* director */{ 0.001,   0.3,    0.001,  0.001,       0.4,    0.3},
            /* country */ { 0.001,   0.3,    0.3,    0.001,       0.2,    0.2},
            /* genre */   { 0.001,   0.4,    0.3,    0.2,         0.001,  0.1},
            /* publish */ { 0.001,   0.1,    0.2,    0.3,         0.4,    0.001}
        };

        private static List<List<double>> roulette_matrix = new List<List<double>>();

        public static Dictionary<ParseStatus, int> parsestatus2int = new Dictionary<ParseStatus, int>()
        {
            { ParseStatus.All, 0 },
            { ParseStatus.Artist, 1 },
            { ParseStatus.Director, 2 },
            { ParseStatus.Country, 3 },
            { ParseStatus.Genre, 4 },
            { ParseStatus.PublishDate, 5 }
        };
        public static Dictionary<int, ParseStatus> int2parsestatus = new Dictionary<int, ParseStatus>()
        {
            { 0, ParseStatus.All },
            { 1, ParseStatus.Artist },
            { 2, ParseStatus.Director },
            { 3, ParseStatus.Country },
            { 4, ParseStatus.Genre },
            { 5, ParseStatus.PublishDate }
        };

        private static Random rand = new Random();

        // for MakeClearParseStatus, this field records those status that allowed to be refined further(ask the user). The score it carried will be subtracted some value from considerd_weight when information came. When score <= 0, this status is not allowed to be refine(ask the user) anymore.
        // it is very useful for ParseStatus.All at the beginning of dialog.
        //private static Dictionary<ParseStatus, int> allow_refine = new Dictionary<ParseStatus, int>()
        //{
        //    { ParseStatus.Artist, 101 },
        //    { ParseStatus.Director, 101 },
        //    { ParseStatus.Country, 30 },
        //    { ParseStatus.Genre, 50 },
        //    { ParseStatus.PublishDate, 100 }
        //};
        
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
            considerd_score += (session.is_considerd[ParseStatus.Director] ? considerd_weight[ParseStatus.Director] * session.carried_director.Count : 0);
            considerd_score += (session.is_considerd[ParseStatus.Country] ? considerd_weight[ParseStatus.Country] * session.carried_country.Count : 0);
            considerd_score += (session.is_considerd[ParseStatus.Genre] ? considerd_weight[ParseStatus.Genre] * session.carried_genre.Count : 0);
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
            Parser parser = new Parser();
            Session session = new Session();
            session.parse_status = ParseStatus.All;
            string query_str = "";
            while (true)
            {
                Query query;
                // get query. if it is the very beginning, then taking the parameter as input
                if (session.parse_status == ParseStatus.All)
                {
                    query_str = Console.ReadLine();
                    query = new Query(query_str);
                    parser.PosTagging(ref query);
                    // movie recommendation trigger
                    if (!parser.isAboutMovie(query))
                    {
                        Console.WriteLine(new string('=', 24));
                        Console.WriteLine("\n");
                        return;
                    }
                }
                else
                {
                    query_str = Console.ReadLine();
                    query = new Query(query_str);
                    parser.PosTagging(ref query);
                }

                // query parse according to parse status
                switch (session.parse_status)
                {
                    case ParseStatus.All:
                        parser.ParseAllTag(ref query);
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
                        Utils.WriteError("error parse status!");
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
                    Utils.WriteResult(String.Join(", ", movies.ToArray()));
                    Console.WriteLine("\n");
                    break;
                }
                else
                {
                    if (session.parse_status == ParseStatus.All)
                    {
                        session.parse_status = MakeClearParseStatus(session);
                    }
                    // using current turn status(session.parse_status) to compute the Transition(next turn) status, a transition matrix requeried.
                    ParseStatus nextturn_status = GetTransitionStatus(session);
                    List<string> answer_entity_candidate = new List<string>();
                    // response according to the nextturn_status we just chosen.
                    switch (nextturn_status)
                    {
                        //case ParseStatus.All:
                        //    answer_entity_candidate = AnalyseAll(session);
                        //    break;
                        case ParseStatus.Artist:
                            answer_entity_candidate = AnalyseArtistName(session);
                            break;
                        case ParseStatus.Director:
                            answer_entity_candidate = AnalyseDirectorName(session);
                            break;
                        case ParseStatus.Country:
                            answer_entity_candidate = AnalyseCountryName(session);
                            break;
                        case ParseStatus.Genre:
                            answer_entity_candidate = AnalyseGenreName(session);
                            break;
                        case ParseStatus.PublishDate:
                            answer_entity_candidate = AnalysePublishDate(session);
                            break;
                        default:
                            Utils.WriteError("error turn status!");
                            break;
                    }
                    // answer and go to the next turn
                    Console.WriteLine("transite to {0}", nextturn_status.ToString());
                    string answer = AnswerGenerator.AnswerIt(answer_entity_candidate, session, nextturn_status);
                    Utils.WriteMachine(answer);
                    session.parse_status = nextturn_status;
                }
            }
        }

        public void TestDialogFlow(string query_str)
        {
            // begin
            Parser parser = new Parser();
            Session session = new Session();
            session.parse_status = ParseStatus.All;

            while (true)
            {
                Query query;
                // get query. if it is the very beginning, then taking the parameter as input
                if (session.parse_status == ParseStatus.All)
                {
                    query = new Query(query_str);
                    parser.PosTagging(ref query);
                    // movie recommendation trigger
                    if (!parser.isAboutMovie(query))
                    {
                        Console.WriteLine(new string('=', 24));
                        Console.WriteLine("\n");
                        return;
                    }
                }
                else
                {
                    query_str = Console.ReadLine();
                    query = new Query(query_str);
                    parser.PosTagging(ref query);
                }

                // query parse according to parse status
                switch (session.parse_status)
                {
                    case ParseStatus.All:
                        parser.ParseAllTag(ref query);
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
                        Utils.WriteError("error parse status!");
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
                    Utils.WriteResult(String.Join(", ", movies.ToArray()));
                    Console.WriteLine("\n");
                    break;
                }
                else
                {
                    if (session.parse_status == ParseStatus.All)
                    {
                        session.parse_status = MakeClearParseStatus(session);
                    }
                    // using current turn status(session.parse_status) to compute the Transition(next turn) status, a transition matrix requeried.
                    ParseStatus nextturn_status = GetTransitionStatus(session);
                    List<string> answer_entity_candidate = new List<string>();
                    // response according to the nextturn_status we just chosen.
                    switch (nextturn_status)
                    {
                        //case ParseStatus.All:
                        //    answer_entity_candidate = AnalyseAll(session);
                        //    break;
                        case ParseStatus.Artist:
                            answer_entity_candidate = AnalyseArtistName(session);
                            break;
                        case ParseStatus.Director:
                            answer_entity_candidate = AnalyseDirectorName(session);
                            break;
                        case ParseStatus.Country:
                            answer_entity_candidate = AnalyseCountryName(session);
                            break;
                        case ParseStatus.Genre:
                            answer_entity_candidate = AnalyseGenreName(session);
                            break;
                        case ParseStatus.PublishDate:
                            answer_entity_candidate = AnalysePublishDate(session);
                            break;
                        default:
                            Utils.WriteError("error turn status!");
                            break;
                    }
                    // answer and go to the next turn
                    Console.WriteLine("transite to {0}", nextturn_status.ToString());
                    string answer = AnswerGenerator.AnswerIt(answer_entity_candidate, session, nextturn_status);
                    Utils.WriteMachine(answer);
                    session.parse_status = nextturn_status;
                }
            }
        }

        public void TestOneTurnDialog(string query_str)
        {
            // begin
            Parser parser = new Parser();

            // get query
            Session session = new Session();
            Query query = new Query(query_str);
            parser.PosTagging(ref query);

            // movie recommendation trigger
            if (!parser.isAboutMovie(query))
            {
                Console.WriteLine(new string('=', 24));
                Console.WriteLine("\n");
                return;
            }
            // query parse 
            parser.ParseAllTag(ref query);

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
            Utils.WriteResult(String.Join(", ", movies.ToArray()));
            Console.WriteLine("\n");
        }
        #endregion

        private ParseStatus MakeClearParseStatus(Session session)
        {
            int start = (int)ParseStatus.Artist;
            int end = (int)ParseStatus.Rating;
            for (int i = start; i < end; i++)
            {
                if (session.is_considerd[(ParseStatus)i])
                {
                    return (ParseStatus)i;
                }
            }
            return ParseStatus.All;
        }

        #region Make Transition Status Decision 
        // using current status to compute the next status, a transition matrix requeried.
        private ParseStatus GetTransitionStatus(Session session)
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
            double selecter = rand.NextDouble();
            List<double> current_type_trans = roulette_matrix[parsestatus2int[current_parsestatus]];
            int lower_bound = 0;
            foreach (double item in current_type_trans)
            {
                if (item > selecter)
                {
                    return int2parsestatus[lower_bound];
                }
                lower_bound++;
            }
            return ParseStatus.All;
        }

        public static ParseStatus TestTransitionStatus(Session session)
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
            List<double> current_type_trans = roulette_matrix[parsestatus2int[current_parsestatus]];
            Console.WriteLine(string.Join(", ", current_type_trans.ToArray()));
            int lower_bound = 0;
            foreach (double item in current_type_trans)
            {
                if (item > selecter)
                {
                    return int2parsestatus[lower_bound];
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

        #region Analyse Candidate Movies to Decide How We Show Machine's Next Answer
        private List<string> TopNResult(Dictionary<string, int> type_appear)
        {
            List<KeyValuePair<string, int>> type_appear_list = type_appear.ToList();
            type_appear_list.Sort((first, second) => second.Value.CompareTo(first.Value));
            List<string> res = new List<string>();
            for (int i = 0; i < condidate_show_number; i++)
            {
                res.Add(type_appear_list[i].Key);
            }
            Console.WriteLine(string.Join(", ", res.ToArray()));
            return res;
        }

        private List<string> AnalyseAll(Session session)
        {
            return new List<string>() { "" };
        }

        private List<string> AnalyseArtistName(Session session)
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
            // need to remove duplicate artist within carried_artist
            foreach (string item in session.carried_artist)
            {
                if (artist_appear.ContainsKey(item))
                {
                    artist_appear.Remove(item);
                }
            }
            return TopNResult(artist_appear);
        }

        private List<string> AnalyseDirectorName(Session session)
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
            return TopNResult(director_appear);
        }

        private List<string> AnalyseCountryName(Session session)
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
            return TopNResult(country_appear);
        }

        private List<string> AnalyseGenreName(Session session)
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
            return TopNResult(genre_appear);
        }

        private List<string> AnalysePublishDate(Session session)
        {
            return new List<string>() { "" };
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

    class AnswerGenerator
    {
        private static string[,] relation_matrix = new string[6, 6]
        {
            ///////////// all   artist                            director                       country                         genre                          publishdate
            /* all */     {"X", "想看哪个演员的电影呢？",          "想看哪个导演的电影呢？",         "想看哪个国家的电影呢？",       "想看哪个类型的电影呢？",       "想看经典的还是最近的电影呢？"}, 
            
            /* artist */  {"X", "{0}和{1}有很多合作，想看谁的呢？",   "{0}和{1}有很多合作，想看谁的呢？",  "X",                    "{0}演了很多{1}，想看哪种呢？",       "他演了很多电影呢，想看经典的还是最近的呢？"}, 

            /* director */{"X", "{0}和{1}有很多合作，想看谁的呢？",   "X",                           "X",                           "{0}拍了很多{1}，想看哪种呢？",       "他拍了很多电影呢，想看经典的还是最近的呢？"}, 

            /* country */ {"X", "{0}有一些著名艺人：{1}，想看谁的呢？","{0}有一些著名导演：{1}，想看谁的呢？","X",                         "这个地区的{1}比较有名，想看哪种呢？",   "这个地区的电影有很多啦，想看经典的还是最近的呢？"}, 
              
            /* genre */   {"X", "{1}拍了很多{0}电影，想看谁的呢？",  "{1}拍了很多{0}电影，想看谁的呢？",  "{1}拍了很多{0}电影，想看哪里的呢？",  "X",                           "这种类型的电影有很多啦，想看经典的还是最近的呢？"}, 

            /* publish */ {"X", "X",                           "X",                            "X",                            "X",                            "X"}
        };

        public static string AnswerIt(List<string> answer_entity, Session session, ParseStatus to)
        {
            int from_status = DialogManager.parsestatus2int[session.parse_status];
            int to_status = DialogManager.parsestatus2int[to];
            string entity_in_question = "";
            switch (DialogManager.int2parsestatus[from_status])
            {
                case ParseStatus.All:
                    entity_in_question = string.Join("、 ", session.carried_artist.ToArray());
                    break;
                case ParseStatus.Artist:
                    entity_in_question = string.Join("、 ", session.carried_artist.ToArray());
                    break;
                case ParseStatus.Director:
                    entity_in_question = string.Join("、 ", session.carried_director.ToArray());
                    break;
                case ParseStatus.Country:
                    entity_in_question = string.Join("、 ", session.carried_country.ToArray());
                    break;
                case ParseStatus.Genre:
                    entity_in_question = string.Join("、 ", session.carried_genre.ToArray());
                    break;
                case ParseStatus.PublishDate:
                    // TODO
                    entity_in_question = "";
                    break;
                default:
                    Utils.WriteError("error turn status!");
                    break;
            }
            string answer = string.Format(relation_matrix[from_status,to_status], entity_in_question, string.Join("、 ", answer_entity.ToArray()));
            return answer;
        }
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

        // those type that can be concated by "AND"
        // include artist, director, country, genre
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
                    Utils.WriteError("error type status!");
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
