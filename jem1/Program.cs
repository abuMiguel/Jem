using System.Collections.Generic;
using static System.Console;
using jem1.Grammar;

namespace jem1
{
    class Program
    {
        static void Main(string[] args)
        {
            var exit = false;
            var usertext = "";
            Session sess = new Session();

            WriteLine("Jem: Input a sentence and I will tag it for you.");

            while (!exit)
            {
                Write("User: ");
                usertext = ReadLine();

                if (!string.IsNullOrEmpty(usertext))
                {
                    Sentence s = new Sentence(usertext);
                    sess.Stm.Add(s);

                    var intent = Intent.GetIntent(s.WordsString);
                    WriteLine(intent);
                }
            }
            //WriteLine("Jem: " + usertext);
            ReadKey();
        }

        public static void WritePosToConsole(Sentence s)
        {
            List<string> abbrs = new List<string>();
            var output = s.Words;

            foreach (Word w in s.Words)
            {
                abbrs.Add(POSTagger.GetAbbrev(w));
            }

            foreach (Word w in output)
            {
                //add punctuation back to word for display
                if (s.Punc.ContainsKey(w.Id) && s.Punc[w.Id] != "none")
                {
                    Write(w.Name + s.Punc[w.Id] + " (" + abbrs[w.Id] + ") ");
                }
                else
                {
                    Write(w.Name + " (" + abbrs[w.Id] + ") ");
                }
            }
            WriteLine("");
        }
    }
}
