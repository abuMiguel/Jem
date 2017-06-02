using jem1.Structure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static jem1.U;
using jem1.Grammar;
using jem1.API;
using System.Text.RegularExpressions;
using static jem1.DB.DBConn;

namespace jem1.Grammar
{
    static class POSTagger
    {
        public static string TagString(string s)
        {
            var sent = new Sentence(s);
            //TO DO: figure out how to return this for a POS Tagging API
            return s;
        }

        //Parts of speech must be unambigous before using TagClause
        public static void TagClause(Clause c)
        {
            int verbPosition = -1;
            int verbCount = 0;
            var nounIds = new List<int>();

            foreach (Word w in c.Words)
            {
                int cID = w.Id - c.Words[0].Id;

                switch (w.Pos)
                {
                    case "verb":
                        if (!w.InRelPhrase && !w.InPrepPhrase)
                        {
                            if (verbCount == 0)
                            {
                                verbPosition = cID;
                            }
                            c.Verbs.Add(w);
                            verbCount++;
                        }
                        break;
                    case "helper verb":
                        if (!w.InRelPhrase)
                        {
                            if (verbCount == 0)
                            {
                                verbPosition = cID;
                            }
                            c.HVerb = w.Name;
                            c.Verbs.Add(w);
                            verbCount++;
                        }
                        break;
                    case "linking verb":
                        if (!w.InRelPhrase)
                        {
                            if (verbCount == 0)
                            {
                                verbPosition = cID;
                            }
                            c.Verbs.Add(w);
                            verbCount++;
                        }
                        break;
                    case "preposition":
                        var ppwords = new List<Word>();
                        for (int i = cID; i < c.WordCount; i++)
                        {
                            c.Words[i].InPrepPhrase = true;
                            ppwords.Add(c.Words[i]);
                            if (c.Words[i].Pos.Contains("noun"))
                            {
                                c.Words[i].Role = "object of the preposition";
                                break;
                            }
                        }
                        var pp = new PrepPhrase(ppwords);
                        c.Preps.Add(pp);
                        break;
                    case "relative pronoun":
                        var vrbs = new List<string>();
                        // when to end the relative phrase
                        bool end = false;
                        for (int i = cID; i < c.WordCount; i++)
                        {
                            switch (c.Words[i].Pos)
                            {
                                case "verb":
                                    vrbs.Add("v");
                                    break;
                                case "helper verb":
                                case "linking verb":
                                    vrbs.Add("hlv");
                                    break;
                            }
                            // if more than 1 Verb AND current word's POS is Verb
                            if (vrbs.Count > 1 && c.Words[i].Pos.Contains("verb"))
                            {
                                switch (vrbs.Count)
                                {
                                    case 2:   // verb-verb OR verb-helper verb
                                        if (vrbs[0] == "v" && (vrbs[1] == "v" || vrbs[1] == "hlv"))
                                        {
                                            end = true;
                                        }     // helper verb-verb AND word before is NOT helper verb
                                        else if (vrbs[0] == "hlv" && vrbs[1] == "v" && (c.Words[i - 1].Pos != "helper verb" || c.Words[i - 1].Pos != "linking verb"))
                                        {
                                            end = true;
                                        }
                                        break;
                                    case 3:
                                        break;
                                }
                            }
                            if (end)
                            {
                                break;
                            }
                            else { c.Words[i].InRelPhrase = true; }
                        }
                        break;
                    case "noun":
                        if (!nounIds.Contains(cID))
                        {
                            var ids = new List<int>();
                            for (int i = cID; i < c.WordCount; i++)
                            {
                                if (c.Words[i].Pos == "noun" || c.Words[i].Pos == "proper noun" || c.Words[i].Pos == "adjective")
                                {
                                    ids.Add(i);
                                }
                                else { break; }
                            }

                            //remove all adjectives at the end if there are any?
                            while(c.Words[ids[ids.Count - 1]].Pos == "adjective")
                            {
                                ids.RemoveAt(ids.Count - 1);
                            }

                            if (ids.Count > 1)
                            {
                                foreach (int id in ids)
                                {   //all nouns before the last noun in a sequence become descriptors of the last noun
                                    if (id != ids[ids.Count - 1])
                                    {
                                        c.Words[id + 1].Descriptor.Add(c.Words[id].Name);
                                        if (c.Words[id].Role == "object of the preposition")
                                        {
                                            c.Words[id + 1].Role = "object of the preposition";
                                        }
                                        //if noun, add it to list of noun ids so we don't iterate through same nouns more than once
                                        if (c.Words[id].Pos != "adjective")
                                        {
                                            c.Words[id].Role = "adjective";
                                            nounIds.Add(id);
                                        }
                                    }
                                }
                            }   //nounFlag is used to avoid iterating twice through the same set of consecutive nouns
                        }
                        break;
                    case "pronoun":
                        if (cID > 0)
                        {
                            if (NothingButBetween(w, "preposition", new string[2] { "determiner", "predeterminer" }, c))
                            {
                                w.Role = "object of the preposition";
                            }
                        }
                        break;
                }
            }

            //set i2 to the verb if the verb is not first
            if (verbCount > 0 && verbPosition > 0)
            {
                c.I2 = c.Words[verbPosition];
            }

            if (verbPosition >= 0)
            {

                int wordCnt = c.WordCount;

                for (int i = 0; i < wordCnt; i++)
                {

                    //Subject comes before verb
                    if (i < verbPosition)
                    {
                        if (c.Words[i].Pos.Contains("noun") && c.Words[i].Role != "object of the preposition" && c.Words[i].Role != "adjective")
                        {
                            c.Words[i].Role = "subject";
                            c.Subjects.Add(c.Words[i]);
                            c.I1 = c.Words[i];
                        }
                    }
                    else
                    {
                        //The predicate nominative is a noun after the verb
                        if (c.Words[i].Pos.Contains("noun") && c.Words[i].Role != "object of the preposition" && c.Words[i].Role != "adjective")
                        {
                            c.Words[i].Role = "predicate nominative";
                            c.PN.Add(c.Words[i]);
                            if (c.I3 == null)
                            {
                                c.I3 = c.Words[i];
                            }
                        }
                        else
                        {   //Anything after the verb is in the predicate

                        }

                        c.Predicate.Add(c.Words[i]);
                    }

                } //End Looping through words

                //Set Predicate Adjective if there is no Predicate Nominative
                if (c.PN.Count == 0)
                {
                    foreach (Word w in c.Predicate)
                    {
                        if (w.Pos == "adjective" && !w.InPrepPhrase)
                        {
                            if (c.I3 == null)
                            {
                                c.I3 = w;
                            }
                            w.Role = "predicate adjective";
                            c.PA.Add(w);
                            if (c.Subjects != null)
                            {
                                foreach (Word subject in c.Subjects)
                                {
                                    subject.Descriptor.Add(w.Name);
                                }
                            }
                        }
                    }
                }

                int adjFlag = 0;
                //find the adjectives and adverbs and what they modify
                foreach (Word w in c.Words)
                {
                    int cID = w.Id - c.Words[0].Id;
                    switch (w.Pos)
                    {
                        case "adjective":
                            if (adjFlag == 0)
                            {
                                var ids = new List<int>();
                                for (int i = cID; i < c.WordCount; i++)
                                {
                                    if (c.Words[i].Pos == "adjective" && c.Words[i].Role != "predicate adjective")
                                    {
                                        ids.Add(i);
                                    }
                                    else if (c.Words[i].Pos == "noun")
                                    {
                                        break;
                                    }
                                }
                                if (ids.Count > 0)
                                {
                                    foreach (int id in ids)
                                    {   //all adjectives before the noun in a sequence become descriptors of the noun
                                        if (c.Words.Count > ids.Last() + 1)
                                        {
                                            c.Words[ids.Last() + 1].Descriptor.Add(c.Words[id].Name);
                                        }
                                        adjFlag++;
                                    }
                                }   //adjFlag is used to avoid iterating twice through the same set of consecutive adjectives
                            }
                            else { adjFlag--; }
                            break;
                        case "adverb":
                            // not the last word in the sentence
                            if (cID != c.WordCount - 1)
                            {
                                // wa = word after the adverb
                                var wa = c.Words[cID + 1].Pos;
                                if (wa == "adjective" || wa.Contains("verb") || wa == "adverb" && c.Predicate.Contains(w))
                                {
                                    c.Words[cID + 1].Descriptor.Add(w.Name);
                                }
                                else
                                {
                                    foreach (Word verb in c.Verbs)
                                    {
                                        verb.Descriptor.Add(w.Name);
                                    }
                                }
                            }
                            else
                            {
                                foreach (Word verb in c.Verbs)
                                {
                                    verb.Descriptor.Add(w.Name);
                                }
                            }
                            break;
                    }
                }
            }
        }

