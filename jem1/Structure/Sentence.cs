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
    internal class Sentence
    {
        public List<string> WordsOriginal { get; set; }
        public string WordsString { get; set; }
        public List<Word> Words { get; set; } = new List<Word>();
        public Dictionary<int, string> Punc { get; set; } = new Dictionary<int, string>();
        public bool Question { get; set; } = false;
        public List<Clause> Clauses { get; set; }
        public int WordCount { get; set; }
        public bool ExpectCompleteThought { get; set; } = true;
        public bool IsCompleteThought { get; set; } = true;

        public Sentence(string sent)
        {
            sent = sent.Trim();
            WordsString = sent;
            WordsOriginal = sent.Split(' ').ToList<string>();
            WordCount = WordsOriginal.Count;

            PopulateWordsList();
            // Find Multiple Word Expressions
            FindMWEs();
            POSTagger.AssignPartsOfSpeech(this);
            Clauses = GetClauses(this);

            foreach (Clause clause in Clauses)
            {
                //JO.PopulateJsonObjectList(clause);
                POSTagger.TagClause(clause);
                if (!U.IsCompleteThought(clause))
                {
                    this.IsCompleteThought = false;
                }
            }

            if (ExpectCompleteThought && this.IsCompleteThought)
            {
                Rules.ProperSentenceCorrectionRule(this);
            }

        }

        private void PopulateWordsList()
        {
            var contractionCount = 0;

            // Use i for index of original List and j for new words List
            for (int i = 0, j = 0; i < WordCount - contractionCount; i++, j++)
            {
                var name = WordsOriginal[i];

                if (!string.IsNullOrEmpty(name))
                {
                    // Check if word is the last in the sentence or has a comma.
                    if (Punctuation.HasComma(WordsOriginal[i]) || i == WordCount - contractionCount - 1)
                    {
                        name = Punctuation.Strip(WordsOriginal[i], this, j);
                    }

                    Words.Add(new Word(name, j));

                    // Insert contraction word into list if word is a contraction
                    if (Words[j].Contraction)
                    {
                        // Change original word to its root by removing contraction ending
                        if (Words[j].Name == "won't")
                        {
                            Words[j].Name = "will";
                        }
                        else
                        {
                            var cL = Words[j].GetContractionEnding().Length;
                            Words[j].Name = Words[j].Name.Remove(Words[j].Name.Length - cL);
                        }

                        j++;
                        Words.Insert(j, new Word(Words[j - 1].ConWord, j));
                        WordCount++;
                        contractionCount++;
                    }

                    switch (Words[j].Name[Words[j].Name.Length - 1])
                    {
                        case ',':
                            Punc.Add(j, ",");
                            break;
                        case '.':
                            Punc.Add(j, ".");
                            break;
                        case '?':
                            Punc.Add(j, "?");
                            if (j == WordCount - 1) { Question = true; }
                            break;
                        case '!':
                            Punc.Add(j, "!");
                            break;
                    }
                }
            }
            if (Punc.Count == 0) { Punc.Add(0, "none"); }
        }

        //Find any Multiple Word Expression found in the sentence and tag the part of speech
        private void FindMWEs()
        {
            var filePath = ConfigurationManager.AppSettings["MWEFilePath"];
            var largest = "";
            var endId = 0;

            foreach (Word w in this.Words)
            {
                //Do not check last word, it cannot begin a MWE
                if (w.Id < this.Words.Count() - 1)
                {
                    var startId = w.Id;
                    var dir = w.Name;
                    var filename = w.Name;

                    foreach (Word w2 in this.Words)
                    {
                        if (w2.Id > w.Id)
                        {
                            filename += " " + w2.Name;
                            var jsonfile = filePath + w.Name[0].ToString() + @"\" + dir + @"\" + filename.Replace(" ", "") + ".json";

                            if (File.Exists(jsonfile))
                            {
                                largest = filename;
                                endId = w2.Id;
                            }
                        }
                    }

                    // If there is a MWE starting with this word
                    if (!string.IsNullOrEmpty(largest))
                    {
                        var posL = JO.GetValsList(largest, "pos");

                        for (int i = startId, j = 0; i <= endId; i++, j++)
                        {
                            try
                            {
                                this.Words[i].Pos = posL[j];
                            }
                            catch
                            {
                                this.Words[i].Pos = posL[0];
                            }
                        }
                    }

                    largest = "";

                }
            }
        }

        public string GetPuncAll()
        {
            var punctuation = Punc.Values.ToList();
            var sb = new StringBuilder();
            foreach (string p in punctuation)
            {
                sb.Append(p);
            }
            return sb.ToString();
        }

        private List<Clause> GetClauses(Sentence s)
        {
            var clauses = new List<Clause>();
            var dividers = GetClauseDividers(s);

            bool firstWordIsDivider = false;
            int previousDividerId = 0;

            foreach (Word div in dividers)
            {
                // Strings built to create a clause, redo is only for divider starting a sentence.
                var cIni = new List<Word>();
                var cIniFinal = new List<Word>();
                var cIniRedo = new List<Word>();

                // Divider is the first word in the sentence.
                if (div.Id == 0)
                {
                    firstWordIsDivider = true;
                    clauses.Add(new Clause(s.Words));
                    if (dividers.Count == 1)
                    {
                        // Divider is first word and there is only 1 divider.
                        //To Do: split clause using cIniRedo at appropriate place
                    }
                }
                else
                {
                    foreach (Word w in s.Words)
                    {
                        // If is first divider and divider is not the first word in the sentence.
                        if (div.Id == dividers[0].Id)
                        {
                            if (w.Id < div.Id)
                            {
                                cIni.Add(w);
                            }
                        }
                        else 
                        {
                            // Is NOT the first divider.
                            // The first word of the sentence was a divider and there are multiple dividers.
                            if (dividers.Count > 1 && div.Id == dividers[1].Id && firstWordIsDivider)
                            {
                                if (w.Id >= div.Id)
                                {
                                    if (dividers.Count > 2)
                                    {
                                        if (w.Id < dividers[2].Id)
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
                                if (w.Id < div.Id && w.Id >= previousDividerId && div != dividers[dividers.Count - 1])
                                {
                                    cIni.Add(w);
                                }
                            }
                        }
                        //If the divider is the last one, get the clause at the end
                        if (div == dividers[dividers.Count - 1])
                        {
                            if (w.Id >= div.Id)
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

                previousDividerId = div.Id;
            }

            // If there are no dividers, the entire sentence is one clause.
            if (dividers.Count == 0)
            {
                clauses.Add(new Clause(s.Words));
            }

            return clauses;
        }

        private List<Word> GetClauseDividers(Sentence s)
        {
            List<Word> divs = new List<Word>();

            foreach (Word w in s.Words)
            {
                switch (w.Pos)
                {
                    case "subordinating conjunction":
                        divs.Add(w);
                        break;
                    case "coordinating conjunction":
                        // Coordinating conjunctions don't always divide a sentence, but sometimes do.
                        bool beforeVerb = false;
                        bool afterVerb = false;

                        foreach (Word word in s.Words)
                        {
                            // Check for verbs in the same sentence.
                            if (word.Pos == "verb" || word.Pos == "linking verb" || word.Pos == "helper verb")
                            {
                                // Is the verb before or after the coordinating conjunction?
                                if (word.Id < w.Id) { beforeVerb = true; }
                                if (word.Id > w.Id) { afterVerb = true; }
                            }
                        }

                        // If there are verbs before and after AND it isn't just a compound verb then it is a clause divider.
                        if (afterVerb == true && beforeVerb == true &&
                            (U.NothingButBetween(w, "verb", new string[2] { "adverb", "verb" }, s) == false &&
                            U.NothingButBetweenForward(w, "verb", new string[4] { "adverb", "linking verb", "helper verb", "verb" }, s) == false))
                        {
                            divs.Add(w);
                        }
                        break;
                }
            }
            return divs;
        }

    }
}
