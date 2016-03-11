using jem1.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jem1
{
    class Word
    {
        public string name { get; set; }
        public string pos { get; set; }
        public string role { get; set; }
        public bool inRelPhrase { get; set; }
        public bool inPrepPhrase { get; set; }
        public bool isPlural { get; set; }
        public string possessiveTag { get; set; }
        public int ID { get; set; }
        public List<string> descriptor { get; set; }

        public Word(string name, int ID)
        {
            this.name = name;
            this.ID = ID;
            this.pos = null;
            this.role = null;
            this.inPrepPhrase = false;
            this.inRelPhrase = false;
            this.isPlural = false;
            this.possessiveTag = name.Contains("'") ? GetPossessiveTag(name) : null;
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
