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

namespace jem1.Grammar
{
    static class POSTagger
    {
        public static string TagString(string s)
        {
            Sentence sent = new Sentence(s);
            //TO DO: figure out how to return this for a POS Tagging API
            return s;
        }

        //Parts of speech must be unambigous before using TagClause
        public static void TagClause(Clause c)
        {
            int verbPosition = -1;
            int verbCount = 0;
            int nounFlag = 0;

            foreach (Word w in c.words)
            {
                int cID = w.ID - c.words[0].ID;

                switch (w.pos)
                {
                    case "verb":
                        if (!w.inRelPhrase)
                        {
                            if (verbCount == 0)
                            {
                                verbPosition = cID;
                            }
                            c.verbs.Add(w);
                            verbCount++;
                        }
                        break;
                    case "helper verb":
                        if (!w.inRelPhrase)
                        {
                            if (verbCount == 0)
                            {
                                verbPosition = cID;
                            }
                            c.hVerb = w.name;
                            c.verbs.Add(w);
                            verbCount++;
                        }
                        break;
                    case "linking verb":
                        if (!w.inRelPhrase)
                        {
                            if (verbCount == 0)
                            {
                                verbPosition = cID;
                            }
                            c.verbs.Add(w);
                            verbCount++;
                        }
                        break;
                    case "preposition":
                        List<Word> ppwords = new List<Word>();
                        for (int i = cID; i < c.WordCount(); i++)
                        {
                            c.words[i].inPrepPhrase = true;
                            ppwords.Add(c.words[i]);
                            if (c.words[i].pos.Contains("noun"))
                            {
                                c.words[i].role = "object of the preposition";
                                break;
                            }
                        }
                        PrepPhrase pp = new PrepPhrase(ppwords);
                        c.preps.Add(pp);
                        break;
                    case "relative pronoun":
                        List<string> vrbs = new List<string>();
                        // when to end the relative phrase
                        bool end = false;
                        for (int i = cID; i < c.WordCount(); i++)
                        {
                            if (c.words[i].pos == "verb")
                            {
                                vrbs.Add("v");
                            }
                            else if (c.words[i].pos == "helper verb" || c.words[i].pos == "linking verb")
                            {
                                vrbs.Add("hlv");
                            }
                            // if more than 1 Verb AND current word's POS is Verb
                            if (vrbs.Count > 1 && c.words[i].pos.Contains("verb"))
                            {
                                switch (vrbs.Count)
                                {
                                    case 2:   // verb-verb OR verb-helper verb
                                        if (vrbs[0] == "v" && (vrbs[1] == "v" || vrbs[1] == "hlv"))
                                        {
                                            end = true;
                                        }     // helper verb-verb AND word before is NOT helper verb
                                        else if (vrbs[0] == "hlv" && vrbs[1] == "v" && (c.words[i - 1].pos != "helper verb" || c.words[i - 1].pos != "linking verb"))
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
                            else { c.words[i].inRelPhrase = true; }
                        }
                        break;
                    case "noun":
                        if (nounFlag == 0)
                        {
                            List<int> ids = new List<int>();
                            for (int i = cID; i < c.WordCount(); i++)
                            {
                                if (c.words[i].pos == "noun")
                                {
                                    ids.Add(i);
                                }
                                else { break; }
                            }
                            if (ids.Count > 1)
                            {
                                foreach (int id in ids)
                                {   //all nouns before the last noun in a sequence become descriptors of the last noun
                                    if (id != ids[ids.Count - 1])
                                    {
                                        c.words[id].descriptor.Add(c.words[id].name);
                                        if (c.words[id].role == "object of the preposition")
                                        {
                                            c.words[id + 1].role = "object of the preposition";
                                        }
                                        c.words[id].role = "adjective";
                                        nounFlag++;
                                    }
                                }
                            }   //nounFlag is used to avoid iterating twice through the same set of consecutive nouns
                        }
                        else { nounFlag--; }
                        break;
                }
            }

            //set i2 to the verb if the verb is not first
            if (verbCount > 0 && verbPosition > 0)
            {
                c.i2 = c.words[verbPosition];
            }

            if (verbPosition >= 0)
            {

                int wordCnt = c.WordCount();

                for (int i = 0; i < wordCnt; i++)
                {

                    //Subject comes before verb
                    if (i < verbPosition)
                    {
                        if (c.words[i].pos.Contains("noun") && c.words[i].role != "object of the preposition" && c.words[i].role != "adjective")
                        {
                            c.words[i].role = "subject";
                            c.subjects.Add(c.words[i]);
                            c.i1 = c.words[i];
                        }
                    }
                    else
                    {
                        //The predicate nominative is a noun after the verb
                        if (c.words[i].pos.Contains("noun") && c.words[i].role != "object of the preposition" && c.words[i].role != "adjective")
                        {
                            c.words[i].role = "predicate nominative";
                            c.pN.Add(c.words[i]);
                            if (c.i3 == null)
                            {
                                c.i3 = c.words[i];
                            }
                        }
                        else
                        {   //Anything after the verb is in the predicate

                        }

                        c.predicate.Add(c.words[i]);
                    }

                } //End Looping through words

                //Set Predicate Adjective if there is no Predicate Nominative
                if (c.pN.Count == 0)
                {
                    foreach (Word w in c.predicate)
                    {
                        if (w.pos == "adjective" && !w.inPrepPhrase)
                        {
                            if (c.i3 == null)
                            {
                                c.i3 = w;
                            }
                            w.role = "predicate adjective";
                            c.pA.Add(w);
                            if (c.subjects != null)
                            {
                                foreach (Word subject in c.subjects)
                                {
                                    subject.descriptor.Add(w.name);
                                }
                            }
                        }
                    }
                }

                int adjFlag = 0;
                //find the adjectives and what they modify
                foreach (Word w in c.words)
                {
                    int cID = w.ID - c.words[0].ID;
                    switch (w.pos)
                    {
                        case "adjective":
                            if (adjFlag == 0)
                            {
                                List<int> ids = new List<int>();
                                for (int i = cID; i < c.WordCount(); i++)
                                {
                                    if (c.words[i].pos == "adjective" && c.words[i].role != "predicate adjective")
                                    {
                                        ids.Add(i);
                                    }
                                    else if (c.words[i].pos == "noun")
                                    {
                                        break;
                                    }
                                }
                                if (ids.Count > 0)
                                {
                                    foreach (int id in ids)
                                    {   //all adjectives before the noun in a sequence become descriptors of the noun
                                        if (c.words.Count > ids.Last() + 1)
                                        {
                                            c.words[ids.Last() + 1].descriptor.Add(c.words[id].name);
                                        }
                                        adjFlag++;
                                    }
                                }   //adjFlag is used to avoid iterating twice through the same set of consecutive adjectives
                            }
                            else { adjFlag--; }
                            break;
                        case "adverb":
                            // not the last word in the sentence
                            if (cID != c.WordCount() - 1)
                            {
                                // wa = word after the adverb
                                var wa = c.words[cID + 1].pos;
                                if (wa == "adjective" || wa.Contains("verb") || wa == "adverb" && c.predicate.Contains(w))
                                {
                                    c.words[cID + 1].descriptor.Add(w.name);
                                }
                                else
                                {
                                    foreach (Word verb in c.verbs)
                                    {
                                        verb.descriptor.Add(w.name);
                                    }
                                }
                            }
                            else
                            {
                                foreach (Word verb in c.verbs)
                                {
                                    verb.descriptor.Add(w.name);
                                }
                            }
                            break;
                    }
                }
            }
        }

        public static void AssignPartsOfSpeech(Sentence s)
        {
            string jsonfile, filepath;
            filepath = ConfigurationManager.AppSettings["FilePath"];

            string pos = "";

            foreach (Word word in s.words)
            {
                if (!string.IsNullOrEmpty(word.possessiveTag))
                {
                    word.RemovePossessiveTag();
                }

                jsonfile = !string.IsNullOrEmpty(word.name) ? filepath + word.name[0].ToString() + @"\" + word.name + ".json" : " ";

                if (File.Exists(jsonfile))
                {
                    var singularForm = JO.GetVal(word.name, "singular");
                    //check whether word is plural or singular
                    if (string.IsNullOrEmpty(singularForm))
                    {
                        pos = JO.GetVal(word.name, "pos");  //singular
                    }
                    else
                    {
                        pos = JO.GetVal(singularForm, "pos");  //plural
                        word.isPlural = true;
                    }

                    word.pos = pos;
                }
                else
                {
                    word.pos = "unknown";
                }
            }
            Deconflict(s);
        }

        public static void Deconflict(Sentence s)
        {
            //Amount of times Sentence should pass through the deconfliction rules
            int pass = 2;

            for (int i = 1; i <= pass; i++)
            {
                foreach (Word w in s.words)
                {
                    var posL = CSVToList(w.pos);

                    if (w.pos.Contains(","))  //has more than one choice
                    {
                        if (s.WordCount() == 1) //Only one word in sentence
                        {
                            w.pos = CSVToList(w.pos)[0];
                        }
                        else if (w.ID == 0 && s.WordCount() > 1) //First word
                        {
                            var wAfter = s.words[w.ID + 1];
                            RunFirstWordRules(wAfter, posL, w, s);
                        }
                        else if (w.ID > 0 && w.ID != s.WordCount() - 1) //NOT first word OR last word
                        {
                            var wBefore = s.words[w.ID - 1];
                            var wAfter = s.words[w.ID + 1];
                            RunMiddleWordRules(wBefore, wAfter, posL, w, s);
                        }
                        else if (w.ID == s.WordCount() - 1)  //Last word
                        {
                            var wBefore = s.words[w.ID - 1];
                            RunLastWordRules(wBefore, posL, w, s);

                        }

                        //After the LAST pass through the rules, set the words that are still
                        //ambiguous to the first POS in the POS list
                        if (posL.Count > 0 && i == pass)
                        {
                            w.pos = posL[0];
                        }

                    }
                    else
                    {
                        //The POS remains AS IS because there was only one choice, unless unknown
                        if(w.pos == "unknown")
                        {
                            if (s.WordCount() == 1) //Only one word in sentence
                            {

                            }
                            else if (w.ID == 0 && s.WordCount() > 1) //First word
                            {

                            }
                            else if (w.ID > 0 && w.ID != s.WordCount() - 1) //NOT first word OR last word
                            {
                                var wBefore = s.words[w.ID - 1];
                                var wAfter = s.words[w.ID + 1];
                                RunUnknownMiddleRules(wBefore, wAfter, w, s);
                            }
                            else if (w.ID == s.WordCount() - 1)  //Last word
                            {
                                var wBefore = s.words[w.ID - 1];
                                RunUnknownLastRules(wBefore, w, s);
                            }
                        }
                    }
                }
            }
        }

        private static void RunFirstWordRules(Word wAfter, List<string> posL, Word w, Sentence s)
        {
            //can safely check the word after
            Rules.IngStartRule(posL, w);
        }

        private static void RunLastWordRules(Word wBefore, List<string> posL, Word w, Sentence s)
        {
            //can safely check the word before
            Rules.DeterminerPreceedingRule(wBefore, posL, w, s);
            if (U.EndsWith(w, "ing") && posL.Contains("verb")) { Rules.IngVerbRule(wBefore, posL, w); }
        }

        private static void RunMiddleWordRules(Word wBefore, Word wAfter, List<string> posL, Word w, Sentence s)
        {
            //can safely check the word before AND the word after
            Rules.DeterminerPreceedingRule(wBefore, wAfter, posL, w, s);
            if (U.EndsWith(w, "ing") && posL.Contains("verb")) { Rules.IngVerbRule(wBefore, posL, w); }

        }

        private static void RunUnknownMiddleRules(Word wBefore, Word wAfter, Word w, Sentence s)
        {
            //Rules.UnknownMiddleRules(wBefore, wAfter, w, s);
        }

        private static void RunUnknownLastRules(Word wBefore, Word w, Sentence s)
        {
            //Rules.UnknownLastRules(wBefore, w, s);
        }
     
        public static string GetAbbrev(Word w)
        {
            string abbr = "";
            switch (w.pos)
            {
                case "noun":
                    if (w.role == "object of the preposition") { abbr = "OP"; }
                    else if (w.role == "predicate nominative") { abbr = "PrN"; }
                    else if (w.role == "subject") { abbr = "SN"; }
                    else if (w.role == "adjective") { abbr = "Adj"; }
                    else { abbr = "N"; }
                    break;
                case "proper noun":
                    if (w.role == "object of the preposition") { abbr = "OP"; }
                    else if (w.role == "predicate nominative") { abbr = "PrN"; }
                    else if (w.role == "subject") { abbr = "SN"; }
                    else if (w.role == "adjective") { abbr = "Adj"; }
                    else { abbr = "N"; }
                    break;
                case "verb":
                    abbr = "V";
                    break;
                case "adjective":
                    if (w.role == "predicate adjective") { abbr = "PA"; }
                    else { abbr = "Adj"; }
                    break;
                case "adverb":
                    abbr = "Adv";
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
                    if (w.role == "subject") { abbr = "SP"; }
                    else { abbr = "Pro"; }
                    break;
                case "conjunction":
                    abbr = "C";
                    break;
                case "preposition":
                    abbr = "P";
                    break;
                case "relative pronoun":
                    abbr = "RP";
                    break;
                case "determiner":
                    abbr = "DT";
                    break;
                case "unknown":
                    abbr = "?";
                    break;
                case "interjection":
                    abbr = "I";
                    break;
                case "coordinating conjunction":
                    abbr = "CC";
                    break;
                case "possessive determiner":
                    abbr = "DT";
                    break;
                default:
                    abbr = "ERR";
                    break;
            }
            return abbr;
        }
    }
}
