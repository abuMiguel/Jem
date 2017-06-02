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
using static jem1.DB.DBConn;
using static System.Console;
using jem1.Grammar;

namespace jem1
{
    class Program
    {
        static void Main(string[] args)
        {
            var exit = false;
            var showPOS = false;
            string usertext = "";
            Session sess = new Session();

            WriteLine("Jem: Input a sentence and I will tag it for you.");

            while (!exit)
            {
                Write("User: ");
                usertext = ReadLine();

                if (!exit && !string.IsNullOrEmpty(usertext))
                {
                    //This makes Jem give you the parts of speech
                    showPOS = true;

                    Sentence s = new Sentence(usertext);
                    sess.Stm.Add(s);

                    if (s.Words[0].Name.ToLower() == "exit" && s.Words.Count == 1)
                    {
                        exit = true;
                    }
                    else
                    {   //show the parts of speech for given sentence
                        if (showPOS)
                        {
                            List<string> abbrs = new List<string>();
                            showPOS = false;
                            var output = s.Words;

                            foreach (Word w in s.Words)
                            {
                                abbrs.Add(POSTagger.GetAbbrev(w));
                            }

                            foreach(Word w in output)
                            {
                                //add punctuation back to word for display
                                if(s.Punc.ContainsKey(w.Id) && s.Punc[w.Id] != "none")
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
                        //WriteLine("Jem: " + Answer.Find(sess));
                    }
                }
            }
            WriteLine("Jem: " + usertext);
            ReadKey();
        }


    }
}
