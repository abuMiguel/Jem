using jem1.Grammar;
using jem1.Structure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static jem1.Structure.JO;

namespace jem1
{
    class Sentence
    {
        public string[] wordsArray { get; set; }
        public string wordsString { get; set; }
        public List<Word> words { get; set; }
        public Dictionary<int, string> punc { get; set; }
        public bool question { get; set; }
        public List<Meaning> meanings { get; set; }
        public List<Clause> clauses { get; set; }

        public Sentence(string sent)
        {
            punc = new Dictionary<int, string>();
            words = new List<Word>();
            meanings = new List<Meaning>();
            wordsString = sent;
            wordsArray = sent.Split(' ');
            question = false;

            PopulateWordsList();
            POSTagger.AssignPartsOfSpeech(this);
            clauses = GetClauses(this);

            foreach (Clause clause in clauses)
            {
                JO.PopulateJsonObjectList(clause);
                POSTagger.TagClause(clause);
            }
        }

        private void PopulateWordsList()
        {
            for (int i = 0; i < wordsArray.Length; i++)
            {
                string name = Punctuation.Strip(wordsArray[i]);
                int id = i;

                if (!string.IsNullOrEmpty(name))
                {
                    words.Add(new Word(name, id));

                    switch (wordsArray[i][wordsArray[i].Length - 1])
                    {
                        case ',':
                            punc.Add(i, ",");
                            break;
                        case '.':
                            punc.Add(i, ".");
                            break;
                        case '?':
                            punc.Add(i, "?");
                            if (i == wordsArray.Length - 1) { question = true; }
                            break;
                        case '!':
                            punc.Add(i, "!");
                            break;
                    }
                }
            }
            if (punc.Count == 0) { punc.Add(0, "none"); }
        }


        public int WordCount()
        {
            return wordsArray.Length;
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
                        foreach(Word word in s.words)
                        {
                            //check for verbs
                            if(word.pos == "verb" || word.pos == "linking verb" || word.pos == "helper verb")
                            {
                                //is the verb before or after the CC
                                if(word.ID < w.ID) { bVerb = true; }
                                if(word.ID > w.ID) { aVerb = true; }
                            }
                        }
                        //if there are verbs before and after AND it isn't just a compound verb then it is a divider
                        if(aVerb == true && bVerb == true && 
                            (U.NothingButBetween(w, "verb", new string[2]{"adverb", "verb"}, s) == false && 
                            U.NothingButBetweenForward(w, "verb", new string[4]{"adverb", "linking verb", "helper verb", "verb"}, s) == false) )
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
                    if(divs.Count == 1)
                    {
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
