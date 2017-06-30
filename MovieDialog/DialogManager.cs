using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChinaOpalSearch;

namespace MovieDialog
{
    class DialogManager
    {
        public static Dictionary<ParseStatus, int> considerd_weight = new Dictionary<ParseStatus, int>()
        {
            { ParseStatus.Movie, int.MaxValue },
            { ParseStatus.Artist, 50 },
            { ParseStatus.Director, 50 },
            { ParseStatus.Country, 50 },
            { ParseStatus.Genre, 50 },
            { ParseStatus.PublishDate, 100 },
            { ParseStatus.Rating, 10 },
            { ParseStatus.Duration, 20 }
        };

        private static int end_movie_count = 5;
        private static int end_considerd_count = 200;
        private static int end_known_info_count = 3;
        private static int cue_condidate_show_number = 3;
        private static int condidate_movie_show_number = 10;

        private static double[,] transition_matrix = new double[6, 6]
        {
            ///////////// all    artist director  country      genre   publishdate
            /* all */     { 0.0,   0.1,    0.1,    0.2,         0.3,    0.3},
            /* artist */  { 0.0,   0.2,    0.2,    0.0,         0.3,    0.3},
            /* director */{ 0.0,   0.3,    0.0,    0.0,         0.4,    0.3},
            /* country */ { 0.0,   0.3,    0.3,    0.0,         0.2,    0.2},
            /* genre */   { 0.0,   0.4,    0.3,    0.2,         0.0,    0.1},
            /* publish */ { 0.0,   0.2,    0.1,    0.3,         0.5,    0.0}
        };

        private double[,] bk = new double[15, 5]
            {
            ////////////////////// artist   director  country  genre   publishdate
            /* artist artist */  	{ 0.001,  0.2,    0.001,    0.3,    0.3},
            /* artist director */ 	{ 0.001,  0.001,  0.001,    0.4,    0.3},
            /* artist country */ 	{ 0.001,  0.3,    0.001,    0.2,    0.2},
            /* artist genre */   	{ 0.001,  0.3,    0.001,    0.001,  0.1},
            /* artist publish */ 	{ 0.001,  0.2,    0.001,    0.4,    0.001},
            /* director director */ { 0.3,    0.001,  0.001,    0.4,    0.3},
            /* director country */ 	{ 0.3,    0.001,  0.001,    0.2,    0.2},
            /* director genre */   	{ 0.4,    0.001,  0.001,    0.001,  0.1},
            /* director publish */ 	{ 0.1,    0.001,  0.001,    0.4,    0.001},
            /* country country */ 	{ 0.3,    0.3,    0.001,    0.2,    0.2},
            /* country genre */   	{ 0.4,    0.3,    0.001,    0.001,  0.1},
            /* country publish */ 	{ 0.1,    0.2,    0.001,    0.4,    0.001},
            /* genre genre */   	{ 0.4,    0.3,    0.2,      0.001,  0.1},
            /* genre publish */ 	{ 0.1,    0.2,    0.3,      0.001,  0.001},
            /* publish publish */ 	{ 0.1,    0.2,    0.3,      0.4,    0.001}
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

        Parser parser = new Parser();
        Session session = new Session();
        PatternBased pattern_qa = new PatternBased();
        CNNBased cnn_qa = new CNNBased();
        GraphEngineQuery graphengine_query = new GraphEngineQuery();


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
            Utils.WriteMachine(" ");
            Utils.WriteMachine("=========================");
            Utils.WriteMachine("Now, you can talk to me~");
        }

