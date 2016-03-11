using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jem1;
using static jem1.Structure.JO;
using jem1.Grammar;
using Newtonsoft.Json.Linq;
using jem1.Structure;

namespace jem1
{
    static class Answer
    {
        public static string Find(Session sess)
        {
            var ls = sess.stm.Last();
            if (ls.question)
            {
                Question q = new Question(ls);
                return q.answer;
            }
            else { return Statement(ls); }
        }

        public static string Statement(Sentence ls)
        {
            //Check if they are saying goodbye     
            var gbye = JO.GetValsList("goodbye", "synonyms");
            var adios = gbye.Find(x => x == ls.wordsString);
            if (!string.IsNullOrEmpty(adios))
            {
                return "exit";
            }
            else
            {
                return "I have no response to that statement.";
            }

        }

    }
}
