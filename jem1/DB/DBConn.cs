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

        public static void InsertEng(string word, string pos)
        {
            int wordID = GetMaxWordID() + 1;
            //escape single quote in the word string
            if (word.Contains("'")) { word = word.Replace("'", "''"); }
            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = $"insert into eng (wordID, word, pos) Values({wordID},'{word}','{pos}');";

            using (SQLiteConnection conn = new SQLiteConnection(dataSource))
            {
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
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

        public static void DeleteMWE()
        {
            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = "delete from mwe;";

            using (SQLiteConnection conn = new SQLiteConnection(dataSource))
            {
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                    Console.WriteLine("Delete from mwe table was successful.");
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Delete from mwe table was unsuccessful.");
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static int GetMaxWordID()
        {
            int wordID = 0;
            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = "SELECT wordID FROM eng ORDER BY wordID DESC LIMIT 1";

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
                        wordID = reader.GetInt32(reader.GetOrdinal("wordID"));
                    }
                    conn.Close();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Get Max WordID from eng table was unsuccessful.");
                    Console.WriteLine(e.Message);
                }
            }
            return wordID;
        }

        public static int GetMaxMWEID()
        {
            int mweID = 0;
            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = "SELECT mweID FROM mwe ORDER BY mweID DESC LIMIT 1";

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
                        mweID = reader.GetInt32(reader.GetOrdinal("mweID"));
                    }
                    conn.Close();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Get Max MWEID from mwe table was unsuccessful.");
                    Console.WriteLine(e.Message);
                }
            }
            return mweID;
        }

        public static string GetPOS(string word)
        {
            string pos = string.Empty;

            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = $"select pos from eng where word = '{word}' COLLATE NOCASE;";

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
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("GET POS eng table was unsuccessful.");
                    Console.WriteLine(e.Message);
                }
            }

            return pos;
        }

        public static int GetWordID(string word)
        {
            int wordID = 0;

            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = $"select wordID from eng where word = '{word}' COLLATE NOCASE;";

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
                        wordID = reader.GetInt32(reader.GetOrdinal("wordID"));
                    }
                    conn.Close();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Get WordID from eng table was unsuccessful.");
                    Console.WriteLine(e.Message);
                }
            }
            return wordID;
        }

        public static void InsertMWE(int word1ID, int word2ID, string text, string pos, int meaningID)
        {
            int mweID = GetMaxMWEID() + 1;
            //escape single quote in the word string
            if (text.Contains("'")) { text = text.Replace("'", "''"); }
            var dbPath = ConfigurationManager.AppSettings["DBPath"];
            var dataSource = "Data Source=" + dbPath + ";Version=3;";

            string sql = $"insert into mwe (mweID, word1ID, word2ID, text, pos, meaningID) Values({mweID},{word1ID},{word2ID},'{text}','{pos}',{meaningID});";

            using (SQLiteConnection conn = new SQLiteConnection(dataSource))
            {
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                    Console.WriteLine("Insert into MWE table was successful.");
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine($"Insert into MWE table was unsuccessful for MWE: {text}, ID: {mweID}.");
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static void MigrateDataFromFileToEngTable()
        {
            var dirPath = ConfigurationManager.AppSettings["FilePath"];
            DirectoryInfo root = new DirectoryInfo(dirPath);
            var subDirs = root.GetDirectories();

            DeleteEng();

            foreach (DirectoryInfo dir in subDirs)
            {
                var files = dir.GetFiles("*.*");
                foreach(FileInfo file in files)
                {
                    var json = File.ReadAllText(file.FullName);
                    var jo = JObject.Parse(json);

                    var word = GetFirst(jo);
                    var pos = GetVal(jo, "pos");

                    InsertEng(word, pos);
                }
            }
        }

        public static void MigrateDataFromFileToMWETable()
        {
            var dirPath = ConfigurationManager.AppSettings["MWEFilePath"];
            DirectoryInfo root = new DirectoryInfo(dirPath);
            var subDirs = root.GetDirectories();

            DeleteMWE();

            foreach (DirectoryInfo dir in subDirs)
            {
                var subDirs2 = dir.GetDirectories();

                foreach (DirectoryInfo dir2 in subDirs2)
                {
                    var files = dir2.GetFiles("*.*");

                    foreach (FileInfo file in files)
                    {
                        var json = File.ReadAllText(file.FullName);
                        var jo = JObject.Parse(json);
                        var word = GetFirst(jo);

                        var pos = GetVal(jo, "pos");
                        List<string> wordL = word.Split(' ').ToList();
                        int word1 = GetWordID(wordL[0]);
                        int word2 = GetWordID(wordL[1]);

                        InsertMWE(word1, word2, word, pos, 0);
                    }
                }
            }
        }

    }
}
