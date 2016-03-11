using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1.Grammar
{
    static class Determiner
    {
        public static string AorAn(string w)
        {
            string determiner = "a";
            string[] vowels = { "a", "e", "i", "o", "u" };
            foreach(string v in vowels)
            {
                if(w.StartsWith(v))
                {
                    determiner = "an";
                }
            }
            
            return determiner;
        }

        public static string AorAn(string w, string silent)
        {
            string determiner = "a";
            string[] vowels = { "a", "e", "i", "o", "u" };
            foreach (string v in vowels)
            {
                if (w.StartsWith(v) || (w.StartsWith(silent) && w[1].ToString() == v) )
                {
                    determiner = "an";
                }
            }

            return determiner;
        }
    }
}
