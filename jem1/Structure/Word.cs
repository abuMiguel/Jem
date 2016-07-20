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
        public string pos { get; set; }
        public bool contraction { get; set; }
        public string conWord { get; set;  }
        public string role { get; set; }
        public bool inRelPhrase { get; set; }
        public bool inPrepPhrase { get; set; }
        public bool inInfPhrase { get; set; }
        public bool isPlural { get; set; }
        public string possessiveTag { get; set; }
        public int ID { get; set; }
        public List<string> descriptor { get; set; }

        public Word(string name, int ID)
        {
            this.name = name;
            this.ID = ID;
            this.contraction = false;
            this.conWord = null;
            if (name.Contains("'"))
            {
                if(IsContraction())
                {
                    this.contraction = true;
                    this.conWord = GetConEndWord(GetContractionEnding());
                }
            }

            this.pos = null;
            this.role = null;
            this.inPrepPhrase = false;
            this.inRelPhrase = false;
            this.isPlural = false;
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
            
            //get rid of accidental double '
            w = Regex.Replace(w, "'+", "'");
            //word has to have at least 3 letters to have possessive form
            if (w.Length > 2)
            {
                var z = w[w.Length - 1];
                var y = w[w.Length - 2];

                // possessives ending in 's, or '
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
            string[] sCon = new string[11]{"it's", "he's", "she's", "that's", "who's", "what's", "where's", "when's", "why's", "how's", "here's"};
            switch (GetContractionEnding())
            {
                case "n't": return true; 
                case "'ll": return true; 
                case "'ve": return true; 
                case "'d": return true; 
                case "'re": return true; 
                case "'m": return true; 
                case "'s":
                    if (sCon.Contains(name.ToLower())) { return true; }
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

        //return contraction's second word given a contraction ending
        private string GetConEndWord(string end)
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
