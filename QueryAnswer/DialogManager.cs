using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryAnswer
{
    class DialogManager
    {
        public Dictionary<string, int> considerd_weight = new Dictionary<string, int>()
        {
            { "movie", 100 },
            { "artist", 50 },
            { "director", 50 },
            { "country", 30 },
            { "genre", 50 },
            { "publishdate", 100 },
            { "rating", 10 },
            { "duration", 20 }
        };

        // judge is this dialog should be ended
        public bool isDialogEnd(Session session)
        {
            int considerd_score = 0;
            foreach (string entity in Entity.EntityList)
            {
                if (session.is_considerd[entity])
                {
                    considerd_score += considerd_weight[entity];
                }
            }
            if (considerd_score >= 100)
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
            Session m_session = new Session();
            Parser m_parser = new Parser();
            string parse_status = "all";

            while (true)
            {
                // get query
                string query_str = Console.ReadLine();
                Query query = new Query(query_str);
                
                // query parse according to parse status
                switch (parse_status)
                {
                    case "all":
                        m_parser.ParseAll(ref query);
                        break;
                    case "movie":
                        m_parser.ParseMovieName(ref query);
                        break;
                    case "artist":
                        m_parser.ParseArtistName(ref query);
                        break;
                    case "director":
                        m_parser.ParseDirectorName(ref query);
                        break;
                    case "country":
                        m_parser.ParseCountryName(ref query);
                        break;
                    case "genre":
                        m_parser.ParseGenreName(ref query);
                        break;
                    case "publishdate":
                        m_parser.ParsePublishDate(ref query);
                        break;
                    case "rating":
                        m_parser.ParseRating(ref query);
                        break;
                    case "duration":
                        m_parser.ParseDuration(ref query);
                        break;
                    default:
                        Console.WriteLine("error parse status!");
                        break;
                }

                // refresh session status
                m_session.RefreshSessionStatus(query);

                // is end
                if (isDialogEnd(m_session))
                {
                    // if it is end, then get and show the final query result
                    List<string> movie_names = GetFinalResult(m_session);
                    foreach (string movie_name in movie_names)
                    {
                        Console.WriteLine(movie_name);
                    }
                    break;
                }
                else
                {

                }
            }
        }

        private string GenerateFinalOsearchQuery(Session session)
        {
            // if considerd movie name, then it is no necessary to conside the others
            if (session.is_considerd["movie"])
            {
            }

            // con
            List<string> total_filters = new List<string>();
            if (session.is_considerd["artist"])
            {
                List<string> filters_list = new List<string>();
                foreach (string item in session.carried_movie)
                {
                    filters_list.Add(string.Format(@"#:""{0}Name """, item));
                }
                string[] filters_arr = filters_list.ToArray();
                total_filters.Add(string.Format("({0})", String.Join(" OR ", filters_arr)));
            }
            if (session.is_considerd["director"])
            {
            }
            if (session.is_considerd["country"])
            {
            }
            if (session.is_considerd["genre"])
            {
            }
            if (session.is_considerd["publishdate"])
            {
            }
            if (session.is_considerd["rating"])
            {
            }
            if (session.is_considerd["duration"])
            {
            }
        }
        private List<string> GetFinalResult(Session session)
        {
            string final_query = GenerateFinalOsearchQuery(session);
                return null;
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
            { "publishdate", "PublishDate" },
            { "rating", "Rating" },
            { "duration", "Length" }
        };

        // movie is different from the others, due to OR
        public static string GenerateMovieNameQuery(Session session)
        {
            List<string> filters_list = new List<string>();
            foreach (string item in session.carried_movie)
            {
                filters_list.Add(string.Format(@"#:""{0}{1} """, item, type_osearchIndex["movie"]));
            }
            string[] filters_arr = filters_list.ToArray();
            return string.Format("({0})", String.Join(" OR ", filters_arr));
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
                    carried_info = session.carried_artist;
                    break;
                case "country":
                    carried_info = session.carried_artist;
                    break;
                case "genre":
                    carried_info = session.carried_artist;
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
            return string.Format("({0})", String.Join(" AND ", filters_arr));
        }

        // for publishdate
        public static string GeneratePublishDateQuery(Session session)
        {
            int year = session.carried_publishdate.year;
            DateType type = session.carried_publishdate.type;
            int low = 0;
            int high = 0;
            string query = "";
            switch (type)
            {
                case DateType.round:
                    low = session.carried_publishdate.year * 10000;
                    high = (session.carried_publishdate.year + 10) * 10000;
                    query = string.Format(@"rangeconstraint:bt:{0}:{1}:#:\"" {2}\""", low, high, type_osearchIndex["publishdate"]);
                    return query;
                case DateType.before:
                    low = session.carried_publishdate.year * 10000;
                    query = string.Format(@"rangeconstraint:lt:{0}:#:\"" {1}\""", low, type_osearchIndex["publishdate"]);
                    return query;
                case DateType.after:
                    high = session.carried_publishdate.year * 10000;
                    query = string.Format(@"rangeconstraint:gt:{0}:#:\"" {1}\""", high, type_osearchIndex["publishdate"]);
                    return query;
                case DateType.exact:
                    low = session.carried_publishdate.year * 10000;
                    high = (session.carried_publishdate.year + 1) * 10000;
                    query = string.Format(@"rangeconstraint:bt:{0}:{1}:#:\"" {2}\""", low, high, type_osearchIndex["publishdate"]);
                    return query;
                default:
                    Console.WriteLine("date type error!");
                    return "";
            }
        }

        // for rating
        public static string GenerateRatingQuery(Session session)
        {
            int low = session.carried_rating - 10;
            int high = session.carried_rating + 10;
            string query = string.Format(@"rangeconstraint:bt:{0}:{1}:#:\"" {3}\""",low,high,type_osearchIndex["rating"]);
            return query;
        }

        // for duration
        public static string GenerateDurationQuery(Session session)
        {
            int low = session.carried_rating - 30;
            int high = session.carried_rating + 10;
            string query = string.Format(@"rangeconstraint:bt:{0}:{1}:#:\"" {3}\""", low, high, type_osearchIndex["duration"]);
            return query;
        }
    }
}
