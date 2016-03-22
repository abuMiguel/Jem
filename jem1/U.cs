using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1
{
    static class U
    {
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

        //Determine whether the words preceding a word up until a particular part of speech, are all
        //words listed in an OK list. This allows a rule to apply where there are an infinite number 
        //of words inbetween that don't disturb the rule
        public static bool NothingButBetween(Word startWord, string endPOS, string[] okList, Sentence s)
        {
            for (int i = startWord.ID - 1; i >= 0; i--)
            {
                var posL = s.words[i].pos.Contains(",") ? s.words[i].pos.Split(',') : new string[1] { s.words[i].pos };

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

            return true;
        }

        
        //Same as NothingButBetween except it looks forward not backwards
        public static bool NothingButBetweenForward(Word startWord, string endPOS, string[] okList, Sentence s)
        {
            for (int i = startWord.ID + 1; i < s.WordCount(); i++)
            {
                var posL = s.words[i].pos.Contains(",") ? s.words[i].pos.Split(',') : new string[1] { s.words[i].pos };

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
            return true;
        }
       
    }
}
