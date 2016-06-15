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
        public List<Word> words { get; set; }
        //json object list
        public List<JObject> jol { get; set; }
        public List<Word> subjects { get; set; }
        public List<Word> verbs { get; set; }
        public string hVerb { get; set; }
        public List<Word> predicate { get; set; }
        //predicate nominative/noun
        public List<Word> pN { get; set; }
        //predicate adjective
        public List<Word> pA { get; set; }
        public List<PrepPhrase> preps { get; set; }
        //important words
        public Word i1 { get; set; }
        public Word i2 { get; set; }
        public Word i3 { get; set; }

        public Clause(string c)
        {
            words = new List<Word>();
            subjects = new List<Word>();
            verbs = new List<Word>();
            jol = new List<JObject>();
            predicate = new List<Word>();
            preps = new List<PrepPhrase>();
            pN = new List<Word>();
            pA = new List<Word>();

            wordsString = c;
            wordsArray = c.Split(' ');

            PopulateWordsList();
        }

        public Clause(List<Word> c)
        {
            words = c;
            subjects = new List<Word>();
            verbs = new List<Word>();
            jol = new List<JObject>();
            predicate = new List<Word>();
            preps = new List<PrepPhrase>();
            pN = new List<Word>();
            pA = new List<Word>();

            wordsArray = new string[c.Count];
            for(int i = 0; i < c.Count; i++)
            {
                wordsArray[i] = c[i].name;
            }
            wordsString = string.Join(" ", wordsArray);

        }

        private void PopulateWordsList()
        {
            for (int i = 0; i < wordsArray.Length; i++)
            {
                words.Add(new Word(Punctuation.Strip(wordsArray[i]), i));   
            }     
        }

        public int WordCount()
        {
            return wordsArray.Length;
        }

        //UNFINISHED
        //is complete is incomplete (ironic?), but will be used to tell whether it is a complete independent thought
        public bool IsComplete()
        {
            if(i1 != null && i2 != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
