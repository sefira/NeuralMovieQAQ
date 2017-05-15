using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using GraphEngine;

namespace GraphEngineDataImport
{
    internal class GraphEngineDataImport
    {
        static void Main(string[] args)
        {
            MovieEntityImport movie_entity_import = new MovieEntityImport(@"D:\MovieDomain\GraphEngine\input\");
            string filename = @"Movie.csv";
            movie_entity_import.ImportMovie(filename);
        }
    }
}