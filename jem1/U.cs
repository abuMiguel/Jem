using jem1.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace jem1
{
    static class U
    {
        //gets part of speech from xml from Merriam Webster dictionary API and normalizes it for Jem
        public static string GetCSVFromXML(string xmlString, string element)
        {
            List<string> posL = new List<string>();
            string pos = string.Empty;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                var doc = XDocument.Load(reader);
                var xmlpos = doc.Root.Elements().Select(x => x.Element(element));
                char[] trim = new char[2] { ' ', ',' };

                foreach (string p in xmlpos)
                {
                    if (!string.IsNullOrEmpty(p))
                    {
                        if (!posL.Contains(p.Trim(trim)))
                        {
                            posL.Add(p.Trim(trim));
                        }
                    }
                }
            }
            //cleanup MW XML
            if (posL.Contains("adjective") && posL.Contains("noun"))
            {
                posL.Remove("adjective");
            }
            if (posL.Contains("geographical name"))
            {
                posL.Remove("geographical name"); posL.Add("proper noun");
            }
            if(!posL.Contains("proper noun"))
            {

            }

            pos = ListToString(posL);
            return pos;
        }

        public static List<string> CSVToList(string csv)
        {
            List<string> x = new List<string>();
            if (csv.Contains(","))
            {
                x = csv.Split(',').ToList();
                return x;
            }
            else
            {
                x.Add(csv);
                return x;
            }
        }

        public static string ListToString(List<string> list)
        {
            if (list.Count > 1)
            {
                //Return string
                string rst = "";
                foreach (string st in list)
                {
                    rst += st + ",";
                }
                rst = rst.TrimEnd(',');
                return rst;
            }
            else if (list.Count == 1) { return list[0]; }
            else { return ""; }
        }

        public static bool EndsWith(Word w, string ending)
        {
            int num = ending.Length;
            string wordReverseEnding = " ";

            if (w.name.Length > num)
            {
                wordReverseEnding = new string(w.name.Reverse().Take(num).ToArray());
            }

            return wordReverseEnding == new string(ending.Reverse().ToArray()) ? true : false;
        }

        public static bool HasPOS(Sentence s, List<string> posL)
        {
            foreach (Word w in s.words)
            {
                if (posL.Contains(w.pos)) { return true; }
            }
            return false;
        }

        //Determine whether the words preceding a word up until a particular part of speech, are all
        //words listed in an OK list. This allows a rule to apply where there are an infinite number 
        //of words inbetween that don't disturb the rule
        public static bool NothingButBetween(Word startWord, string endPOS, string[] okList, Sentence s)
        {
            for (int i = startWord.ID - 1; i >= 0; i--)
            {
                var posL = s.words[i].pos.Contains(",") ? s.words[i].pos.Split(',') : new string[1] { s.words[i].pos };

                if(posL.Contains(endPOS) && posL.Count() > 1)
                {
                    return false;
                }

                for (int j = 0; j < okList.Length; j++)
                {
                    var flag = false;
                    for (int k = 0; k < posL.Length; k++)
                    {

                        if (posL[k] == endPOS)
                        {
                            return true;
                        }

                        if (posL[k] == okList[j])
                        {
                            flag = true;
                            break;
                        }
                        else if (j == okList.Length - 1)
                        {
                            return false;
                        }
                    }
                    if (flag) { break; }
                }
            }
            return false;
        }

        //override for clauses
        public static bool NothingButBetween(Word startWord, string endPOS, string[] okList, Clause c)
        {
            int cID = startWord.ID - c.words[0].ID;

            for (int i = cID - 1; i >= 0; i--)
            {
                var posL = c.words[i].pos.Contains(",") ? c.words[i].pos.Split(',') : new string[1] { c.words[i].pos };

                if (posL.Contains(endPOS) && posL.Count() > 1)
                {
                    return false;
                }

                for (int j = 0; j < okList.Length; j++)
                {
                    var flag = false;
                    for (int k = 0; k < posL.Length; k++)
                    {

                        if (posL[k] == endPOS)
                        {
                            return true;
                        }

                        if (posL[k] == okList[j])
                        {
                            flag = true;
                            break;
                        }
                        else if (j == okList.Length - 1)
                        {
                            return false;
                        }
                    }
                    if (flag) { break; }
                }
            }

            return false;
        }


        //Same as NothingButBetween except it looks forward not backwards
        public static bool NothingButBetweenForward(Word startWord, string endPOS, string[] okList, Sentence s)
        {
            for (int i = startWord.ID + 2; i < s.wordCount; i++)
            {
                var posL = s.words[i].pos.Contains(",") ? s.words[i].pos.Split(',') : new string[1] { s.words[i].pos };

                if (posL.Contains(endPOS) && posL.Count() > 1)
                {
                    return false;
                }

                for (int j = 0; j < okList.Length; j++)
                {
                    var flag = false;
                    for (int k = 0; k < posL.Length; k++)
                    {

                        if (posL[k] == endPOS)
                        {
                            return true;
                        }

                        if (posL[k] == okList[j])
                        {
                            flag = true;
                            break;
                        }
                        else if (j == okList.Length - 1)
                        {
                            return false;
                        }
                    }
                    if (flag) { break; }
                }
            }
            return false;
        }

        //Same as NothingButBetweenForward except it uses contains instead of equals for endPOS
        //Used for multiple possible endPOS, comma separated like: noun,pronoun
        public static bool NothingButBetweenForwardContains(Word startWord, string endPOS, string[] okList, Sentence s)
        {
            for (int i = startWord.ID + 2; i < s.wordCount; i++)
            {
                var posL = s.words[i].pos.Contains(",") ? s.words[i].pos.Split(',') : new string[1] { s.words[i].pos };

                for (int j = 0; j < okList.Length; j++)
                {
                    var flag = false;
                    for (int k = 0; k < posL.Length; k++)
                    {

                        if (endPOS.Contains(posL[k]))
                        {
                            return true;
                        }

                        if (posL[k] == okList[j])
                        {
                            flag = true;
                            break;
                        }
                        else if (j == okList.Length - 1)
                        {
                            return false;
                        }
                    }
                    if (flag) { break; }
                }
            }
            return false;
        }

    }
}
