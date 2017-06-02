using jem1.Structure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1.Grammar
{
    //This class needs a lot of reworking after change to JSON structure and to make it more organized.
    class Question
    {
        public string[] qwords = { "do", "did", "does", "what", "where", "when", "why", "how", "which", "who", "whose", "can", "will", "have", "has", "had", "are" };
        

        public Question(Sentence s)
        {
           
        }

    }
}
