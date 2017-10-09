using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1.Grammar
{
    public class PrepPhrase
    {
        private List<Word> Words { get; set; }
        private string Prep { get; set; }
        private string ObjPrep { get; set; }
        private int Count { get; set; }
        //relationship
        private string Rel { get; set; }

        public PrepPhrase(List<Word> w)
        {
            Words = w;
            Prep = w[0].Name;
            ObjPrep = w[w.Count - 1].Name;
            Count = w.Count;
            Rel = FindRelationship();
        }

        private static string FindRelationship()
        {

            return "";
        }
    }
}
