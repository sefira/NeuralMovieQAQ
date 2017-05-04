﻿using System;
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
            // number of results < 10 then end
            // TODO
            //
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
            Session session = new Session();
            Parser parser = new Parser();
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
                        parser.ParseAll(ref query);
                        break;
                    case "movie":
                        parser.ParseMovieName(ref query);
                        break;
                    case "artist":
                        parser.ParseArtistName(ref query);
                        break;
                    case "director":
                        parser.ParseDirectorName(ref query);
                        break;
                    case "country":
                        parser.ParseCountryName(ref query);
                        break;
                    case "genre":
                        parser.ParseGenreName(ref query);
                        break;
                    case "publishdate":
                        parser.ParsePublishDate(ref query);
                        break;
                    case "rating":
                        parser.ParseRating(ref query);
                        break;
                    case "duration":
                        parser.ParseDuration(ref query);
                        break;
                    default:
                        Console.WriteLine("error parse status!");
                        break;
                }

                // refresh session status
                session.RefreshSessionStatus(query);
                DealArtistDirectorDuplicate(ref session);
                // is end
                if (isDialogEnd(session))
                {
                    // if it is end, then get and show the final query result
                    List<string> movies = GetAllResult(session);
                    Console.WriteLine(String.Join(", ", movies.ToArray()));
                    break;
                }
                else
                {

                }
            }
        }

        //wangbaoqiang as artist and director
        private void DealArtistDirectorDuplicate(ref Session session)
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
                    session.is_considerd["artist"] = false;
                }
                if (session.carried_director.Count == 0)
                {
                    session.is_considerd["director"] = false;
                }
            }
        }

        // for final result show, when session is end
        private string GenerateAllOsearchQuery(Session session)
        {
            // if considerd movie name, then it is no necessary to conside the others
            if (session.is_considerd["movie"])
            {
                string movie_query = string.Format("({0})", OsearchQueryGenerator.GenerateMovieNameQuery(session));
                return movie_query;
            }

            // conside the other filters
            List<string> osearch_query_list = new List<string>();
            if (session.is_considerd["artist"])
            {
                string artist_query = string.Format("({0})", OsearchQueryGenerator.GenerateTypeQuery(session, "artist"));
                osearch_query_list.Add(artist_query);
            }
            if (session.is_considerd["director"])
            {
                string director_query = string.Format("({0})", OsearchQueryGenerator.GenerateTypeQuery(session, "director"));
                osearch_query_list.Add(director_query);
            }
            if (session.is_considerd["country"])
            {
                string country_query = string.Format("({0})", OsearchQueryGenerator.GenerateTypeQuery(session, "country"));
                osearch_query_list.Add(country_query);
            }
            if (session.is_considerd["genre"])
            {
                string genre_query = string.Format("({0})", OsearchQueryGenerator.GenerateTypeQuery(session, "genre"));
                osearch_query_list.Add(genre_query);
            }
            if (session.is_considerd["publishdate"])
            {
                string publishdate_query = string.Format("({0})", OsearchQueryGenerator.GeneratePublishDateQuery(session));
                osearch_query_list.Add(publishdate_query);
            }
            if (session.is_considerd["rating"])
            {
                string rating_query = string.Format("({0})", OsearchQueryGenerator.GenerateRatingQuery(session));
                osearch_query_list.Add(rating_query);
            }
            if (session.is_considerd["duration"])
            {
                string duration_query = string.Format("({0})", OsearchQueryGenerator.GenerateDurationQuery(session));
                osearch_query_list.Add(duration_query);
            }
            string[] osearch_query_arr = osearch_query_list.ToArray();
            string osearch_query = string.Join(" AND ", osearch_query_arr);
            return osearch_query;
        }

        private List<string> GetAllResult(Session session)
        {
            string final_query = GenerateAllOsearchQuery(session);
            var results = oSearchClient.Query(final_query);
            List<string> format_results = new List<string>();
            foreach (var item in results)
            {
                format_results.Add(item.Name);
            }
            return format_results;
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
            string query = string.Format(@"rangeconstraint:bt:{0}:{1}:#:\"" {2}\""",low,high,type_osearchIndex["rating"]);
            return query;
        }

        // for duration
        public static string GenerateDurationQuery(Session session)
        {
            int low = session.carried_rating - 30;
            int high = session.carried_rating + 10;
            string query = string.Format(@"rangeconstraint:bt:{0}:{1}:#:\"" {2}\""", low, high, type_osearchIndex["duration"]);
            return query;
        }
    }
}
