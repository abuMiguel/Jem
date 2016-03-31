using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using jem1.Structure;
using static jem1.Structure.JO;
using static System.Console;
using jem1.Grammar;

namespace jem1
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;
            bool showPOS = false;
            string usertext = "";
            Session sess = new Session();
            WriteLine("Jem: Hi, what can I do for you?");
            
            while (!exit)
            {
                Write("User: ");
                usertext = ReadLine();
                
                if (!exit && !string.IsNullOrEmpty(usertext))
                {   
                    //get first 4 chars to see if user wants POS
                    string f4 = new string(usertext.Take(4).ToArray());
                    if (f4 == "pos:")
                    {
                        showPOS = true;
                        usertext = usertext.Remove(0, 4);
                        usertext = usertext.TrimStart();
                    }

                    Sentence s = new Sentence(usertext);
                    sess.stm.Add(s);

                    if (Answer.Find(sess) == "exit")
                    {
                        exit = true;
                    }
                    else
                    {   //if user typed pos:, then show the parts of speech for given sentence
                        if (showPOS)
                        {
                            showPOS = false;
                            var wordsOutput = s.wordsArray;
                            foreach (Word w in s.words)
                            {
                                var abb = POSTagger.GetAbbrev(w);
                                var wLen = w.name.Length;
                                //if there is punctuation on a word then increase its length
                                if (s.punc.ContainsKey(w.ID) && s.punc[w.ID] != "none" ) { wLen++; }
                                //force the word and POS Tag to be the same length to make it look nice
                                while (wLen > abb.Length) { abb = abb + " "; }
                                while (abb.Length > wLen) { wordsOutput[w.ID] = wordsOutput[w.ID] + " "; wLen++; }

                                Write(abb + " ");
                            }
                            WriteLine("");
                            WriteLine(string.Join(" ", wordsOutput));
                        }
                        WriteLine("Jem: " + Answer.Find(sess));
                    }                   
                }
            }
            WriteLine("Jem: " + usertext);
            ReadKey();
        }

        
    }
}