        public static void AssignPartsOfSpeech(Sentence s)
        {
            foreach (Word word in s.Words)
            {
                if (string.IsNullOrEmpty(word.Pos))
                {
                    if (!string.IsNullOrEmpty(word.PossessiveTag))
                    {
                        word.RemovePossessiveTag();
                    }

                    word.Pos = GetWordID(word.Name) > 0 ? GetPOS(word.Name) : "unknown";
                }
            }
            Deconflict(s);
        }

        public static void Deconflict(Sentence s)
        {
            //Amount of times Sentence should pass through the deconfliction rules
            int pass = 3;

            for (int i = 1; i <= pass; i++)
            {
                foreach (Word w in s.Words)
                {
                    var posL = CSVToList(w.Pos);

                    if (w.Pos.Contains(","))  //has more than one choice for POS
                    {
                        if (s.WordCount == 1) //Only one word in sentence
                        {
                            //just give it the default most commonly used pos because there is no context
                            w.Pos = posL[0];
                        }
                        else if (w.Id == 0 && s.WordCount > 1) //First word
                        {
                            var wAfter = s.Words[w.Id + 1];
                            RunFirstWordRules(wAfter, posL, w, s, i);
                        }
                        else if (w.Id > 0 && w.Id != s.WordCount - 1) //NOT first word OR last word
                        {
                            var wBefore = s.Words[w.Id - 1];
                            var wAfter = s.Words[w.Id + 1];
                            RunMiddleWordRules(wBefore, wAfter, posL, w, s, i);
                        }
                        else if (w.Id == s.WordCount - 1)  //Last word
                        {
                            var wBefore = s.Words[w.Id - 1];
                            RunLastWordRules(wBefore, posL, w, s, i);
                        }

                        //After the LAST pass through the rules, set the words that are still
                        //ambiguous to the first POS in the POS list
                        if (posL.Count > 0 && i == pass)
                        {
                            w.Pos = posL[0];
                        }
                    }
                    else
                    {
                        //The POS remains AS IS because there was only one choice, unless unknown
                        if (w.Pos != "unknown") continue;

                        //Lookup word's possible parts of speech on dictionary.com on first pass
                        if (i == 1)
                        {
                            string dcomPos, wiki;
                            try
                            {
                                dcomPos = DCom.Scrape(w.Name);

                                //test
                                wiki = Wiki.Lookup(w.Name);
                                //test
                            }
                            catch
                            {
                                dcomPos = string.Empty;
                                wiki = string.Empty;
                            }

                            if (!string.IsNullOrEmpty(dcomPos))
                            {
                                w.Pos = dcomPos;
                                //make word lower case if it's not a proper noun
                                if (!w.Pos.Contains("proper noun")) { w.Name = w.Name.ToLower(); }
                                if(w.Pos.Contains("auxiliary verb")) { w.Pos = w.Pos.Replace("auxiliary verb", "helper verb"); }
                                InsertEng(w.Name, w.Pos);
                            }
                            else
                            {
                                //word starting with uppercase letter that is not the first word of a sentence
                                //and whose pos could not be found in the dictionary API, is likely a Proper Noun
                                if (w.Id != 0 && char.IsUpper(w.Name[0]))
                                {
                                    w.Pos = "proper noun";
                                    InsertEng(w.Name, w.Pos);
                                }
                                Console.WriteLine("Dictionary.com has no entry for " + w.Name);
                            }
                        }

                        if (!string.IsNullOrEmpty(w.Pos) && w.Pos != "unknown") continue;

                        if (s.WordCount > 1)
                        {
                            RunGeneralUnknownRules(w, s);
                        }
                        else if (s.WordCount == 1) //Only one word in sentence
                        {

                        }
                        else if (w.Id == 0 && s.WordCount > 1) //First word
                        {
                            var wAfter = s.Words[w.Id + 1];
                            RunUnknownFirstRules(wAfter, w, s);
                        }
                        else if (w.Id > 0 && w.Id != s.WordCount - 1) //NOT first word OR last word
                        {
                            var wBefore = s.Words[w.Id - 1];
                            //var wAfter = s.Words[w.Id + 1];
                            RunUnknownMiddleRules(wBefore, w, s);
                        }
                        else if (w.Id == s.WordCount - 1)  //Last word
                        {
                            var wBefore = s.Words[w.Id - 1];
                            RunUnknownLastRules(wBefore, w, s);
                        }
                    }
                }
            }
        }

