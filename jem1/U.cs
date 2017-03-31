using jem1.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace jem1
{
    internal static class U
    {
        public static List<string> CSVToList(string csv)
        {
            var x = new List<string>();
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
                var rst = "";
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

            if (w.Name.Length > num)
            {
                wordReverseEnding = new string(w.Name.Reverse().Take(num).ToArray());
            }

            return wordReverseEnding == new string(ending.Reverse().ToArray()) ? true : false;
        }

        public static bool HasPOS(Sentence s, List<string> posL)
        {
            foreach (Word w in s.Words)
            {
                if (posL.Contains(w.Pos)) { return true; }
            }
            return false;
        }

        public static bool IsCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input)) { return false; }
            var regex = new Regex("[A-Z]([A-Z0-9]*[a-z][a-z0-9]*[A-Z]|[a-z0-9]*[A-Z][A-Z0-9]*[a-z])[A-Za-z0-9]*");
            return regex.IsMatch(input);
        }

        public static string SplitCamelCase(string input)
        {
            var output = Regex.Replace(input, "([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            return output;
        }

        //Determine whether the words preceding a word up until a particular part of speech, are all
        //words listed in an OK list. This allows a rule to apply where there are an infinite number 
        //of words inbetween that don't disturb the rule.
        public static bool NothingButBetween(Word startWord, string endPOS, string[] okList, Sentence s)
        {
            for (int i = startWord.Id - 1; i >= 0; i--)
            {
                var posL = s.Words[i].Pos.Contains(",") ? s.Words[i].Pos.Split(',') : new string[1] { s.Words[i].Pos };

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
            int cID = startWord.Id - c.Words[0].Id;

            for (int i = cID - 1; i >= 0; i--)
            {
                var posL = c.Words[i].Pos.Contains(",") ? c.Words[i].Pos.Split(',') : new string[1] { c.Words[i].Pos };

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
            for (int i = startWord.Id + 1; i < s.WordCount; i++)
            {
                var posL = s.Words[i].Pos.Contains(",") ? s.Words[i].Pos.Split(',') : new string[1] { s.Words[i].Pos };

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
            for (int i = startWord.Id + 1; i < s.WordCount; i++)
            {
                var posL = s.Words[i].Pos.Contains(",") ? s.Words[i].Pos.Split(',') : new string[1] { s.Words[i].Pos };

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
