using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using Newtonsoft.Json.Linq;
using static jem1.Structure.JO;

namespace jem1.DB
{
    static class DBConn
    {
        public static void ReadWords()
        {
            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = "select * from eng order by wordID asc";

            using (SQLiteConnection conn = new SQLiteConnection(dataSource))
            {
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                conn.Open();
                command.ExecuteNonQuery();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    Console.WriteLine("WordID: " + reader["wordID"] + " \tWord: " + reader["word"]);
                conn.Close();
            }
            
        }

        public static void InsertEng(int wordID, string word, string pos, int badSpell, int? lemmaID)
        {
            //escape single quote in the word string
            if (word.Contains("'")) { word = word.Replace("'", "''"); }
            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = $"insert into eng (wordID, word, pos, badSpell, lemmaID) Values({wordID},'{word}','{pos}',{badSpell},{lemmaID});";

            using (SQLiteConnection conn = new SQLiteConnection(dataSource))
            {
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                    Console.WriteLine("Insert into eng table was successful.");
                }
                catch(SQLiteException e)
                {
                    Console.WriteLine($"Insert into eng table was unsuccessful for word: {word}, ID: {wordID}.");
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static void DeleteEng()
        {
            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = "delete from eng;";

            using (SQLiteConnection conn = new SQLiteConnection(dataSource))
            {
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                    Console.WriteLine("Delete from eng table was successful.");
                }
                catch(SQLiteException e)
                {
                    Console.WriteLine("Delete from eng table was unsuccessful.");
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static string GetPOS(string word)
        {
            string pos = string.Empty;

            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = $"select pos from eng where word = '{word}';";

            using (SQLiteConnection conn = new SQLiteConnection(dataSource))
            {
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        pos = reader["pos"].ToString();
                    }
                    conn.Close();
                    Console.WriteLine("Delete from eng table was successful.");
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Delete from eng table was unsuccessful.");
                    Console.WriteLine(e.Message);
                }
            }

            return pos;
        }

        public static void MigrateDataFromFileToEngTable()
        {
            
            var dirPath = ConfigurationManager.AppSettings["FilePath"];
            DirectoryInfo root = new DirectoryInfo(dirPath);
            var subDirs = root.GetDirectories();

            DeleteEng();
            int wordID = 1;
            foreach (DirectoryInfo dir in subDirs)
            {
                var files = dir.GetFiles("*.*");
                foreach(FileInfo file in files)
                {
                    var json = File.ReadAllText(file.FullName);
                    var jo = JObject.Parse(json);

                    var word = GetFirst(jo);
                    var pos = GetVal(jo, "pos");

                    InsertEng(wordID, word, pos, 0, 0);
                    wordID++;
                }
            }
        }



    }
}
