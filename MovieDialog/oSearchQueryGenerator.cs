using System;
using System.Collections.Generic;

namespace MovieDialog
{
    class oSearchQueryGenerator
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
            string query = string.Format(@"rangeconstraint:bt:{0}:{1}:#:"" {2}""", low, high, type_osearchIndex["rating"]);
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