        private static void RunFirstWordRules(Word wAfter, List<string> posL, Word w, Sentence s, int pass)
        {
            //can safely check the word after
            if (U.EndsWith(w, "ing")) { Rules.IngStartRule(posL, w); }
            if (w.Name == "to") { Rules.InfinitiveRule(w, wAfter); }
            if (w.Pos.Contains("determiner") && w.Pos.Contains("pronoun") && pass == 2) { Rules.DetPronounRule(w, s, posL); }
            if (!string.IsNullOrEmpty(w.PossessiveTag)) { w.Pos = "possessive determiner"; } //Why did I do this?
            if (w.Pos.Contains("relative pronoun")) { Rules.FirstRelPro(w, posL); }
        }

        private static void RunMiddleWordRules(Word wBefore, Word wAfter, List<string> posL, Word w, Sentence s, int pass)
        {
            //can safely check the word before AND the word after
            if ((w.Pos.Contains("noun") || posL.Contains("adjective")) && !w.Pos.Contains("determiner")) { Rules.DeterminerPrecedingRule(wBefore, posL, w, s); }
            if (U.EndsWith(w, "ing") && posL.Contains("verb")) { Rules.IngVerbRule(wBefore, posL, w); }
            if (posL.Contains("relative pronoun") && posL.Count > 1) { Rules.RelativePronounRule(wBefore, w); }
            if (w.Name == "to") { Rules.InfinitiveRule(w, wAfter); }
            if (wBefore.Pos == "infinitive") { Rules.ToBeforeRule(posL, w, wBefore); }
            if (w.Pos.Contains("determiner") && w.Pos.Contains("pronoun") && pass == 2) { Rules.DetPronounRule(w, s, posL); }
            if (w.Pos.Contains("adjective") && w.Pos.Contains("adverb")) { Rules.AdjAdvRule(w, wAfter, s); }
            if (posL.Contains("verb") && wBefore.Pos == "verb" && posL.Contains("noun")) { Rules.DoubleVerbRule(w, s); }
            if (posL.Contains("verb") && posL.Contains("noun") && wBefore.Pos == "pronoun") { Rules.HeSheItNounVerbRule(w, wBefore, s); }
            //if (wBefore.Pos == "noun" && posL.Contains("noun") && posL.Contains("verb")) { Rules.}
            if (!string.IsNullOrEmpty(w.PossessiveTag)) { w.Pos = "possessive determiner"; }

        }

