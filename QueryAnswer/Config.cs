using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryAnswer
{
    class Config
    {
        public static readonly string data_path = @"D:\MovieDomain\QueryAnswer\resource\";
        public static readonly string movie_filename = @"movie_name.csv";
        public static readonly string artist_filename = @"artist_name.csv";
        public static readonly string director_filename = @"director_name.csv";
        public static readonly string country_filename = @"country_name.csv";
        public static readonly string genre_filename = @"genre_name.csv";

        public static readonly string pattern_filename = @"QA_pattern.txt";
        public static readonly string patternQ_filename = @"QA_pattern_question.txt";
        public static readonly string patternA_filename = @"QA_pattern_answer.txt";
    }
}
