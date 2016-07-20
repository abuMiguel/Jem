using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jem1;

namespace jem1.Grammar
{
    public static class SpecChars
    {
        public static Dictionary<int, char> GetSpecialCharacters(string s)
        {
            Dictionary<int, char> sc = new Dictionary<int, char>();

            for (int i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case '@':
                    case '#':
                    case '$':
                    case '(':
                    case ')':
                    case '<':
                    case '>':
                    case '/':
                    case '\\':
                    case '+':
                    case '-':
                    case '&':
                    case '*':
                    case '=':
                    case '~':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                    case '_':
                    case '^':
                        sc.Add(i, s[i]);
                        break;
                    default: break;
                }
            }
            return sc;
        }

        public static bool ContainsSpecChar(string s)
        {
            foreach (char c in s)
            {
                switch (c)
                {
                    case '@':
                    case '#':
                    case '$':
                    case '(':
                    case ')':
                    case '<':
                    case '>':
                    case '/':
                    case '\\':
                    case '+':
                    case '-':
                    case '&':
                    case '*':
                    case '=':
                    case '~':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                    case '_':
                    case '^':
                        return true;
                    default: return false;
                }
            }
            return false;
        }

        public static string At(Dictionary<int, char> sc, string w)
        {
            //word starting with @ is usually a social media username
            if (sc[0] == '@') { return "proper noun"; }

            return "unknown";
        }

        public static string HashTag(Dictionary<int, char> sc, Word w)
        {
            //Need to look up word without hashtag, possibly split word into MWE or sentence


            return "unknown";
        }

        public static string Ampersand(Dictionary<int, char> sc, string w)
        {
            if(w.Length == 1) { return "coordinating conjunction"; }

            return "unknown";
        }

        public static string Equals(Dictionary<int, char> sc, string w)
        {
            if(w.Length == 1) { return "linking verb"; }

            return "unknown";
        }

    }
}
