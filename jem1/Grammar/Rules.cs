using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jem1.Structure;
using static jem1.U;
using static jem1.Grammar.SpecChars;

namespace jem1.Grammar
{
    internal static class Rules
    {
        //first word is not a rel pro
        public static void FirstRelPro(Word w, List<string> posL)
        {
            posL.Remove("relative pronoun");
            w.Pos = ListToString(posL);
        }

        //Determiner preceding rule 
        public static void DeterminerPrecedingRule(Word wBefore, List<string> posL, Word w, Sentence s)
        {
            var oklist = new[] { "adjective", "adverb" };

            if (wBefore.Pos.Contains("determiner") || NothingButBetween(w, "determiner", oklist, s))
            {
                posL.RemoveAll(x => !x.Contains("noun") && !x.Contains("adjective") && !x.Contains("adverb"));
            }

            w.Pos = ListToString(posL);
        }

        //word at end of sentence with preceding determiner
        public static void DeterminerPrecedingEndRule(Word wBefore, Word w, List<string> posL)
        {
            posL.RemoveAll(x => !x.Contains("noun"));
            if (posL.Count == 0) { posL.Add("noun"); }
            w.Pos = ListToString(posL);
        }

        //Is the ING word preceded by 
        public static void IngVerbRule(Word wBefore, List<string> posL, Word w)
        {
            //ING verb rule
            if (wBefore.Pos == "helper verb" || wBefore.Pos == "linking verb")
            {
                posL.RemoveAll(x => x != "verb");
            }

            w.Pos = ListToString(posL);
        }

        //Is the ING word the first word 
        public static void IngStartRule(List<string> posL, Word w)
        {
            posL.Remove("verb");
            w.Pos = ListToString(posL);
        }

        //Infinitive rule, not last word
        public static void InfinitiveRule(Word w, Word wAfter)
        {
            if (wAfter.Pos.Contains("verb") && !wAfter.Pos.Contains("adjective"))
            {
                w.Pos = "infinitive";
            }
            else if (wAfter.Pos == "noun" || wAfter.Pos.Contains("determiner") || wAfter.Pos == "adjective" || wAfter.Pos == "pronoun")
            {
                w.Pos = "preposition";
            }
        }

        //NOT first word, any word after infinitive must be adverb or verb
        public static void ToBeforeRule(List<string> posL, Word w, Word wBefore)
        {
            if (wBefore.Pos == "infinitive")
            {
                posL.RemoveAll(x => x != "verb" && x != "adverb");
                w.Pos = ListToString(posL);

            }
            else if (wBefore.Pos == "preposition")
            {
                posL.Remove("verb"); posL.Remove("adverb");
            }
        }

        //Determiner pronoun disambiguation
        public static void DetPronounRule(Word w, Sentence s, List<string> posL)
        {
            if (NothingButBetweenForwardContains(w, "noun,pronoun", new string[] { "adjective", "determiner", "possessive determiner" }, s))
            {
                w.Pos = posL.Contains("predeterminer") ? "predeterminer" : "determiner";
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
            if (wBefore.Pos == "noun" || wBefore.Pos == "pronoun")
            {
                w.Pos = "relative pronoun";
            }
        }

        //Middle word, adj adv disambiguation
        public static void AdjAdvRule(Word w, Word wAfter, Sentence s)
        {
            if (wAfter.Pos.Contains("adverb") || wAfter.Pos.Contains("adjective") || wAfter.Pos.Contains("determiner"))
            {
                if (NothingButBetweenForwardContains(w, "noun", new[] { "adverb", "adjective", "determiner", "predeterminer", "coordinating conjunction", "conjunction" }, s))
                {
                    w.Pos = "adverb";
                }
            }
        }

        public static void HeSheItNounVerbRule(Word w, Word wBefore, Sentence s)
        {
            var wb = wBefore.Name.ToLower();
            if (wb == "he" || wb == "she" || wb == "it")
            {
                w.Pos = "verb";
            }
        }

        public static void ProperSentenceCorrectionRule(Sentence s)
        {
            bool lacksVerb = !s.Clauses.Any(x => x.Verbs.Count > 0);

            if (lacksVerb)
            {
                
            }

        }

        //Middle word, disambiguate between verb and noun when there are two verbs in a row
        public static void DoubleVerbRule(Word w, Sentence s)
        {
            w.Pos = "noun";
        }

        //best guess for a word ending in 'ly' is adverb
        public static void LyAdverbRule(Word w)
        {
            w.Pos = "adverb";
        }

        //assume an unknown word ending in S is a plural noun
        public static void UnknownSRule(Word w, Sentence s)
        {
            if (HasPOS(s, new List<string> { "verb", "helper verb", "linking verb" }))
            {
                w.Pos = "noun";
                w.IsPlural = true;
            }
            else
            {
                w.Pos = "verb";
            }
        }

        public static void UnknownMiddleWordAfterDetRule(Word wBefore, Word w, Sentence s)
        {
            //Determiner preceding rule
            //var oklist = new[] { "adjective", "noun", "unknown", "adverb" };
            if (wBefore.Pos.Contains("determiner"))
            {
                w.Pos = "noun,adjective";
            }

        }

        public static void UnknownLastWordAfterDetRule(Word wBefore, Word w, Sentence s)
        {
            //Determiner preceding rule
            //var oklist = new[] { "adjective", "noun", "unknown", "adverb" };
            if (wBefore.Pos.Contains("determiner"))
            {
                w.Pos = "noun";
            }
        }

        public static void UnknownSpecialCharactersRule(Word w, Sentence s)
        {
            //find what special chars are being used
            Dictionary<int, char> sc = GetSpecialCharacters(w.Name);

            foreach (int id in sc.Keys)
            {
                if (w.Pos == "unknown" || string.IsNullOrEmpty(w.Pos))
                {
                    switch (sc[id])
                    {
                        case '@':
                            w.Pos = At(sc, w);
                            break;
                        case '#':
                            w.Pos = HashTag(sc, w);
                            break;
                        case '$':
                            break;
                        case '(':
                            break;
                        case ')':
                            break;
                        case '<':
                            break;
                        case '>':
                            break;
                        case '/':
                            break;
                        case '\\':
                            break;
                        case '+':
                            break;
                        case '-':
                            break;
                        case '&':
                            w.Pos = Ampersand(sc, w.Name);
                            break;
                        case '*':
                            break;
                        case '=':
                            w.Pos = SpecChars.Equals(sc, w.Name);
                            break;
                        case '~':
                            break;
                        case '[':
                            break;
                        case ']':
                            break;
                        case '{':
                            break;
                        case '}':
                            break;
                        case '_':
                            break;
                        case '^':
                            break;
                        case '.':
                            w.Pos = URL(sc, w);
                            break;
                        default: break;
                    }
                }
            }
        }
    }
}
