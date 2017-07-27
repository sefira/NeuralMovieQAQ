using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScopeRuntime;
using MS.Internal.Bing.DataMining.SearchLogApi;

public class ClickProcessor : Processor
{
    public static bool IsBaiduRedirect(string url)
    {
        return (url.StartsWith("http://www.baidu.com/link?url=") || url.StartsWith("http://www.baidu.com/adrc.php?t=") || url.StartsWith("http://www.baidu.com/s?") || url.StartsWith("http://cache.baiducontent.com/c?")
            || url.StartsWith("http://wenku.baidu.com/link?url=") || url.StartsWith("http://zhidao.baidu.com/link?url=") || url.StartsWith("http://baike.baidu.com/link?url=") || url.StartsWith("http://www.baidu.com/search?"));
    }

    public override Schema Produces(string[] requestedColumns, string[] args, Schema input)
    {
        return new Schema(requestedColumns);
    }

    public override IEnumerable<Row> Process(RowSet input, Row outputRow, string[] args)
    {
        foreach (Row row in input.Rows)
        {
            outputRow["LogDate"].Set(row["LogDate"].String);
            outputRow["Query_Engine"].Set(row["Query_Engine"].String);
            outputRow["Query"].Set(row["Query"].String);
            outputRow["Session_SessionId"].Set(row["Session_SessionId"].String);

            CompetitiveClickList ccl = row["Page_AllClicks"].Value as CompetitiveClickList;
            outputRow["Session_ClickCount"].Set(ccl.Count);

            foreach (CompetitiveClick c in ccl)
            {
                if (IsBaiduRedirect(c.TargetPageView.Url))
                {
                    continue;
                }
                outputRow["ClickUrl"].Set(c.TargetPageView.Url);
                outputRow["Title"].Set(c.TargetPageView.Title);
                outputRow["DwellTime"].Set(c.TargetPageView.DwellTime);
                outputRow["ClickType"].Set(c.TargetPageView.ClickType);
                outputRow["SequenceNumber"].Set(c.TargetPageView.SequenceNumber);

                yield return outputRow;
            }
        }
    }
}

public class SampleReducer : Reducer
{
    public override Schema Produces(string[] columns, string[] args, Schema input)
    {
        return new Schema("Query:string,Url:string,Click:long,Traffic:long,ClickPer:double,ClickDecay:double,ClickDis:int");
    }

    public override IEnumerable<Row> Reduce(RowSet input, Row output, string[] args)
    {
        int count = 0;
        string query = "";
        string url = "";
        long clickcount = 0;
        long querytraffic = 0;
        double ClickPer = 1.0;
        double ClickDecay = 0.0;

        foreach (Row row in input.Rows)
        {
            count++;
            if (count == 1)
            {
                query = row["Query"].String;
                url = row["ClickUrl"].String;
                clickcount = row["ClickCount"].Long;
                querytraffic = row["ImpressionCount"].Long;
                ClickPer = (clickcount * 1.0 / querytraffic);
            }
            else if (count == 2)
            {
                long curclickcount = row["ClickCount"].Long;
                ClickDecay = (curclickcount * 1.0 / clickcount);
            }
        }

        output[0].Set(query);
        output[1].Set(url);
        output[2].Set(clickcount);
        output[3].Set(querytraffic);
        output[4].Set(ClickPer);
        output[5].Set(ClickDecay);
        output[6].Set(count);

        yield return output;
    }
}