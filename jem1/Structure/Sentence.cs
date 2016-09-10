using jem1.Grammar;
using jem1.Structure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static jem1.Structure.JO;

namespace jem1
{
    class Sentence
    {
        public List<string> wordsOriginal { get; set; }
        public string wordsString { get; set; }
        public List<Word> words { get; set; } = new List<Word>();
        public Dictionary<int, string> punc { get; set; } = new Dictionary<int, string>();
        public bool question { get; set; } = false;
        public List<Clause> clauses { get; set; }
        public int wordCount { get; set; }

        public Sentence(string sent)
        {
            sent = sent.Trim();
            wordsString = sent;
            wordsOriginal = sent.Split(' ').ToList<string>();
            wordCount = wordsOriginal.Count;

            PopulateWordsList();
            //MWE is Multiple Word Expression
            FindMWEs();
            POSTagger.AssignPartsOfSpeech(this);
            clauses = GetClauses(this);

            foreach (Clause clause in clauses)
            {
                //JO.PopulateJsonObjectList(clause);
                POSTagger.TagClause(clause);
            }
        }

        private void PopulateWordsList()
        {
            int numCon = 0;  //number of contractions
            string name = string.Empty;

            //use i for index of original List and j for new words List
            for (int i = 0, j = 0; i < wordCount - numCon; i++, j++)
            {
                name = wordsOriginal[i];

                if (!string.IsNullOrEmpty(name))
                {
                    //check if it's the last word in the sentence or a word with a comma
                    if (Punctuation.HasComma(wordsOriginal[i]) || i == wordCount - numCon - 1)
                    {
                        name = Punctuation.Strip(wordsOriginal[i], this, j);
                    }

                    words.Add(new Word(name, j));

                    //insert contraction word into list if word is a contraction
                    if (words[j].contraction)
                    {
                        //change original word to it's root by removing contraction ending
                        if (words[j].name == "won't")
                        {
                            words[j].name = "will";
                        }
                        else
                        {
                            var cL = words[j].GetContractionEnding().Length;
                            words[j].name = words[j].name.Remove(words[j].name.Length - cL);
                        }

                        j++;
                        words.Insert(j, new Word(words[j-1].conWord, j));
                        wordCount++;
                        numCon++;
                    }

                    switch (words[j].name[words[j].name.Length - 1])
                    {
                        case ',':
                            punc.Add(j, ",");
                            break;
                        case '.':
                            punc.Add(j, ".");
                            break;
                        case '?':
                            punc.Add(j, "?");
                            if (j == wordCount - 1) { question = true; }
                            break;
                        case '!':
                            punc.Add(j, "!");
                            break;
                    }
                }
            }
            if (punc.Count == 0) { punc.Add(0, "none"); }
        }

        //Find any Multiple Word Expression found in the sentence and tag the part of speech
        private void FindMWEs()
        {
            var filepath = ConfigurationManager.AppSettings["MWEFilePath"];
            string dir = "", filename = "";
            string jsonfile = "", largest = "";
            int sID = 0, eID = 0;

            foreach (Word w in this.words)
            {
                //Do not check last word, it cannot begin a MWE
                if (w.ID < this.words.Count() - 1)
                {
                    sID = w.ID;
                    dir = w.name;
                    filename = w.name;

                    foreach (Word w2 in this.words)
                    {
                        if (w2.ID > w.ID)
                        {
                            filename += " " + w2.name;
                            jsonfile = filepath + w.name[0].ToString() + @"\" + dir + @"\" + filename.Replace(" ", "") + ".json";

                            if (File.Exists(jsonfile))
                            {
                                largest = filename;
                                eID = w2.ID;
                            }
                        }
                    }

                    //if there is a MWE starting with this word
                    if (!string.IsNullOrEmpty(largest))
                    {
                        List<string> posL = new List<string>();
                        posL = JO.GetValsList(largest, "pos");

                        for (int i = sID, j = 0; i <= eID; i++, j++)
                        {
                            try
                            {
                                this.words[i].pos = posL[j];
                            }
                            catch
                            {
                                this.words[i].pos = posL[0];
                            }
                        }
                    }

                    largest = "";

                }
            }
        }

