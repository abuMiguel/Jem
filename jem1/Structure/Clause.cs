using jem1.Grammar;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1.Structure
{
    public class Clause
    {
        public string[] WordsArray { get; set; }
        public string WordsString { get; set; }
        public List<Word> Words { get; set; } = new List<Word>();
        public int WordCount { get; set; }
        public List<Word> Subjects { get; set; } = new List<Word>();
        public List<Word> Verbs { get; set; } = new List<Word>();
        public string HVerb { get; set; }
        public List<Word> Predicate { get; set; } = new List<Word>();
        // Predicate nominative/noun
        public List<Word> PN { get; set; } = new List<Word>();
        // Predicate adjective
        public List<Word> PA { get; set; } = new List<Word>();
        public List<PrepPhrase> Preps { get; set; } = new List<PrepPhrase>();
        // Important words - Idea was that nearly every sentence can be deduced to 3 words, where 
        // all other words only modify the Noun, Verb, and Predicate Nominative. 
        public Word I1 { get; set; }
        public Word I2 { get; set; }
        public Word I3 { get; set; }

        public Clause(string c)
        {
            WordsString = c;
            WordsArray = c.Split(' ');
            WordCount = WordsArray.Length; 

            PopulateWordsList();
        }

        public Clause(List<Word> c)
        {
            Words = c;

            WordsArray = new string[c.Count];
            for(var i = 0; i < c.Count; i++)
            {
                WordsArray[i] = c[i].Name;
            }
            WordsString = string.Join(" ", WordsArray);
            WordCount = WordsArray.Length;

        }

        private void PopulateWordsList()
        {
            for (var i = 0; i < WordCount; i++)
            {
                Words.Add(new Word(WordsArray[i], i));   
            }     
        }
    }
}
