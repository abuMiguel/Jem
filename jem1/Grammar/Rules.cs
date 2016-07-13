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
        //first word is not a rel pro
        public static void FirstRelPro(Word w, List<string> posL)
        {
            posL.Remove("relative pronoun");
            w.pos = ListToString(posL);
        }

        //Determiner preceding rule 
        public static void DeterminerPrecedingRule(Word wBefore, List<string> posL, Word w, Sentence s)
        {
            string[] oklist = new string[2] { "adjective", "adverb" };

            if (wBefore.pos.Contains("determiner") || NothingButBetween(w, "determiner", oklist, s))
            {
                posL.RemoveAll(x => !x.Contains("noun") && !x.Contains("adjective") && !x.Contains("adverb"));
            }

            w.pos = ListToString(posL);
        }

        //word at end of sentence with preceding determiner
        public static void DeterminerPrecedingEndRule(Word wBefore, Word w, List<string> posL)
        {
            posL.RemoveAll(x => !x.Contains("noun"));
            if(posL.Count == 0) { posL.Add("noun"); }
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
            posL.Remove("verb");
            w.pos = ListToString(posL);
        }

        //Infinitive rule, not last word
        public static void InfinitiveRule(Word w, Word wAfter)
        {
            if (wAfter.pos.Contains("verb") && !wAfter.pos.Contains("adjective"))
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
            if (wBefore.pos == "infinitive")
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
            if (NothingButBetweenForwardContains(w, "noun,pronoun", new string[3] { "adjective", "determiner", "possessive determiner" }, s) == true)
            {
                if(posL.Contains("predeterminer")){  w.pos = "predeterminer";  }
                else { w.pos = "determiner"; }
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
            if (wBefore.pos == "noun" || wBefore.pos == "pronoun")
            {
                w.pos = "relative pronoun";
            }
        }

        //Middle word, adj adv disambiguation
        public static void AdjAdvRule(Word w, Word wAfter, Sentence s)
        {
            if (wAfter.pos.Contains("adverb") || wAfter.pos.Contains("adjective") || wAfter.pos.Contains("determiner"))
            {
                if (U.NothingButBetweenForwardContains(w, "noun", new string[6] { "adverb", "adjective", "determiner", "predeterminer", "coordinating conjunction", "conjunction" }, s))
                {
                    w.pos = "adverb";
                }
            }
        }

        //best guess for a word ending in 'ly' is adverb
        public static void LyAdverbRule(Word w)
        {
            w.pos = "adverb"; 
        }

        //assume an unknown word ending in S is a plural noun
        public static void UnknownSRule(Word w, Sentence s)
        {
            if (HasPOS(s, new List<string> { "verb", "helper verb", "linking verb" }))
            {
                w.pos = "noun";
                w.isPlural = true;
            }
            else
            {
                w.pos = "verb";
            }
        }

        public static void UnknownMiddleWordAfterDetRule(Word wBefore, Word w, Sentence s)
        {
            //Determiner preceding rule
            string[] oklist = new string[4] { "adjective", "noun", "unknown", "adverb" };
            if (wBefore.pos.Contains("determiner"))
            {
                w.pos = "noun,adjective";
            }

        }

        public static void UnknownLastWordAfterDetRule(Word wBefore, Word w, Sentence s)
        {
            //Determiner preceding rule
            string[] oklist = new string[4] { "adjective", "noun", "unknown", "adverb" };
            if (wBefore.pos.Contains("determiner"))
            {
                w.pos = "noun";
            }
        }
    }
}
