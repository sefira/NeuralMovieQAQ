using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryAnswer
{
    class Query
    {
        public string RawQuery;
        public List<string> NormalizedQuery;
        public List<string> WordBrokenQuery;

        public Query(string query)
        {
            RawQuery = query;
        }

        // the filters for movie recommendation
        public List<string> carried_movie = new List<string>();
        public bool is_movie_considerd = false;
        public List<string> carried_artist = new List<string>();
        public bool is_artist_considerd = false;
        public List<string> carried_director = new List<string>();
        public bool is_director_considerd = false;
        public List<string> carried_country = new List<string>();
        public bool is_countrye_considerd = false;
        public List<string> carried_genre = new List<string>();
        public bool is_genre_considerd = false;
        public string[] carried_publishdate = new string[2] { DateTime.Today.Year.ToString(),"round" };// round, before, after
        public bool is_publishdate_considerd = false;
        public int carried_rating = 90;
        public bool is_rating_considerd = true;
        public int carried_duration = 120;
        public bool is_duration_considerd = false;
    }

    class Session : Query
    {
        private List<Query> query_history;

        public Session(string query = "") : base(query)
        {
        }

        // using a query and its carried status to update session status
        public void RefreshSessionStatus(Query query)
        {

        }
    }
}

