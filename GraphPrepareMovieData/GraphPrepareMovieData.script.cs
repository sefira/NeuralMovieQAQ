using Microsoft.SCOPE.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScopeRuntime;
using System.Text.RegularExpressions;
using ScopeRuntime.Diagnostics;

public class ExceptJunkMovieProcessor : Processor
{
    public override Schema Produces(string[] requested_columns, string[] args, Schema input_schema)
    {
        //Name AS MovieName,                  //Movie Name
        //Generes AS Genres,                  //Moive Type
        //Artists AS Artists,                 //Actors
        //Performance AS Performance,         //Actors with order
        //Directors AS Directors,             //Directors
        //Rating AS Rating,                   //The Rating
        //RatingCount AS NumberOfShortReview, //Number of Short Review
        //VisitCount AS NumberOfWatched,      //Number of People Who Watched
        //Popularity AS NumberOfWantToWatch,  //Number of People Who Want to watch
        //Rank AS NumberOfReviewer,           //Number of Reviewer for the Rating
        //PublishDate AS PublishDate,         //Publish Date
        //Length AS Length,                   //How Long is the Movie
        //Geographies AS Country,             //Country
        //Filter AS Language                  //Language
        var output_schema = input_schema.Clone();
        return output_schema;
    }

    public override IEnumerable<Row> Process(RowSet input_rowset, Row output_row, string[] args)
    {
        foreach (Row input_row in input_rowset.Rows)
        {
            if (!string.IsNullOrEmpty(input_row["NumberOfReviewer"].String) && Int32.Parse(input_row["NumberOfReviewer"].String) > 8000)
            {
                input_row.CopyTo(output_row);
                yield return output_row;
            }
        }
    }
}
