using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryAnswer
{
    enum DateType { exact, round, before, after };

    class PublishDateType
    {
        public int year;
        public DateType type;
        public PublishDateType(int y, DateType type) { }
    }

    class InformationSentence
    {
        // the filters for movie recommendation
        public Dictionary<string, bool> is_considerd = new Dictionary<string, bool>()
        {
            { "movie", false },
            { "artist", false },
            { "director", false },
            { "country", false },
            { "genre", false },
            { "publishdate", false },
            { "rating", true },
            { "duration", false }
        };

        public List<string> carried_movie = new List<string>();
        public List<string> carried_artist = new List<string>();
        public List<string> carried_director = new List<string>();
        public List<string> carried_country = new List<string>();
        public List<string> carried_genre = new List<string>();
        public PublishDateType carried_publishdate = new PublishDateType (DateTime.Now.Year, DateType.round);
        public int carried_rating = 90;
        public int carried_duration = 120;

        // can not put them into a dictionary due to their different behaviour
        //public Dictionary<string, object> carried_info = new Dictionary<string, object>()
        //{
        //    { "movie", new List<string>() },
        //    { "artist", new List<string>() },
        //    { "director", new List<string>() },
        //    { "country", new List<string>() },
        //    { "genre", new List<string>() },
        //    { "publishdate", new string[2] { DateTime.Today.Year.ToString(), "round" }},// round, before, after 
        //    { "rating", 90 },
        //    { "duration", 120 }
        //};
    }

    class Query : InformationSentence
    {
        public string RawQuery;
        public List<string> NormalizedQuery;
        public List<string> WordBrokenQuery;

        public Query(string query)
        {
            RawQuery = query;
        }
    }

    class Session : InformationSentence
    {
        private List<Query> query_history;

        // using a query and its carried status to update session status
        public void RefreshSessionStatus(Query query)
        {
            query_history.Add(query);
            if (query.is_considerd["movie"])
            {
                carried_movie = query.carried_movie;
                is_considerd["movie"] = query.is_considerd["movie"];
            }
            if (query.is_considerd["artist"])
            {
                carried_artist = query.carried_artist;
                is_considerd["artist"] = query.is_considerd["artist"];
            }
            if (query.is_considerd["director"])
            {
                carried_director = query.carried_director;
                is_considerd["director"] = query.is_considerd["director"];
            }
            if (query.is_considerd["country"])
            {
                carried_country = query.carried_country;
                is_considerd["country"] = query.is_considerd["country"];
            }
            if (query.is_considerd["genre"])
            {
                carried_genre = query.carried_genre;
                is_considerd["genre"] = query.is_considerd["genre"];
            }
            if (query.is_considerd["publishdate"])
            {
                carried_publishdate = query.carried_publishdate;
                is_considerd["publishdate"] = query.is_considerd["publishdate"];
            }
            if (query.is_considerd["rating"])
            {
                carried_rating = query.carried_rating;
                is_considerd["rating"] = query.is_considerd["rating"];
            }
            if (query.is_considerd["duration"])
            {
                carried_duration = query.carried_duration;
                is_considerd["duration"] = query.is_considerd["duration"];
            }
        }
    }
}