        #region Dialog Logic
        // judge is this dialog should be ended
        public bool isRecommendationEnd(Session session)
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
            if (considerd_score >= end_considerd_count || session.known_info_num >= end_known_info_count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DialogFlow(string input)
        {
            // begin
            session.parse_status = ParseStatus.All;
            string query_str = "";
            while (true)
            {
                Query query;
                // get query. if it is the very beginning, then taking the parameter as input
                if (session.parse_status == ParseStatus.All)
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        query_str = Console.ReadLine();
                        Utils.WriteQuery(query_str);
                    }
                    else
                    {
                        query_str = input;
                        input = null;
                    }
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
                    Utils.WriteQuery(query_str);
                    query = new Query(query_str);

                    Query query_kbqa = new Query(query_str);
                    parser.PosTagging(ref query_kbqa);
                    if (KBQA(query_kbqa))
                    {
                        continue;
                    }
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
                    //case ParseStatus.Rating:
                    //    parser.ParseRating(ref query);
                    //    break;
                    //case ParseStatus.Duration:
                    //    parser.ParseDuration(ref query);
                    //    break;
                    default:
                        Utils.WriteError("error parse status!");
                        break;
                }

                // refresh session status using user query
                session.RefreshSessionStatus(query);
                ClarifyArtistDirector();

                // refresh session movie candidate status 
                GetAllResult(ref session);

                // is recommendation end or is user accept the kbqa answer
                if (isRecommendationEnd(session) || parser.isAcceptCandidate(query))
                {
                    if (parser.isAcceptCandidate(query))
                    {
                        Utils.WriteMachine("Have Fun~~~");
                        return;
                    }
                    if (session.is_considerd[ParseStatus.Movie] && session.candidate_movies.Count > 0)
                    {
                        Utils.WriteResult("想看" + session.candidate_movies[0].name + "啊");
                        Utils.WriteResult("眼光不错哦");
                    }
                    else
                    {
                        // if it is end, then show some movie candidate, let user confirm the result
                        ConfirmSession();
                        return;
                    }
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
                    Console.WriteLine("from {0} transite to {1}", session.parse_status.ToString(), nextturn_status.ToString());
                    string answer = AnswerGenerator.AnswerIt(answer_entity_candidate, session, nextturn_status);
                    Utils.WriteMachine(answer);
                    session.parse_status = nextturn_status;
                }
            }
        }

        private void ConfirmSession()
        {
            Console.WriteLine("Going to Confirm Session");
            Utils.WriteMachine("我知道你想看什么啦");
            bool jump_show_candidate_dueto_kbqa = false;
            int offset = 0;
            int confirm_turn = 1;
            while (true)
            {
                if (!jump_show_candidate_dueto_kbqa)
                {
                    if (confirm_turn > 3)
                    {
                        break;
                    }
                    List<string> movies = new List<string>();
                    for (int j = offset; j < offset + confirm_turn && j < session.candidate_movies.Count; j++)
                    {
                        movies.Add(session.candidate_movies[j].name);
                    }
                    if (movies.Count < 1)
                    {
                        break;
                    }
                    Utils.WriteMachine("你想看 " + String.Join(" 或者 ", movies.ToArray()) + " 吗");
                    confirm_turn++;
                    offset += confirm_turn;
                }

                string query_str = Console.ReadLine();
                Utils.WriteQuery(query_str);
                Query query = new Query(query_str);
                parser.PosTagging(ref query);

                if (parser.isDenyCandidate(query))
                {
                    jump_show_candidate_dueto_kbqa = false;
                    continue;
                }
                if (parser.isAcceptCandidate(query))
                {
                    Utils.WriteMachine("Have Fun~~~");
                    return;
                }
                if (KBQA(query))
                {
                    jump_show_candidate_dueto_kbqa = true;
                }
                else
                {
                    jump_show_candidate_dueto_kbqa = true;
                    Utils.WriteResult("数据库中没有相关的答案...");
                }
            }
            Utils.WriteMachine("好像找不到你喜欢看的电影...");
        }

