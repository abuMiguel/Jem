using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jem1.DB;
using System.Configuration;

namespace MWEInsert
{
    class Program
    {
        static void Main(string[] args)
        {
            var exit = false;
            string w1 = string.Empty;
            string w2 = string.Empty;
            string w1pos = string.Empty;
            string w2pos = string.Empty;

            while (!exit)
            {
                Console.Write("word 1: ");
                w1 = Console.ReadLine();

                Console.Write("word 1 pos: ");
                w1pos = Console.ReadLine();

                Console.Write("word 2: ");
                w2 = Console.ReadLine();

                Console.Write("word 2 pos: ");
                w2pos = Console.ReadLine();

                Console.Write("Write to DB?");
                var answer = Console.ReadLine();
                if (answer.ToLower() == "y" || answer.ToLower() == "yes")
                {
                    var path = ConfigurationManager.AppSettings["DBPath"];
                    DBConn.DbPath = path;
                    var w1id = DBConn.GetWordID(w1);
                    var w2id = DBConn.GetWordID(w2);
                    
                    DBConn.InsertMWE(w1id, w2id, w1 + " " + w2, w1pos + "," + w2pos, 0);
                }
            }
        }
    }
}
