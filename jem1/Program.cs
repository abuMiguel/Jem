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
using static jem1.DB.DBConn;
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
                    sess.stm.Add(s);

                    if (Answer.Find(sess) == "exit")
                    {
                        exit = true;
                    }
                    else
                    {   //show the parts of speech for given sentence
                        if (showPOS)
                        {
                            List<string> abbrs = new List<string>();
                            showPOS = false;
                            var output = s.words;

                            foreach (Word w in s.words)
                            {
                                abbrs.Add(POSTagger.GetAbbrev(w));
                            }

                            foreach(Word w in output)
                            {
                                //add punctuation back to word for display
                                if(s.punc.ContainsKey(w.ID) && s.punc[w.ID] != "none")
                                {
                                    Write(w.name + s.punc[w.ID] + " (" + abbrs[w.ID] + ") ");
                                }
                                else
                                {
                                    Write(w.name + " (" + abbrs[w.ID] + ") ");
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
