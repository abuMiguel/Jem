using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jem1;
using System.Text.RegularExpressions;

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
                    case '.':
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
                    case '.':
                        return true;
                    default: break;
                }
            }
            return false;
        }

        public static string At(Dictionary<int, char> sc, Word w)
        {
            // Word starting with @ is usually a social media username
            if (sc[0] == '@') { w.Role = "username";  return "proper noun"; }

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

        public static string URL(Dictionary<int,char> sc, Word w)
        {
            if (w.Name[w.Name.Length - 1] != '.')
            {
                var regex = new Regex(@"^((ht|f)tp(s?)\:\/\/)?[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$");
                if (regex.IsMatch(w.Name)) { w.Role = "URL"; return "particle"; }
            }
            return "unknown";
        }

    }
}
