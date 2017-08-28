using ChinaOpalSearch;
using JiebaNet.Segmenter.PosSeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDialog
{
    class PublishDateType
    {
        public int from;
        public int to;
        public PublishDateType(int from, int to) { }
    }

    class MovieEntity : IEquatable<MovieEntity>, IComparable<MovieEntity>
    {
        public MovieEntity(SnappsEntity item)
        {
            name = item.Name;
            artist = item.Entment.Artists;
            director = item.Entment.Directors;
            country = item.Geographies;
            genre = item.Entment.Genres;
            publishdate = item.PublishDate;
            rating = item.Rating;
            duration = item.Length;
            number_reviewer = item.Rank;
        }

        public string name = "";
        public List<string> artist = new List<string>();
        public List<string> director = new List<string>();
        public List<string> country = new List<string>();
        public List<string> genre = new List<string>();
        public uint publishdate = 0;
        public uint rating = 0;
        public uint duration = 0;
        public uint number_reviewer = 0;
        public string douban_url = "";

        public bool Equals(MovieEntity other)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal. 
            return name.Equals(other.name);
        }

        public override int GetHashCode()
        {
            //Get hash code for the Name field if it is not null. 
            int hashProductName = name == null ? 0 : name.GetHashCode();
            //Calculate the hash code for the MovieEntity. 
            return hashProductName;
        }

        public int CompareTo(MovieEntity other)
        {
            if (other == null)
                return 0;

            else
                return other.number_reviewer.CompareTo(this.number_reviewer);
        }
    }

    class InformationSentence
    {
        // the filters for movie recommendation
        public Dictionary<ParseStatus, bool> is_considerd = new Dictionary<ParseStatus, bool>()
        {
            { ParseStatus.Movie, false },
            { ParseStatus.Artist, false },
            { ParseStatus.Director, false },
            { ParseStatus.Country, false },
            { ParseStatus.Genre, false },
            { ParseStatus.PublishDate, false },
            { ParseStatus.Rating, false },
            { ParseStatus.Duration, false }
        };

        public List<string> carried_movie = new List<string>();
        public List<string> carried_artist = new List<string>();
        public List<string> carried_director = new List<string>();
        public List<string> carried_country = new List<string>();
        public List<string> carried_genre = new List<string>();
        public PublishDateType carried_publishdate = new PublishDateType (DateTime.Now.Year, DateTime.Now.Year);
        public int carried_rating = 90;
        public int carried_duration = 120;
    }

    class Query : InformationSentence
    {
        public string raw_query;
        public List<string> wordbroken_query;

        // postagged query is the query that movie, person, ... entities have been tagged and replaced.
        public List<Pair> postag_pair;
        public string postagged_query;

        public Query(string query)
        {
            raw_query = query;
        }
    }

    class Session : InformationSentence
    {
        public List<Query> query_history = new List<Query>();
        public List<MovieEntity> candidate_movies = new List<MovieEntity>();
        public ParseStatus parse_status = ParseStatus.All;
        public int known_info_num = 0;

        // using a query and its carried status to update session status
        public void RefreshSessionStatus(Query query)
        {
            query_history.Add(query);
            if (query.is_considerd[ParseStatus.Movie])
            {
                carried_movie.AddRange(query.carried_movie);
                is_considerd[ParseStatus.Movie] = query.is_considerd[ParseStatus.Movie];
            }
            if (query.is_considerd[ParseStatus.Artist])
            {
                carried_artist.AddRange(query.carried_artist);
                is_considerd[ParseStatus.Artist] = query.is_considerd[ParseStatus.Artist];
            }
            if (query.is_considerd[ParseStatus.Director])
            {
                carried_director.AddRange(query.carried_director);
                is_considerd[ParseStatus.Director] = query.is_considerd[ParseStatus.Director];
            }
            if (query.is_considerd[ParseStatus.Country])
            {
                carried_country.AddRange(query.carried_country);
                is_considerd[ParseStatus.Country] = query.is_considerd[ParseStatus.Country];
            }
            if (query.is_considerd[ParseStatus.Genre])
            {
                carried_genre.AddRange(query.carried_genre);
                is_considerd[ParseStatus.Genre] = query.is_considerd[ParseStatus.Genre];
            }
            if (query.is_considerd[ParseStatus.PublishDate])
            {
                carried_publishdate = query.carried_publishdate;
                is_considerd[ParseStatus.PublishDate] = query.is_considerd[ParseStatus.PublishDate];
            }
            if (query.is_considerd[ParseStatus.Rating])
            {
                carried_rating = query.carried_rating;
                is_considerd[ParseStatus.Rating] = query.is_considerd[ParseStatus.Rating];
            }
            if (query.is_considerd[ParseStatus.Duration])
            {
                carried_duration = query.carried_duration;
                is_considerd[ParseStatus.Duration] = query.is_considerd[ParseStatus.Duration];
            }

            // DealArtistDirectorDuplicate();
            RefreshKnownInfoNum();
        }

        // wangbaoqiang as artist and director
        // DEPRECATED, use DialogManager.ClarifyArtistDirector instead
        private void DealArtistDirectorDuplicate()
        {
            List<string> duplicate_name = new List<string>();
            foreach (string art in carried_artist)
            {
                foreach (string dir in carried_director)
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
                    List<string> osearch_query = oSearchQueryGenerator.GenerateSingleArtDirQuery(name);
                    //var as_an_art = oSearchClient.Query(osearch_query[0]);
                    var as_an_art = SearchObjectStoreClient.Query(osearch_query[0]);
                    //var as_a_dir = oSearchClient.Query(osearch_query[1]);
                    var as_a_dir = SearchObjectStoreClient.Query(osearch_query[1]);
                    bool is_an_art = (as_an_art.Count() > as_a_dir.Count()) ? true : false;
                    if (is_an_art)
                    {
                        carried_director.Remove(name);
                    }
                    else
                    {
                        carried_artist.Remove(name);
                    }
                }
                if (carried_artist.Count == 0)
                {
                    is_considerd[ParseStatus.Artist] = false;
                }
                if (carried_director.Count == 0)
                {
                    is_considerd[ParseStatus.Director] = false;
                }
            }
        }

        private void RefreshKnownInfoNum()
        {
            known_info_num = 0;
            known_info_num += (is_considerd[ParseStatus.Artist] ?   carried_artist.Count : 0);
            known_info_num += (is_considerd[ParseStatus.Director] ? carried_director.Count : 0);
            known_info_num += (is_considerd[ParseStatus.Country] ?  carried_country.Count : 0);
            known_info_num += (is_considerd[ParseStatus.Genre] ?    carried_genre.Count : 0);
            known_info_num += (is_considerd[ParseStatus.PublishDate] ? 1 : 0);
        }
    }
}

