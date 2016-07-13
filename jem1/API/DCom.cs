using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrapySharp.Network;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using static jem1.U;

namespace jem1.API
{
    class DCom
    {
        public static string Scrape(string word)
        {
            List<string> posL = new List<string>();

            ScrapingBrowser browser = new ScrapingBrowser();

            WebPage pageResult = browser.NavigateToPage(new Uri("http://dictionary.com/browse/" + word + "?s=t"));
            var def = pageResult.Html.CssSelect("div.source-box").First();
            var htmlNodes = def.CssSelect("header.luna-data-header");
            //var htmlNodes = pageResult.Html.CssSelect("header.luna-data-header");
            foreach (HtmlNode node in htmlNodes)
            {
                if (node.InnerText.Trim().ToLower() == "verb phrases" || node.InnerText.Trim().ToLower().Contains("idioms")) { return ListToString(posL); }
                if (node.InnerText.Trim().ToLower().StartsWith("verb ("))
                {
                    if (!posL.Contains("verb"))
                    {
                        posL.Add("verb");
                    }
                }
                else if(node.InnerText.Trim().Contains("noun") && node.InnerText.Trim().Contains(","))
                {
                    if (!posL.Contains("noun"))
                    {
                        posL.Add("noun");
                    }
                }
                else if(node.InnerText.Trim().ToLower().Contains("adjective"))
                {
                    if (!posL.Contains("adjective"))
                    {
                        posL.Add("adjective");
                    }
                }
                else
                {
                    if (!posL.Contains(node.InnerText.Trim().ToLower()))
                    {
                        posL.Add(node.InnerText.Trim().ToLower());
                    }
                }
            }
            return ListToString(posL);
        }
    }
}