        private static void RunLastWordRules(Word wBefore, List<string> posL, Word w, Sentence s, int pass)
        {
            //can safely check the word before
            if ((w.Pos.Contains("noun") || posL.Contains("adjective")) && !w.Pos.Contains("determiner")) { Rules.DeterminerPrecedingRule(wBefore, posL, w, s); }
            if (U.EndsWith(w, "ing") && posL.Contains("verb")) { Rules.IngVerbRule(wBefore, posL, w); }
            if (w.Name == "to") { w.Pos = "preposition"; }
            if (wBefore.Pos == "infinitive") { Rules.ToBeforeRule(posL, w, wBefore); }
            if (w.Pos.Contains("determiner") && w.Pos.Contains("pronoun")) { Rules.DetPronounRule(w, posL); }
            if (wBefore.Pos == "determiner" || wBefore.Pos == "predeterminer" || wBefore.Pos == "possessive determiner") { Rules.DeterminerPrecedingEndRule(wBefore, w, posL); }
            if (!string.IsNullOrEmpty(w.PossessiveTag)) { w.Pos = "possessive determiner"; }
        }

        private static void RunGeneralUnknownRules(Word w, Sentence s)
        {
            if (U.EndsWith(w, "s")) { Rules.UnknownSRule(w, s); }
            if (U.EndsWith(w, "ly")) { Rules.LyAdverbRule(w); }
            if (!string.IsNullOrEmpty(w.PossessiveTag)) { w.Pos = "possessive determiner"; }
            if (SpecChars.ContainsSpecChar(w.Name)) { Rules.UnknownSpecialCharactersRule(w, s); }

            //if the pos is still unknown, check if it is a number
            if (w.Pos == "unknown" || string.IsNullOrEmpty(w.Pos))
            {
                var regex = new Regex(@"^[0-9]+$");
                if (regex.IsMatch(w.Name)) { w.Pos = "noun,determiner"; }
            }

        }

