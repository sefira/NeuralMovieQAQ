using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using GraphEngine;
using System.IO;

namespace GraphEngineDataImport
{
    class MovieEntityImport
    {
        enum MovieFieldType { Key, KGId, Genres, Artists, Directors, Characters, Performance, Distributors, Channels, Albums, Name, Alias, Description, Segments, Categories, IntEmbeddedFilters, NumberOfWantToWatch, Rating, NumberOfShortReview, ReviewCount, NumberOfWatched, NumberOfReviewer, PublishDate, Length, Country, Language, SourceUrls, ImageUrls, OfficialSite, EntityContainer, Logo, QueryRank };

        enum PersonFieldType { Name, Gender, Married, Spouse, Act, Direct };

        Dictionary<string, long> person_cellid = new Dictionary<string, long>();

        public List<string> GetListFromString(string str)
        {
            return new List<string>(str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public List<long> GetArtistsFromString(string str, long this_movie)
        {
            List<string> artists =  new List<string>(str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            List<long> persons = new List<long>();

            foreach (string this_name in artists)
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
                    Person this_person = new Person(Name: this_name);
                    this_person.SetField(PersonFieldType.Act.ToString(), this_movie);
                    this_cellid = this_person.CellID;
                    Global.LocalStorage.SavePerson(this_person);
                }
                persons.Add(this_cellid);
            }
            return persons;
        }

        public List<long> GetDirectorsFromString(string str, long this_movie)
        {
            List<string> directors = new List<string>(str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
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
                    Person this_person = new Person(Name: this_name);
                    this_person.SetField(PersonFieldType.Direct.ToString(), this_movie);
                    this_cellid = this_person.CellID;
                    Global.LocalStorage.SavePerson(this_person);
                }
                persons.Add(this_cellid);
            }
            return persons;
        }

        public void ImportMovie(string filename)
        {
            TrinityConfig.CurrentRunningMode = RunningMode.Embedded;

            StreamReader reader = new StreamReader(filename);

            string line;
            string[] fields;
            while (null != (line = reader.ReadLine()))
            {
                try
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
                }
                catch
                {
                    Console.Error.WriteLine("Failed to import the line:");
                    Console.Error.WriteLine(line);
                }
            }

            foreach (var movie in Global.LocalStorage.Movie_Accessor_Selector())
            {
                Console.WriteLine(movie.Name);
                Console.WriteLine(movie.CellID);
            }
            Console.WriteLine();
            Global.LocalStorage.SaveStorage();
        }
    }
}
