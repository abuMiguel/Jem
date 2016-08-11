using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace jem1.API
{
    class Wiki
    {
        public class Result
        {
            public Query query { get; set; }
        }

        public class Query
        {
            public Dictionary<string, Page> pages { get; set; }
        }

        public class Page
        {
            public string extract { get; set; }
        }

        public static string Lookup(string word)
        {
            string url = "http://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&explaintext=1&titles=" + word.Replace(' ', '_') + "&redirects";

            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";
            webrequest.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
            Encoding enc = Encoding.GetEncoding("utf-8");

            var txt = string.Empty;
            using (StreamReader reader = new StreamReader(webresponse.GetResponseStream(), enc))  // create a StreamReader
            {
                JsonSerializer ser = new JsonSerializer();                            // create JsonSerializer object.
                Result result = ser.Deserialize<Result>(new JsonTextReader(reader));  // Deserialize the data and store it in Result class’s object

                foreach (Page page in result.query.pages.Values)
                {
                    txt += page.extract;   // Append each value from page to txtArticle.
                }
            }
            return txt;
        }
    }
}
