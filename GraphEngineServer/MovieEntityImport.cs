using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;

namespace GraphEngineServer
{
    class MovieEntityImport
    {
        public string path;
        private string person_cellid_dict_filename = "person_cellid.dict";

        enum MovieFieldType { Key, KGId, Genres, Artists, Directors, Characters, Performance, Distributors, Channels, Albums, Name, Alias, Description, Segments, Categories, IntEmbeddedFilters, NumberOfWantToWatch, Rating, NumberOfShortReview, ReviewCount, NumberOfWatched, NumberOfReviewer, PublishDate, Length, Country, Language, SourceUrls, ImageUrls, OfficialSite, EntityContainer, Logo, QueryRank };

        enum PersonFieldType { Name, Gender, Married, Spouse, Act, Direct };

        Dictionary<string, long> person_cellid = new Dictionary<string, long>();

        #region constructor & destructor function
        private void ReadDictionary()
        {
            string line = "";
            try
            {
                StreamReader sr = new StreamReader(path + person_cellid_dict_filename);
                while ((line = sr.ReadLine()) != null)
                {
                    string[] line_arr = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    person_cellid[line_arr[0]] = long.Parse(line_arr[1]);
                }
            }
            catch
            {
                Console.WriteLine("Dictionary doesn't exist");
            }
        }

        private void WriteDictionary()
        {
            using (StreamWriter sw = new StreamWriter(path + person_cellid_dict_filename))
            {
                foreach (var item in person_cellid)
                {
                    sw.WriteLine(item.Key + "\t" + item.Value);
                }
            }
        }

        public MovieEntityImport()
        {
            //ReadDictionary();
        }

        public MovieEntityImport(string str)
        {
            path = str;
            //ReadDictionary();
        }

        ~MovieEntityImport()
        {
            WriteDictionary();
        }
        #endregion 

        public List<string> GetListFromString(string str)
        {
            return new List<string>(str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public List<long> GetArtistsFromString(string str, long this_movie)
        {
            List<string> artists = new List<string>(str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToList();
            List<long> persons = new List<long>();

            foreach (string this_name in artists)
            {
                // for each parsed person name, we look whether this person exist already,
                // if exist, load and return CellID directly;
                // if not, new this person and save.
                long this_cellid = -1;
                if (person_cellid.ContainsKey(this_name))
                {
                    // a old artist
                    this_cellid = person_cellid[this_name];
                }
                else
                {
                    // a new artist
                    Person temp_person = new Person(Name: this_name);
                    temp_person.Act = new List<long>();
                    this_cellid = temp_person.CellID;
                    Global.LocalStorage.SavePerson(temp_person);
                }
                using (var this_person = Global.LocalStorage.UsePerson(this_cellid))
                {
                    this_person.Act.Add(this_movie);
                    person_cellid[this_name] = this_cellid;
                    persons.Add(this_cellid);
                }
            }
            return persons;
        }

        public List<long> GetDirectorsFromString(string str, long this_movie)
        {
            List<string> directors = new List<string>(str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToList();
            List<long> persons = new List<long>();

            foreach (string this_name in directors)
            {
                // for each parsed person name, we look whether this person exist already,
                // if exist, load and return CellID directly;
                // if not, new this person and save.
                long this_cellid = -1;
                if (person_cellid.ContainsKey(this_name))
                {
                    this_cellid = person_cellid[this_name];
                }
                else
                {
                    Person temp_person = new Person(Name: this_name);
                    temp_person.Direct = new List<long>();
                    this_cellid = temp_person.CellID;
                    Global.LocalStorage.SavePerson(temp_person);
                }
                using (var this_person = Global.LocalStorage.UsePerson(this_cellid))
                {
                    this_person.Direct.Add(this_movie);
                    person_cellid[this_name] = this_cellid;
                    persons.Add(this_cellid);
                }
            }
            return persons;
        }

        public void ImportMovie(string filename)
        {
            TrinityConfig.CurrentRunningMode = RunningMode.Embedded;

            StreamReader reader = new StreamReader(path + filename);

            string line;
            string[] fields;
            int movie_count = 0;
            while (null != (line = reader.ReadLine()))
            {
                fields = line.Split('\t');
                string movie_key = fields[0];
                Movie movie = new Movie(movie_key);

                for (int i = 0; i < fields.Count(); i++)
                {
                    List<string> temp_list;
                    List<long> temp_person;
                    string temp_field;
                    switch ((MovieFieldType)i)
                    {
                        case MovieFieldType.Genres:
                            temp_list = GetListFromString(fields[i]);
                            movie.SetField(((MovieFieldType)i).ToString(), temp_list);
                            break;

                        case MovieFieldType.Artists:
                            temp_person = GetArtistsFromString(fields[i], movie.CellID);
                            movie.SetField(((MovieFieldType)i).ToString(), temp_person);
                            break;

                        case MovieFieldType.Directors:
                            temp_person = GetDirectorsFromString(fields[i], movie.CellID);
                            movie.SetField(((MovieFieldType)i).ToString(), temp_person);
                            break;

                        case MovieFieldType.Performance:
                            temp_list = GetListFromString(fields[i]);
                            movie.SetField(((MovieFieldType)i).ToString(), temp_list);
                            break;

                        default:
                            temp_field = fields[i];
                            movie.SetField(((MovieFieldType)i).ToString(), temp_field);
                            break;
                    }
                }
                Global.LocalStorage.SaveMovie(movie);
                movie_count++;
                Console.WriteLine("#movie: " + movie_count);
            }
        }
    }
}
