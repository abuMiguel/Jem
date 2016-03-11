using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static jem1.U;

namespace jem1.Grammar
{
    static class Rules
    {
        //Determiner preceeding rule overloaded for last word only.
        public static void DeterminerPreceedingRule(Word wBefore, List<string> posL, Word w, Sentence s)
        {
            string[] oklist = new string[3] { "adjective", "noun", "unknown" };
            //Determiner preceeding rule
            if (wBefore.pos.Contains("determiner") || NothingButBetween(w, "determiner", oklist, s))
            {
                posL.RemoveAll(x => !x.Contains("noun") && !x.Contains("adjective"));
            }

            w.pos = ListToString(posL);
        }

        //Does the word come after a determiner?  This is for middle words only.
        public static void DeterminerPreceedingRule(Word wBefore, Word wAfter, List<string> posL, Word w, Sentence s)
        {
            string[] oklist = new string[3] { "adjective", "noun", "unknown" };
            //Determiner preceeding rule
            if (wBefore.pos.Contains("determiner") || NothingButBetween(w, "determiner", oklist, s))
            {
                posL.RemoveAll(x => !x.Contains("noun") && !x.Contains("adjective"));
            }

            w.pos = ListToString(posL);
        }

        //Is the ING word preceeded by 
        public static void IngVerbRule(Word wBefore, List<string> posL, Word w)
        {
            //ING verb rule
            if (wBefore.pos == "helper verb")
            {
                posL.RemoveAll(x => x != "verb");
            }

            w.pos = ListToString(posL);
        }

        //Is the ING word the first word 
        public static void IngStartRule(List<string> posL, Word w)
        {
            //ING first word rule
            posL.Remove("verb");
            w.pos = ListToString(posL);
        }

        public static void UnknownMiddleRules(Word wBefore, Word wAfter, Word w, Sentence s)
        {
            //Determiner preceeding rule
            string[] oklist = new string[3] { "adjective", "noun", "unknown" };
            if (wBefore.pos.Contains("determiner") || NothingButBetween(w, "determiner", oklist, s))
            {
                w.pos = "noun,adjective";
            }

        }

        public static void UnknownLastRules(Word wBefore, Word w, Sentence s)
        {
            //Determiner preceeding rule
            string[] oklist = new string[3] { "adjective", "noun", "unknown" };
            if (wBefore.pos.Contains("determiner") || NothingButBetween(w, "determiner", oklist, s))
            {
                w.pos = "noun";
            }
        }
    }
}
