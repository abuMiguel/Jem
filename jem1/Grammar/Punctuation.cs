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
                if (!char.IsPunctuation(c) || c == '\'')
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