        private static void RunUnknownFirstRules(Word wAfter, Word w, Sentence s)
        {

        }

        private static void RunUnknownMiddleRules(Word wBefore, Word w, Sentence s)
        {
            Rules.UnknownMiddleWordAfterDetRule(wBefore, w, s);
        }

        private static void RunUnknownLastRules(Word wBefore, Word w, Sentence s)
        {
            Rules.UnknownLastWordAfterDetRule(wBefore, w, s);
        }

        //Get the abbreviation for the POS of a given word
        public static string GetAbbrev(Word w)
        {
            string abbr = "";
            switch (w.Pos)
            {
                case "noun":
                    //if (w.role == "object of the preposition") { abbr = "OP"; }
                    //else if (w.role == "predicate nominative") { abbr = "PrN"; }
                    //else if (w.role == "subject") { abbr = "SN"; }
                    abbr = w.Role == "adjective" ? "JJ" : "NN";
                    break;
                case "proper noun":
                    //if (w.role == "object of the preposition") { abbr = "OP"; }
                    //else if (w.role == "predicate nominative") { abbr = "PrN"; }
                    //else if (w.role == "subject") { abbr = "SN"; }
                    //else if (w.role == "adjective") { abbr = "Adj"; }
                    abbr = "NNP";
                    break;
                case "verb":
                    abbr = "VB";
                    break;
                case "adjective":
                    //if (w.role == "predicate adjective") { abbr = "PA"; }
                    abbr = "JJ"; 
                    break;
                case "adverb":
                    abbr = "RB";
                    break;
                case "subordinating conjunction":
                    abbr = "SC";
                    break;
                case "helper verb":
                    abbr = "HV";
                    break;
                case "linking verb":
                    abbr = "LV";
                    break;
                case "pronoun":
                    //if (w.role == "subject") { abbr = "SP"; }
                    //else if (w.role == "object of the preposition") { abbr = "OP"; }
                    abbr = "PRP"; 
                    break;
                case "possessive pronoun":
                    //if (w.role == "subject") { abbr = "SP"; }
                    abbr = "PRP$"; 
                    break;
                case "conjunction":
                    abbr = "C";
                    break;
                case "preposition":
                    abbr = "IN";
                    break;
                case "relative pronoun":
                    abbr = "RPRP";
                    break;
                case "determiner":
                    abbr = "DT";
                    break;
                case "unknown":
                    abbr = "?";
                    break;
                case "interjection":
                    abbr = "UH";
                    break;
                case "infinitive":
                    abbr = "INF";
                    break;
                case "coordinating conjunction":
                    abbr = "CC";
                    break;
                case "possessive determiner":
                    abbr = "DT";
                    break;
                case "predeterminer":
                    abbr = "PDT";
                    break;
                case "gerund":
                    abbr = "GER";
                    break;
                case "particle":
                    abbr = "RP";
                    break;
                default:
                    abbr = string.IsNullOrEmpty(w.Pos) ? "ERR" : w.Pos;
                    break;
            }
            return abbr;
        }
    }
}
