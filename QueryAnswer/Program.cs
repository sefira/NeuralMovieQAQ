using JiebaNet.Segmenter;
using JiebaNet.Analyser;
using JiebaNet.Segmenter.PosSeg;
using System;
using System.Linq;
using System.Collections.Generic;

namespace QueryAnswer
{
    class Program
    {
        static void Main(string[] args)
        {
            //string query = @"上世纪香港的刘德华出演了张艺谋和冯小刚2001年的天下无贼一部喜剧片";
            //Query q = new Query(query);
            //Parser m_Parser = new Parser();
            //m_Parser.ParseAll(ref q);
            //var a = q;
            //oSearchClient.TestQuery();
            DialogManager movie_dialog = new DialogManager();
            movie_dialog.DialogFlow();
        }
    }
}