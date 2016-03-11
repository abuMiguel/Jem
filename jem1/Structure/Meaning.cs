using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1.Structure
{
    class Meaning
    {
        public List<Word> words { get; set; }
        public string meaning { get; set; }
        public Meaning(List<Word> w)
        {
            words = new List<Word>();
            words = w;
            meaning = "";
        }

        public Meaning(Word w)
        {
            words = new List<Word>();
            words.Add(w);
            meaning = "";
        }
    }
}
