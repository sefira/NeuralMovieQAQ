using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDialog
{
    class AnswerGenerator
    {
        private static string[,] relation_matrix = new string[6, 6]
        {
            ///////////// all   artist                            director                       country                         genre                          publishdate
            /* all */     {"X", "想看哪个演员的电影呢？",          "想看哪个导演的电影呢？",         "想看哪个国家的电影呢？",       "想看哪个类型的电影呢？",       "想看经典的还是最近的电影呢？"}, 
            
            /* artist */  {"X", "{0}和{1}有很多合作，想看谁的呢？",   "{0}和{1}有很多合作，想看谁的呢？",  "X",                    "{0}演了很多{1}，想看哪种呢？",       "他演了很多电影呢，想看经典的还是最近的呢？"}, 

            /* director */{"X", "{0}和{1}有很多合作，想看谁的呢？",   "X",                           "X",                        "{0}拍了很多{1}，想看哪种呢？",       "他拍了很多电影呢，想看经典的还是最近的呢？"}, 

            /* country */ {"X", "{0}有一些著名艺人：{1}，想看谁的呢？","{0}有一些著名导演：{1}，想看谁的呢？","X",                  "这个地区的{1}比较有名，想看哪种呢？",   "这个地区的电影有很多啦，想看经典的还是最近的呢？"}, 
              
            /* genre */   {"X", "{1}拍了很多{0}电影，想看谁的呢？",  "{1}拍了很多{0}电影，想看谁的呢？",  "{1}拍了很多{0}电影，想看哪里的呢？",  "X",                           "这种类型的电影有很多啦，想看经典的还是最近的呢？"}, 

            /* publish */ {"X", "想看哪个演员的电影呢？{1} or 其他？","想看哪个导演的电影呢？{1} or 其他？","想看哪个国家的电影呢？{1} or 其他？","想看哪个类型的电影呢？{1} or 其他？", "X"}
        };

        public static string AnswerIt(List<string> answer_entity, Session session, ParseStatus to)
        {
            int from_status = DialogManager.parsestatus2int[session.parse_status];
            int to_status = DialogManager.parsestatus2int[to];
            string entity_in_question = "";
            switch (DialogManager.int2parsestatus[from_status])
            {
                case ParseStatus.All:
                    entity_in_question = string.Join("、 ", session.carried_artist.ToArray());
                    break;
                case ParseStatus.Artist:
                    entity_in_question = string.Join("、 ", session.carried_artist.ToArray());
                    break;
                case ParseStatus.Director:
                    entity_in_question = string.Join("、 ", session.carried_director.ToArray());
                    break;
                case ParseStatus.Country:
                    entity_in_question = string.Join("、 ", session.carried_country.ToArray());
                    break;
                case ParseStatus.Genre:
                    entity_in_question = string.Join("、 ", session.carried_genre.ToArray());
                    break;
                case ParseStatus.PublishDate:
                    int carried_start_data = session.carried_publishdate.from;
                    int data_interface = int.Parse(DateTime.Now.AddMonths(-18).ToString("yyyyMMdd"));
                    entity_in_question = carried_start_data > data_interface ? "最近" : "过去";
                    break;
                default:
                    Utils.WriteError("error turn status!");
                    break;
            }
            string answer = string.Format(relation_matrix[from_status, to_status], entity_in_question, string.Join("、 ", answer_entity.ToArray()));
            return answer;
        }
    }
}
