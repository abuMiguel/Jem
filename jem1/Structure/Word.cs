using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace jem1
{
    public class Word
    {
        public string Name { get; set; }
        public string Pos { get; set; } = null;
        public bool Contraction { get; set; } = false;
        public string ConWord { get; set;  } = null;
        public string Role { get; set; } = null;
        public bool InRelPhrase { get; set; } = false;
        public bool InPrepPhrase { get; set; } = false;
        public bool InInfPhrase { get; set; } = false;
        public bool IsPlural { get; set; } = false;
        public string PossessiveTag { get; set; }
        public int Id { get; set; }
        public List<string> Descriptor { get; set; }

        public Word(string name, int id)
        {
            this.Name = name;
            this.Id = id;

            if (name.Contains("'"))
            {
                if(IsContraction())
                {
                    this.Contraction = true;
                    this.ConWord = GetContractionEndWord(GetContractionEnding());
                }
            }

            if(name.Contains("'") && this.Contraction == false)
            {
                this.PossessiveTag = GetPossessiveTag(name);
            }
            else if(name == "its")
            {
                this.PossessiveTag = "s";
            }
            else
            {
                this.PossessiveTag = null;
            }
            
            this.Descriptor = new List<string>();
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
            var sContraction = new string[]{"it's", "he's", "she's", "that's", "who's", "what's", "where's", "when's", "why's", "how's", "here's", "there's"};
            switch (GetContractionEnding())
            {
                case "n't": return true; 
                case "'ll": return true; 
                case "'ve": return true; 
                case "'d": return true; 
                case "'re": return true; 
                case "'m": return true; 
                case "'s":
                    if (sContraction.Contains(Name.ToLower())) { return true; }
                    break;
                default: return false;
            }
            return false;
        }

        public string GetContractionEnding()
        {
            var w = this.Name;
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
        private static string GetContractionEndWord(string end)
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
            possessive = !string.IsNullOrEmpty(PossessiveTag);

            // 's at the end
            if (possessive)
            {
                var pLen = PossessiveTag.Length;
                var starti = Name.Length - pLen;

                Name = Name.Remove(starti, pLen);
            }

        }
    }
}
