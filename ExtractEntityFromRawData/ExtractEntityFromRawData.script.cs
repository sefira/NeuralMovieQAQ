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
            if(!string.IsNullOrEmpty(input_row["NumberOfReviewer"].String) && Int32.Parse(input_row["NumberOfReviewer"].String) > 5000)
            {
                input_row.CopyTo(output_row);
                yield return output_row;
            }
        }
    }
}

public enum Markets
{
    zhCN,
    enUS
}

public class NameFilter
{
    public NameFilter()
    {
        StreamReader sr = new StreamReader("SplitChar.txt");
        string lines = "";
        while (true)
        {
            string line = sr.ReadLine();
            if (line != null)
            {
                lines += line;
            }
            else
            {
                break;
            }
        }
        lines += " ";
        split_chars = lines.ToCharArray();
    }

    private char[] split_chars;
    public char[] SplitChars
    {
        get { return split_chars; }
    }

    public static Boolean isAppropriateLength(string input_string, int low = 1, int high = 10)
    {
        if (!string.IsNullOrEmpty(input_string) && input_string.Length > low && input_string.Length < high)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsChinese(string input_string)
    {
        Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        return cjkCharRegex.IsMatch(input_string[0].ToString());
    }

    public static Boolean isAppropriateLanguage(string input_string, Markets market = Markets.zhCN)
    {
        if (string.IsNullOrEmpty(input_string))
        {
            return false;
        }
        if (market == Markets.zhCN)
        {
            return IsChinese(input_string);
        }
        else
        {
            return !IsChinese(input_string);
        }
    }


}

public class MovieNameProcessor : Processor
{
    public override Schema Produces(string[] requested_columns, string[] args, Schema input_schema)
    {
        var output_schema = input_schema.Clone();
        return output_schema;
    }

    public override IEnumerable<Row> Process(RowSet input_rowset, Row output_row, string[] args)
    {
        NameFilter name_filter = new NameFilter();

        foreach (Row input_row in input_rowset.Rows)
        {
            string input_string = input_row["MovieName"].String;
            string[] input_split = input_string.Split(name_filter.SplitChars);
            foreach (string item in input_split)
            {
                string name = item.Trim();
                if (NameFilter.isAppropriateLanguage(name) && NameFilter.isAppropriateLength(name, 1, 10))
                {
                    output_row["MovieName"].Set(name);
                    yield return output_row;
                }
                else
                {
                    // for debug
                    output_row["MovieName"].Set("000MovieNameProcessor" + name);
                    //yield return output_row;
                    DebugStream.WriteLine(output_row.ToString());
                }
            }
        }
    }
}

public class ArtistNameProcessor : Processor
{
    public override Schema Produces(string[] requested_columns, string[] args, Schema input_schema)
    {
        var output_schema = input_schema.Clone();
        return output_schema;
    }

    public override IEnumerable<Row> Process(RowSet input_rowset, Row output_row, string[] args)
    {
        NameFilter name_filter =  new NameFilter();
        foreach (Row input_row in input_rowset.Rows)
        {
            string input_string = input_row["ArtistName"].String;
            string[] input_split = input_string.Split(name_filter.SplitChars);
            foreach (string item in input_split)
            {
                string name = item.Trim();
                if (NameFilter.isAppropriateLanguage(name) && NameFilter.isAppropriateLength(name, 1, 6))
                {
                    output_row["ArtistName"].Set(name);
                    yield return output_row;
                }
                else
                {
                    // for debug
                    output_row["ArtistName"].Set("000ArtistNameProcessor" + name);
                    //yield return output_row;
                    DebugStream.WriteLine(output_row.ToString());
                }
            }
        }
    }
}


public class DirectorNameProcessor : Processor
{
    public override Schema Produces(string[] requested_columns, string[] args, Schema input_schema)
    {
        var output_schema = input_schema.Clone();
        return output_schema;
    }

    public override IEnumerable<Row> Process(RowSet input_rowset, Row output_row, string[] args)
    {
        NameFilter name_filter = new NameFilter();
        foreach (Row input_row in input_rowset.Rows)
        {
            string input_string = input_row["DirectorName"].String;
            string[] input_split = input_string.Split(name_filter.SplitChars);
            foreach (string item in input_split)
            {
                string name = item.Trim();
                if (NameFilter.isAppropriateLanguage(name) && NameFilter.isAppropriateLength(name, 1, 6))
                {
                    output_row["DirectorName"].Set(name);
                    yield return output_row;
                }
                else
                {
                    // for debug
                    output_row["DirectorName"].Set("000DirectorNameProcessor" + name);
                    //yield return output_row;
                    DebugStream.WriteLine(output_row.ToString());
                }
            }
        }
    }
}

public class GenereNameProcessor : Processor
{
    public override Schema Produces(string[] requested_columns, string[] args, Schema input_schema)
    {
        var output_schema = input_schema.Clone();
        return output_schema;
    }

    public override IEnumerable<Row> Process(RowSet input_rowset, Row output_row, string[] args)
    {
        NameFilter name_filter = new NameFilter();
        foreach (Row input_row in input_rowset.Rows)
        {
            string input_string = input_row["GenreName"].String;
            string[] input_split = input_string.Split(name_filter.SplitChars);
            foreach (string item in input_split)
            {
                string name = item.Trim();
                if (NameFilter.isAppropriateLanguage(name) && NameFilter.isAppropriateLength(name, 1, 6))
                {
                    output_row["GenreName"].Set(name);
                    yield return output_row;
                }
                else
                {
                    // for debug
                    output_row["GenreName"].Set("000GenreNameProcessor" + name);
                    //yield return output_row;
                    DebugStream.WriteLine(output_row.ToString());
                }
            }
        }
    }
}

public class CountryNameProcessor : Processor
{
    public override Schema Produces(string[] requested_columns, string[] args, Schema input_schema)
    {
        var output_schema = input_schema.Clone();
        return output_schema;
    }

    public override IEnumerable<Row> Process(RowSet input_rowset, Row output_row, string[] args)
    {
        NameFilter name_filter = new NameFilter();
        foreach (Row input_row in input_rowset.Rows)
        {
            string input_string = input_row["CountryName"].String;
            string[] input_split = input_string.Split(name_filter.SplitChars);
            foreach (string item in input_split)
            {
                string name = item.Trim();
                if (NameFilter.isAppropriateLanguage(name) && NameFilter.isAppropriateLength(name, 1, 6))
                {
                    output_row["CountryName"].Set(name);
                    yield return output_row;
                }
                else
                {
                    // for debug
                    output_row["CountryName"].Set("000CountryNameProcessor" + name);
                    //yield return output_row;
                    DebugStream.WriteLine(output_row.ToString());
                }
            }
        }
    }
}