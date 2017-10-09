using jem1.Structure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jem1.Grammar
{
    public class Question
    {
        public static List<string> qwords = new List<string>
        {
            "do", "did", "does", "what", "where", "when", "why", "how", "which", "who", "whose",
            "can", "will", "have", "has", "had", "are", "is", "am", "was", "were", "should", "could", "would", "may",
            "might"
        };

        public Question(Sentence s)
        {

        }

        public static bool IsQuestion(Sentence s)
        {
            var isQuestion = false;
            var hasQuestionMark = HasQuestionMark(s);
            var startsWithQuestionWord = StartsWithQuestionWord(s);
            if (startsWithQuestionWord && hasQuestionMark)
            {
                isQuestion = true;
            }

            return isQuestion;
        }

        private static bool HasQuestionMark(Sentence s)
        {
            var len = s.Words.Count;
            var iLast = len - 1;
            bool hasQMark = false;

            if (s.Punc.ContainsKey(iLast))
            {
                var p = s.Punc[iLast];
                if (p == "?")
                {
                    hasQMark = true;
                }
            }
            return hasQMark;
        }

        private static bool StartsWithQuestionWord(Sentence s)
        {
            var startsWithQuestionWord = false;

            if (s.Words.Count < 1) { return false; }

            if (qwords.Contains(s.Words[0].Name.ToLower()))
            {
                startsWithQuestionWord = true;
            }
            return startsWithQuestionWord;
        }
    }
}