        private bool KBQA(Query query)
        {
            parser.ParseAllTag(ref query);
            PatternResponse pattern_response;
            if (pattern_qa.QuestionClassify(query, out pattern_response) || cnn_qa.QuestionClassify(query, out pattern_response))
            {
                Console.WriteLine("Start to KBQA");
                string question_topic = "";
                try
                {
                    switch (pattern_response.entity_type)
                    {
                        case EntityType.Movie:
                            question_topic = query.carried_movie[0];
                            break;
                        case EntityType.Celebrity:
                            question_topic = (query.carried_artist.Count > 0) ? query.carried_artist[0] : query.carried_director[0];
                            break;
                    }
                    List<object> res = graphengine_query.GetGraphEngineData(question_topic, pattern_response.property, pattern_response.hop_num);
                    string answer = string.Join(",", res.ToArray());
                    if (answer.Length < 2)
                    {
                        Utils.WriteResult("数据库中没有相关的答案...");
                    }
                    else
                    {
                        Utils.WriteResult(answer);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Utils.WriteError("It seems Neural Network makes a mistake");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void ClarifyArtistDirector()
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
                Console.WriteLine("Start to Clarify Artist Director");
                foreach (string name in duplicate_name)
                {
                    Utils.WriteMachine($"QAQ有一点儿疑惑...，因为 {name} 既是演员也是导演呢...");
                    while (true)
                    {
                        Utils.WriteMachine("你想看他拍的还是他演的呢?");
                        string query_str = Console.ReadLine();
                        Query query = new Query(query_str);
                        parser.PosTagging(ref query);
                        parser.ParseAllTag(ref query);
                        int is_artist_director = parser.isArtistOrDirector(query);
                        if (is_artist_director != -1)
                        {
                            if (is_artist_director == 1)
                            {
                                session.carried_director.Remove(name);
                            }
                            else
                            {
                                if (is_artist_director == 2)
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
                            break;
                        }
                        if (KBQA(query))
                        {
                            continue;
                        }
                    }
                }
            }
        }

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
        #endregion

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
            ClarifyArtistDirector();

            // refresh session movie candidate status 
            GetAllResult(ref session);

            // end
            List<string> movies = new List<string>();
            int i = 0;
            foreach (var item in session.candidate_movies)
            {
                if (i++ == condidate_movie_show_number)
                {
                    break;
                }
                movies.Add(item.name);
            }
            Utils.WriteResult(String.Join(", ", movies.ToArray()));
            Console.WriteLine("\n");
        }

        #region Make Transition Status Decision 
        // using current status to compute the next status, a transition matrix requeried.
        public ParseStatus GetTransitionStatus(Session session)
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
        #endregion

        #region Analyse Candidate Movies to Decide How We Show Machine's Next Answer
        private List<string> TopNResult(Dictionary<string, int> type_appear)
        {
            List<KeyValuePair<string, int>> type_appear_list = type_appear.ToList();
            type_appear_list.Sort((first, second) => second.Value.CompareTo(first.Value));
            List<string> res = new List<string>();
            for (int i = 0; i < cue_condidate_show_number; i++)
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
                foreach (var artist in movie.artist)
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
                string movie_query = string.Format("({0})", oSearchQueryGenerator.GenerateMovieNameQuery(session));
                return movie_query;
            }

            // conside the other filters
            List<string> osearch_query_list = new List<string>();
            if (session.is_considerd[ParseStatus.Artist])
            {
                string artist_query = string.Format("({0})", oSearchQueryGenerator.GenerateTypeQuery(session, "artist"));
                osearch_query_list.Add(artist_query);
            }
            if (session.is_considerd[ParseStatus.Director])
            {
                string director_query = string.Format("({0})", oSearchQueryGenerator.GenerateTypeQuery(session, "director"));
                osearch_query_list.Add(director_query);
            }
            if (session.is_considerd[ParseStatus.Country])
            {
                string country_query = string.Format("({0})", oSearchQueryGenerator.GenerateTypeQuery(session, "country"));
                osearch_query_list.Add(country_query);
            }
            if (session.is_considerd[ParseStatus.Genre])
            {
                string genre_query = string.Format("({0})", oSearchQueryGenerator.GenerateTypeQuery(session, "genre"));
                osearch_query_list.Add(genre_query);
            }
            if (session.is_considerd[ParseStatus.PublishDate])
            {
                string publishdate_query = string.Format("({0})", oSearchQueryGenerator.GeneratePublishDateQuery(session));
                osearch_query_list.Add(publishdate_query);
            }
            if (session.is_considerd[ParseStatus.Rating])
            {
                string rating_query = string.Format("({0})", oSearchQueryGenerator.GenerateRatingQuery(session));
                osearch_query_list.Add(rating_query);
            }
            if (session.is_considerd[ParseStatus.Duration])
            {
                string duration_query = string.Format("({0})", oSearchQueryGenerator.GenerateDurationQuery(session));
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

}
