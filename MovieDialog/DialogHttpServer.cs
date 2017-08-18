using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;
using RestSharp.Extensions.MonoHttp;

namespace MovieDialog
{
    public class DialogHttpServer
    {
        private HttpListener listener;
        private Thread dialog_thread;
        public static List<string> dialog_history = new List<string>();
        public static Semaphore bot_response_sem;
        public static string user_query;
        public static Semaphore user_request_sem;

        public DialogHttpServer()
        {
            listener = new HttpListener();
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            listener.Prefixes.Add("http://+:9009/");

            listener.Start();
            Console.WriteLine("Listening...");
        }

        ~DialogHttpServer()
        {
            listener.Stop();
        }

        public void WorkWithMovieDialog()
        {
            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request. 
                HttpListenerContext http_context = listener.GetContext();
                HttpListenerRequest http_request = http_context.Request;
                HttpListenerResponse http_response = http_context.Response;

                if (http_request.HttpMethod == "POST")
                {
                    user_query = new StreamReader(http_request.InputStream).ReadToEnd();
                    if (string.IsNullOrWhiteSpace(user_query))
                    {
                        continue;
                    }
                    Console.WriteLine("POST:" + http_request.Url + $"body={user_query}");
                }
                else
                {
                    if (http_request.HttpMethod == "GET")
                    {
                        var QueryString = HttpUtility.ParseQueryString(http_request.Url.Query);
                        user_query = QueryString["query"];
                        if(string.IsNullOrWhiteSpace(user_query))
                        {
                            continue;
                        }
                        Console.WriteLine("GET:" + http_request.Url + $"?query={user_query}");
                    }
                }
                string response = "";
                switch (user_query)
                {
                    case "start":
                        response = StartDialogThread();
                        break;

                    case "end":
                        response = EndDialogThread();
                        break;

                    default:
                        response = SendQueryToDialogThread();
                        break;
                }

                byte[] res = Encoding.UTF8.GetBytes(response);
                http_response.OutputStream.Write(res, 0, res.Length);
                http_response.OutputStream.Close();
            }
        }

        private string StartDialogThread()
        {
            dialog_history = new List<string>();
            bot_response_sem = new Semaphore(0, 1);
            user_query = "";
            user_request_sem = new Semaphore(0, 1);
            
            dialog_thread = new Thread(new ThreadStart(DialogThread));
            dialog_thread.Start();

            bot_response_sem.WaitOne();
            return string.Join("\n", dialog_history);
        }

        private void DialogThread()
        {
            try
            {
                DialogManager movie_dialog = new DialogManager();
                movie_dialog.DialogFlow(null);
            }
            catch (Exception e)
            {
                Utils.WriteError(e.ToString());
            }
            DialogHttpServer.bot_response_sem.Release();
            return;
        }

        private string EndDialogThread()
        {
            if (dialog_thread != null && (dialog_thread.ThreadState & ThreadState.WaitSleepJoin) == ThreadState.WaitSleepJoin)
            {
                dialog_thread.Abort();
            }

            return "Dialog Thread Aborted";
        }

        private string SendQueryToDialogThread()
        {
            if (dialog_thread != null && (dialog_thread.ThreadState & ThreadState.WaitSleepJoin) == ThreadState.WaitSleepJoin)
            {
                string response;
                try
                {
                    user_request_sem.Release();
                    bot_response_sem.WaitOne();
                    response = string.Join("\n", dialog_history);
                }
                catch (Exception e)
                {
                    response = "没有对话在进行， 你的输入太快了";
                }
                return response;
            }
            else
            {
                return "Dialog Thread Has Not Been Started Yet";
            }
        }
    }
}
