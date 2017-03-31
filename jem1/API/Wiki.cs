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
    internal class Wiki
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

            var webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";
            webrequest.ContentType = "application/x-www-form-urlencoded";
            var webresponse = (HttpWebResponse)webrequest.GetResponse();
            var enc = Encoding.GetEncoding("utf-8");

            var txt = string.Empty;
            using (var reader = new StreamReader(webresponse.GetResponseStream(), enc)) 
            {
                var ser = new JsonSerializer();                            
                var result = ser.Deserialize<Result>(new JsonTextReader(reader));

                foreach (Page page in result.query.pages.Values)
                {
                    txt += page.extract; 
                }
            }
            return txt;
        }
    }
}
