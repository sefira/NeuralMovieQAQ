using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Trinity;

namespace GraphEngineServer
{
    class MovieEntityImport : DataImport
    {
        public MovieEntityImport(string str) : base(str)
        {
            path = str;
            ReadDictionary();
        }

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
                    Person temp_person = new Person(TheType: EntityType.Person.ToString(), Name: this_name, Act: new List<long>());
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
                    // a old director
                    this_cellid = person_cellid[this_name];
                }
                else
                {
                    // a new director
                    Person temp_person = new Person(TheType: EntityType.Person.ToString(), Name: this_name, Direct: new List<long>());
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

        public int GetNumberFromString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            string resultString = Regex.Match(str, @"\d+").Value;
            if (string.IsNullOrEmpty(resultString))
            {
                return 0;
            }
            return int.Parse(resultString);
        }

        public int GetDateFromString(string str)
        {
            if (str.Count() == 4)
            {
                str += "-01-01";
            }
            if (str.Count() > 10)
            {
                str = str.Substring(0, 10);
            }
            string date = DateTime.Parse(str).ToString("yyyyMMdd");
            return int.Parse(date);
        }

        public void ImportMovie(string filename)
        {
            if (movie_cellid.Count != 0)
            {
                Console.WriteLine("=============================Data had been imported once. Skipping this turn.");
                return;
            }

            using (StreamReader reader = new StreamReader(path + filename))
            {
                string line;
                string[] fields;
                int movie_count = 0;
                while (null != (line = reader.ReadLine()))
                {
                    fields = line.Split('\t');
                    Movie movie = new Movie(TheType: EntityType.Movie.ToString());

                    for (int i = 0; i < fields.Count(); i++)
                    {
                        List<string> temp_list;
                        List<long> temp_person;
                        string temp_field;
                        int temp_int;
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

                            case MovieFieldType.NumberOfReviewer:
                                temp_int = GetNumberFromString(fields[i]);
                                movie.SetField(((MovieFieldType)i).ToString(), temp_int);
                                break;

                            case MovieFieldType.NumberOfShortReview:
                                temp_int = GetNumberFromString(fields[i]);
                                movie.SetField(((MovieFieldType)i).ToString(), temp_int);
                                break;

                            case MovieFieldType.NumberOfWantToWatch:
                                temp_int = GetNumberFromString(fields[i]);
                                movie.SetField(((MovieFieldType)i).ToString(), temp_int);
                                break;

                            case MovieFieldType.NumberOfWatched:
                                temp_int = GetNumberFromString(fields[i]);
                                movie.SetField(((MovieFieldType)i).ToString(), temp_int);
                                break;

                            case MovieFieldType.Length:
                                temp_int = GetNumberFromString(fields[i]);
                                movie.SetField(((MovieFieldType)i).ToString(), temp_int);
                                break;

                            case MovieFieldType.Rating:
                                temp_int = GetNumberFromString(fields[i]);
                                movie.SetField(((MovieFieldType)i).ToString(), temp_int);
                                break;

                            case MovieFieldType.PublishDate:
                                temp_int = GetDateFromString(fields[i]);
                                movie.SetField(((MovieFieldType)i).ToString(), temp_int);
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
                    movie_cellid[movie.Name] = movie.CellID;
                }
            }
            WriteDictionary();
            //Global.LocalStorage.SaveStorage();
        }
    }
}
