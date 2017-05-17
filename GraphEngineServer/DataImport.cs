using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEngineServer
{
    class DataImport
    {
        public string path;
        public const string person_cellid_dict_filename = @"data\person_cellid.dict";
        public const string movie_cellid_dict_filename = @"data\movie_cellid.dict";

        public enum EntityType { Movie, Person };

        /// <summary>
        /// the fields order in the below enum must be same with schema.tsl
        /// </summary>
        public enum MovieFieldType { Key, KGId, Genres, Artists, Directors, Characters, Performance, Distributors, Channels, Albums, Name, Alias, Description, Segments, Categories, IntEmbeddedFilters, NumberOfWantToWatch, Rating, NumberOfShortReview, ReviewCount, NumberOfWatched, NumberOfReviewer, PublishDate, Length, Country, Language, SourceUrls, ImageUrls, OfficialSite, EntityContainer, Logo, QueryRank, TheType };

        public enum PersonFieldType { Age, Parent, Name, Gender, Married, Spouse, Act, Direct, TheType };


        public Dictionary<string, long> person_cellid = new Dictionary<string, long>();
        public Dictionary<string, long> movie_cellid = new Dictionary<string, long>();


        #region constructor & destructor function
        public void ReadDictionary()
        {
            string line = "";
            try
            {
                using (StreamReader sr = new StreamReader(path + person_cellid_dict_filename))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] line_arr = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        person_cellid[line_arr[0]] = long.Parse(line_arr[1]);
                    }
                }
                using (StreamReader sr = new StreamReader(path + movie_cellid_dict_filename))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] line_arr = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        movie_cellid[line_arr[0]] = long.Parse(line_arr[1]);
                    }
                }
            }
            catch { Console.WriteLine("Dicionaries don't exist!"); }
        }

        public void WriteDictionary()
        {
            using (StreamWriter sw = new StreamWriter(path + person_cellid_dict_filename))
            {
                foreach (var item in person_cellid)
                {
                    sw.WriteLine(item.Key + "\t" + item.Value);
                }
            }
            using (StreamWriter sw = new StreamWriter(path + movie_cellid_dict_filename))
            {
                foreach (var item in movie_cellid)
                {
                    sw.WriteLine(item.Key + "\t" + item.Value);
                }
            }
        }
        public DataImport()
        {
            ReadDictionary();
        }

        public DataImport(string str)
        {
            path = str;
            ReadDictionary();
        }
        #endregion 
    }
}
