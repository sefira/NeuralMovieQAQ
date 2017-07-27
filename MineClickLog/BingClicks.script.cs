using Microsoft.SCOPE.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScopeRuntime;

public class ZhidaoFilterProcessor : Processor
{
    public static bool IsBaiduZhidao(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }
        return (url.StartsWith("https://zhidao.baidu.com/") || url.StartsWith("http://zhidao.baidu.com/"));
    }

    public override Schema Produces(string[] requestedColumns, string[] args, Schema input)
    {
        var output_schema = input.Clone();
        return output_schema;
    }

    public override IEnumerable<Row> Process(RowSet input, Row outputRow, string[] args)
    {
        foreach (Row input_row in input.Rows)
        {
            try
            {
                if (!IsBaiduZhidao(input_row["ClickedUrl"].String))
                {
                    continue;
                }
            }
            catch (Exception e)
            {
                ScopeRuntime.Diagnostics.DebugStream.WriteLine(e.Message);
            }
            input_row.CopyTo(outputRow);
            yield return outputRow;
        }
    }
}