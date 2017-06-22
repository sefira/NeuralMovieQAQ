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
        //Name AS Name,                       //Movie Name
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
            if (!string.IsNullOrEmpty(input_row["NumberOfReviewer"].String) && 
                Int32.Parse(input_row["NumberOfReviewer"].String) > 1000
                )
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

        sr = new StreamReader("artist_director_noise.csv");
        List<string> entities = new List<string>();
        while (true)
        {
            string line = sr.ReadLine();
            string entity = "";
            if (line != null && !string.IsNullOrEmpty(line))
            {
                entity = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                entities.Add(entity);
            }
            else
            {
                break;
            }
        }
        NoiseEntity = new HashSet<string>(entities);
    }

    private char[] split_chars;
    public char[] SplitChars
    {
        get { return split_chars; }
    }

    public HashSet<string> NoiseEntity { get; set; }

    public bool isNoiseEntity(string input_entity)
    {
        return NoiseEntity.Contains(input_entity);
    }

    public bool isAppropriateLength(string input_string, int low = 1, int high = 10)
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

    public bool IsChinese(string input_string)
    {
        Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        return cjkCharRegex.IsMatch(input_string[0].ToString());
    }

    public bool isAppropriateLanguage(string input_string, Markets market = Markets.zhCN)
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
            string input_string = input_row["Name"].String;
            string[] input_split = input_string.Split(name_filter.SplitChars, StringSplitOptions.RemoveEmptyEntries);
            List<string> input_splited = new List<string>(input_split);
            //input_splited.Add(input_string);
            foreach (string item in input_splited)
            {
                string name = item.Trim();
                if (name_filter.isAppropriateLanguage(name) && name_filter.isAppropriateLength(name, 2, 10) && !name_filter.isNoiseEntity(name))
                //if (name_filter.isAppropriateLanguage(name) && !name_filter.isNoiseEntity(name))
                {
                    output_row["Name"].Set(name);
                    output_row["NumberOfReviewer"].Set(input_row["NumberOfReviewer"].String);
                    yield return output_row;
                }
                else
                {
                    // for debug
                    output_row["Name"].Set("000MovieNameProcessor" + name);
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
        NameFilter name_filter = new NameFilter();
        foreach (Row input_row in input_rowset.Rows)
        {
            string input_string = input_row["Artists"].String;
            string[] input_split = input_string.Split(name_filter.SplitChars, StringSplitOptions.RemoveEmptyEntries);
            List<string> input_splited = new List<string>(input_split);
            //input_splited.Add(input_string);
            foreach (string item in input_splited)
            {
                string name = item.Trim();
                if (name_filter.isAppropriateLanguage(name) && name_filter.isAppropriateLength(name, 1, 6) && !name_filter.isNoiseEntity(name))
                //if (name_filter.isAppropriateLanguage(name) && !name_filter.isNoiseEntity(name))
                {
                    output_row["Artists"].Set(name);
                    yield return output_row;
                }
                else
                {
                    // for debug
                    output_row["Artists"].Set("000ArtistNameProcessor" + name);
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
            string input_string = input_row["Directors"].String;
            string[] input_split = input_string.Split(name_filter.SplitChars, StringSplitOptions.RemoveEmptyEntries);
            List<string> input_splited = new List<string>(input_split);
            //input_splited.Add(input_string);
            foreach (string item in input_splited)
            {
                string name = item.Trim();
                if (name_filter.isAppropriateLanguage(name) && name_filter.isAppropriateLength(name, 1, 6) && !name_filter.isNoiseEntity(name))
                //if (name_filter.isAppropriateLanguage(name) && !name_filter.isNoiseEntity(name))
                {
                    output_row["Directors"].Set(name);
                    yield return output_row;
                }
                else
                {
                    // for debug
                    output_row["Directors"].Set("000DirectorNameProcessor" + name);
                    //yield return output_row;
                    DebugStream.WriteLine(output_row.ToString());
                }
            }
        }
    }
}