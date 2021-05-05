using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jem1
{
    public static class Intent
    {
        public static Dictionary<string, string> intents = new Dictionary<string, string>()
        {
            { @"^(what) (is|are)", "define"},
            { @"^(who) (is|are)", "identify"},
            { @"^(can|could) ", "possibility" }
        };

        public static string GetIntent(string sentence)
        {
            string intentValue = "define";
            foreach (KeyValuePair<string, string> intent in intents)
            {
                var match = Regex.IsMatch(sentence, intent.Key);
                if (match)
                {
                    intentValue = intent.Value;
                    break;
                }
            }
            return intentValue;
        }
        public static string GenerateResponse(string intent, Sentence s)
        {
            if (intent == "define")
            {
                //s.Clauses[0].Subjects[0].
            }
            return "";
        }

        public static List<IMeaning> meanings = new List<IMeaning>()
        {
            new Meaning()
            {
                Name = "Company",
                Is = new List<string>(){"business"},
                Definition = "Company Description",
                Synonyms = new List<string>(){}
            }
        };
    }
}
