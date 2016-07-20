using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1.Grammar
{
    static class Punctuation
    {
        public static string Strip(string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                switch(c)
                { 
                    case ',': 
                    case '.': 
                    case '!': 
                    case '?':
                    case ':':
                    case ';': 
                        break;
                    default: sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
