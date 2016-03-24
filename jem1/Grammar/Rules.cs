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
        //Determiner preceding rule overloaded for last word only.
        public static void DeterminerPrecedingRule(Word wBefore, List<string> posL, Word w, Sentence s)
        {
            string[] oklist = new string[3] { "adjective", "noun", "unknown" };
            //Determiner preceding rule
            if (wBefore.pos.Contains("determiner") || NothingButBetween(w, "determiner", oklist, s))
            {
                posL.RemoveAll(x => !x.Contains("noun") && !x.Contains("adjective"));
            }

            w.pos = ListToString(posL);
        }

        //Is the ING word preceded by 
        public static void IngVerbRule(Word wBefore, List<string> posL, Word w)
        {
            //ING verb rule
            if (wBefore.pos == "helper verb" || wBefore.pos == "linking verb")
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

        //(middle word) any relative pronoun possibility meeting this requirement is a relative pronoun
        public static void RelativePronounRule(Word wBefore, Word w)
        {
            if(wBefore.pos == "noun" || wBefore.pos == "pronoun")
            {
                w.pos = "relative pronoun";
            }
        }

        //best guess for a word ending in 'ly' is adverb
        public static void LyAdverbRule(Word w)
        {
            if(EndsWith(w, "ly")) { w.pos = "adverb"; }
        }

        public static void UnknownMiddleRules(Word wBefore, Word wAfter, Word w, Sentence s)
        {
            //Determiner preceding rule
            string[] oklist = new string[3] { "adjective", "noun", "unknown" };
            if (wBefore.pos.Contains("determiner"))
            {
                w.pos = "noun,adjective";
            }
            LyAdverbRule(w);

        }

        public static void UnknownLastRules(Word wBefore, Word w, Sentence s)
        {
            //Determiner preceding rule
            string[] oklist = new string[3] { "adjective", "noun", "unknown" };
            if (wBefore.pos.Contains("determiner"))
            {
                w.pos = "noun";
            }
            LyAdverbRule(w);
        }
    }
}
