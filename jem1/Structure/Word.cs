using jem1.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jem1
{
    public class Word
    {
        public string name { get; set; }
        public string pos { get; set; } = null;
        public bool contraction { get; set; } = false;
        public string conWord { get; set;  } = null;
        public string role { get; set; } = null;
        public bool inRelPhrase { get; set; } = false;
        public bool inPrepPhrase { get; set; } = false;
        public bool inInfPhrase { get; set; } = false;
        public bool isPlural { get; set; } = false;
        public string possessiveTag { get; set; }
        public int ID { get; set; }
        public List<string> descriptor { get; set; }

        public Word(string name, int ID)
        {
            this.name = name;
            this.ID = ID;

            if (name.Contains("'"))
            {
                if(IsContraction())
                {
                    this.contraction = true;
                    this.conWord = GetContractionEndWord(GetContractionEnding());
                }
            }

            if(name.Contains("'") && this.contraction == false)
            {
                this.possessiveTag = GetPossessiveTag(name);
            }
            else if(name == "its")
            {
                this.possessiveTag = "s";
            }
            else
            {
                this.possessiveTag = null;
            }
            
            this.descriptor = new List<string>();
        }

        private string GetPossessiveTag(string w)
        {
            string tag = null;
            
            // Get rid of accidental double '
            w = Regex.Replace(w, "'+", "'");

            // Word has to have at least 3 characters to have possessive form.
            if (w.Length > 2)
            {
                var z = w[w.Length - 1];
                var y = w[w.Length - 2];

                // Possessives ending in 's, or '
                if (z == 's' && y == '\'')
                {
                    tag = "'s";
                }
                else if (z == '\'')
                {
                    tag = "'";
                }
            }
            return tag;
        }

        private bool IsContraction()
        {
            string[] sContraction = new string[12]{"it's", "he's", "she's", "that's", "who's", "what's", "where's", "when's", "why's", "how's", "here's", "there's"};
            switch (GetContractionEnding())
            {
                case "n't": return true; 
                case "'ll": return true; 
                case "'ve": return true; 
                case "'d": return true; 
                case "'re": return true; 
                case "'m": return true; 
                case "'s":
                    if (sContraction.Contains(name.ToLower())) { return true; }
                    break;
                default: return false;
            }
            return false;
        }

        public string GetContractionEnding()
        {
            var w = this.name;
            var aPos = w.IndexOf("'");
            if(aPos < 0) { return ""; }
            //if only one letter after the '
            if(aPos == w.Length - 2)
            {
                switch(w.Substring(aPos))
                {
                    case "'t": if(w[aPos - 1] == 'n') { return "n't"; }
                        break;
                    case "'m": return "'m";
                    case "'s": return "'s";
                    case "'d": return "'d";
                    default: return "";
                }
            }
            //if 2 letters after the '
            if (aPos == w.Length - 3)
            {
                switch (w.Substring(aPos))
                {
                    case "'ll": return "'ll";
                    case "'ve": return "'ve";
                    case "'re": return "'re";
                    default: return "";
                }
            }
                return "";
        }

        // Return contraction's second word given a contraction ending.
        private string GetContractionEndWord(string end)
        {
            switch (end)
            {
                case "'s": return "is";
                case "n't": return "not";
                case "'ve": return "have";
                case "'m": return "am";
                case "'d": return "had";
                case "'ll": return "will";
                case "'re": return "are";
            }
            return "";
        }

        public void RemovePossessiveTag()
        {
            bool possessive = false;
            possessive = !string.IsNullOrEmpty(possessiveTag);

            // 's at the end
            if (possessive)
            {
                var pLen = possessiveTag.Length;
                var starti = name.Length - pLen;

                name = name.Remove(starti, pLen);
            }

        }
    }
}
