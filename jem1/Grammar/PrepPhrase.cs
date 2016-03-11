using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1.Grammar
{
    class PrepPhrase
    {
        List<Word> words { get; set; }
        string prep { get; set; }
        string objPrep { get; set; }
        int count { get; set; }
        //relationship
        string rel { get; set; }

        public PrepPhrase(List<Word> w)
        {
            words = w;
            prep = w[0].name;
            objPrep = w[w.Count - 1].name;
            count = w.Count;
            rel = FindRelationship();
        }

        private string FindRelationship()
        {

            return "";
        }
    }
}
