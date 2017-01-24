using jem1.Grammar;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1.Structure
{
    class Clause
    {
        public string[] wordsArray { get; set; }
        public string wordsString { get; set; }
        public List<Word> words { get; set; } = new List<Word>();
        public int wordCount { get; set; }
        //json object list
        public List<JObject> jol { get; set; } = new List<JObject>();
        public List<Word> subjects { get; set; } = new List<Word>();
        public List<Word> verbs { get; set; } = new List<Word>();
        public string hVerb { get; set; }
        public List<Word> predicate { get; set; } = new List<Word>();
        // Predicate nominative/noun
        public List<Word> pN { get; set; } = new List<Word>();
        // Predicate adjective
        public List<Word> pA { get; set; } = new List<Word>();
        public List<PrepPhrase> preps { get; set; } = new List<PrepPhrase>();
        // Important words - Idea was that nearly every sentence can be deduced to 3 words, where 
        // all other words only modify the Noun, Verb, and Predicate Nominative. 
        public Word i1 { get; set; }
        public Word i2 { get; set; }
        public Word i3 { get; set; }

        public Clause(string c)
        {
            wordsString = c;
            wordsArray = c.Split(' ');
            wordCount = wordsArray.Length; 

            PopulateWordsList();
        }

        public Clause(List<Word> c)
        {
            words = c;

            wordsArray = new string[c.Count];
            for(int i = 0; i < c.Count; i++)
            {
                wordsArray[i] = c[i].name;
            }
            wordsString = string.Join(" ", wordsArray);
            wordCount = wordsArray.Length;

        }

        private void PopulateWordsList()
        {
            for (int i = 0; i < wordCount; i++)
            {
                words.Add(new Word(wordsArray[i], i));   
            }     
        }
    }
}
