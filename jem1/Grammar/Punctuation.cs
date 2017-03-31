using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1.Grammar
{
    internal static class Punctuation
    {
        public static string Strip(string s, Sentence sent, int i)
        {
            string p, sw;

            switch (s.Last<char>())
            {
                case ',':
                case '!':
                case '?':
                case ':':
                case ';':
                    sw = s.TrimEnd(new char[6] { ',', '.', '!', ':', ';', '?' });
                    p = s.Remove(0, sw.Length);
                    sent.Punc.Add(i, p);
                    return sw;
                case '.':
                    sw = s.TrimEnd(new char[6] { ',', '.', '!', ':', ';', '?' });
                    sw = sw.Replace(".", "");
                    p = s.Remove(0, sw.Length);
                    sent.Punc.Add(i, p);
                    return sw;
                default:
                    return s;
            }

        }

        public static string GetPunc(string s)
        {
            var p = string.Empty;

            switch (s.Last<char>())
            {
                case ',':
                case '.':
                case '!':
                case '?':
                case ':':
                case ';':
                    var sw = s.TrimEnd(new char[] { ',', '.', '!', ':', ';', '?' });
                    p = s.Remove(0, sw.Length);
                    return sw;
                default:
                    return null;
            }
        }

        public static bool HasPunc(string s)
        {
            switch (s.Last<char>())
            {
                case ',':
                case '.':
                case '!':
                case '?':
                case ':':
                case ';':
                    return true;
                default:
                    return false;
            }
        }

        public static bool HasComma(string s)
        {
            if (s.Last<char>() == ',')
            {
                return true;
            }
            else { return false; }
        }
    }
}
