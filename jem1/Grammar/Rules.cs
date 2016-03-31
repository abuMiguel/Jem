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
            string[] oklist = new string[2] { "adjective", "noun" };
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

        //Infinitive rule, not last word
        public static void InfinitiveRule(Word w, Word wAfter)
        {
            if(wAfter.pos.Contains("verb") && !wAfter.pos.Contains("adjective"))
            {
                w.pos = "infinitive";
            }
            else if (wAfter.pos == "noun" || wAfter.pos.Contains("determiner") || wAfter.pos == "adjective" || wAfter.pos == "pronoun")
            {
                w.pos = "preposition";
            }
        }

        //NOT first word, any word after infinitive must be adverb or verb
        public static void ToBeforeRule(List<string> posL, Word w, Word wBefore)
        {
            if(wBefore.pos == "infinitive")
            {
                posL.RemoveAll(x => x != "verb" && x != "adverb");
                w.pos = ListToString(posL);

            }
            else if (wBefore.pos == "preposition")
            {
                posL.Remove("verb"); posL.Remove("adverb");
            }
        }

        //Determiner pronoun disambiguation
        public static void DetPronounRule(Word w, Sentence s, List<string> posL)
        {
            if(NothingButBetweenForwardContains(w, "noun,pronoun", new string[3] { "adjective", "determiner", "possessive determiner" }, s) == true)
            {
                w.pos = "determiner";
            }
            else
            {
                posL.RemoveAll(x => x.Contains("determiner"));
            }
        }

        //Determiner pronoun at the end of a sentence
        public static void DetPronounRule(Word w, List<string> posL)
        {
            posL.RemoveAll(x => x.Contains("determiner"));
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

        //assume an unknown word ending in S is a plural noun
        public static void UnknownSRule(Word w)
        {
            w.pos = "noun";
            w.isPlural = true;
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
