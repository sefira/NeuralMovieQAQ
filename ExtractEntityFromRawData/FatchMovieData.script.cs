using Microsoft.SCOPE.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScopeRuntime;

public class MovieReducer : Reducer
{
    public override Schema Produces(string[] columns, string[] args, Schema input_schema)
    {
        var output_schema = input_schema.Clone();
        return output_schema;
    }

    public override IEnumerable<Row> Reduce(RowSet input, Row output, string[] args)
    {
        foreach (Row input_row in input.Rows)
        {
            input_row.CopyTo(output);
            break;
        }
        yield return output;
    }
}