        public string GetPuncAll()
        {
            var punctuation = punc.Values.ToList();
            StringBuilder sb = new StringBuilder();
            foreach (string p in punctuation)
            {
                sb.Append(p);
            }
            return sb.ToString();
        }

        private List<Clause> GetClauses(Sentence s)
        {
            List<Clause> clauses = new List<Clause>();
            List<Word> divs = new List<Word>();

            foreach (Word w in s.words)
            {
                switch (w.pos)
                {
                    case "subordinating conjunction":
                        divs.Add(w);
                        break;
                    case "coordinating conjunction":
                        //coordinating conjunctions don't always divide a sentence, but sometimes do
                        bool bVerb = false; bool aVerb = false;
                        foreach (Word word in s.words)
                        {
                            //check for verbs
                            if (word.pos == "verb" || word.pos == "linking verb" || word.pos == "helper verb")
                            {
                                //is the verb before or after the CC
                                if (word.ID < w.ID) { bVerb = true; }
                                if (word.ID > w.ID) { aVerb = true; }
                            }
                        }
                        //if there are verbs before and after AND it isn't just a compound verb then it is a divider
                        if (aVerb == true && bVerb == true &&
                            (U.NothingButBetween(w, "verb", new string[2] { "adverb", "verb" }, s) == false &&
                            U.NothingButBetweenForward(w, "verb", new string[4] { "adverb", "linking verb", "helper verb", "verb" }, s) == false))
                        {
                            divs.Add(w);
                        }
                        break;
                }
            }

            bool firstWordIsDiv = false;
            // previous divider's ID
            int pDivID = 0;

            foreach (Word div in divs)
            {
                // strings built to create a clause, redo is only for divider starting a sentence
                List<Word> cIni = new List<Word>();
                List<Word> cIniFinal = new List<Word>();
                List<Word> cIniRedo = new List<Word>();

                //divider is the first word in the sentence
                if (div.ID == 0)
                {
                    firstWordIsDiv = true;
                    clauses.Add(new Clause(s.words));
                    if (divs.Count == 1)
                    {
                        //Div is first word and there is only 1
                        //To Do: split clause using cIniRedo at appropriate place
                    }
                }
                else
                {
                    foreach (Word w in s.words)
                    {
                        //if first divider and divider is not the first word in the sentence
                        if (div.ID == divs[0].ID)
                        {
                            if (w.ID < div.ID)
                            {
                                cIni.Add(w);
                            }
                        }
                        else //is NOT the first divider
                        {    //the first word of the sentence was a divider and there are multiple dividers
                            if (divs.Count > 1 && div.ID == divs[1].ID && firstWordIsDiv)
                            {
                                if (w.ID >= div.ID)
                                {
                                    if (divs.Count > 2)
                                    {
                                        if (w.ID < divs[2].ID)
                                        {
                                            cIni.Add(w);
                                        }
                                    }
                                    else { cIni.Add(w); }
                                }
                                else
                                {
                                    cIniRedo.Add(w);
                                }
                            }
                            else
                            {
                                if (w.ID < div.ID && w.ID >= pDivID && div != divs[divs.Count - 1])
                                {
                                    cIni.Add(w);
                                }
                            }
                        }
                        //If the divider is the last one, get the clause at the end
                        if (div == divs[divs.Count - 1])
                        {
                            if (w.ID >= div.ID)
                            {
                                cIniFinal.Add(w);
                            }
                        }
                    }
                    if (cIni.Count > 0)
                    {
                        clauses.Add(new Clause(cIni));
                    }
                    if (cIniFinal.Count > 0)
                    {
                        clauses.Add(new Clause(cIniFinal));
                    }
                    if (cIniRedo.Count > 0)
                    {
                        clauses[0] = new Clause(cIniRedo);
                    }
                }

                pDivID = div.ID;
            }

            if (divs.Count == 0)
            {
                clauses.Add(new Clause(s.words));
            }

            return clauses;
        }

    }
}
