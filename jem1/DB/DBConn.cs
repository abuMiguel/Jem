using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using ImpromptuInterface.InvokeExt;
using jem1.Structure;
using Newtonsoft.Json.Linq;

namespace jem1.DB
{
    public static class DBConn
    {
        public static string DbPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string DataSource => "Data Source=" + DbPath + "jem.sqlite;Version=3;";

        public static void ReadWords()
        {
            var sql = "select * from eng order by wordID asc";

            using (var conn = new SQLiteConnection(DataSource))
            {
                var command = new SQLiteCommand(sql, conn);
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
            int wordId = GetMaxWordID() + 1;
            //escape single quote in the word string
            if (word.Contains("'")) { word = word.Replace("'", "''"); }

            string sql = $"insert into eng (wordID, word, pos) Values({wordId},'{word}','{pos}');";

            using (var conn = new SQLiteConnection(DataSource))
            {
                try
                {
                    var command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
                catch(SQLiteException e)
                {
                    Console.WriteLine($"Insert into eng table was unsuccessful for word: {word}, ID: {wordId}.");
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static void DeleteEng()
        {
            var sql = "delete from eng;";

            using (var conn = new SQLiteConnection(DataSource))
            {
                try
                {
                    var command = new SQLiteCommand(sql, conn);
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
            var sql = "delete from mwe;";

            using (var conn = new SQLiteConnection(DataSource))
            {
                try
                {
                    var command = new SQLiteCommand(sql, conn);
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
            int wordId = 0;
            var sql = "SELECT wordID FROM eng ORDER BY wordID DESC LIMIT 1";

            using (var conn = new SQLiteConnection(DataSource))
            {
                try
                {
                    var command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        wordId = reader.GetInt32(reader.GetOrdinal("wordID"));
                    }
                    conn.Close();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Get Max WordID from eng table was unsuccessful.");
                    Console.WriteLine(e.Message);
                }
            }
            return wordId;
        }

        public static int GetMaxMWEID()
        {
            int mweID = 0;

            var sql = "SELECT mweID FROM mwe ORDER BY mweID DESC LIMIT 1";

            using (var conn = new SQLiteConnection(DataSource))
            {
                try
                {
                    var command = new SQLiteCommand(sql, conn);
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
            var pos = string.Empty;
            string sql = $"select pos from eng where word = '{word}' COLLATE NOCASE;";

            using (var conn = new SQLiteConnection(DataSource))
            {
                try
                {
                    var command = new SQLiteCommand(sql, conn);
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
            int wordId = 0;
            string sql = $"select wordID from eng where word = '{word}' COLLATE NOCASE;";

            using (var conn = new SQLiteConnection(DataSource))
            {
                try
                {
                    var command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        wordId = reader.GetInt32(reader.GetOrdinal("wordID"));
                    }
                    conn.Close();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Get WordID from eng table was unsuccessful.");
                    Console.WriteLine(e.Message);
                }
            }
            return wordId;
        }

        public static List<MWE> GetMWEs(string w, string wa)
        {
            var mweList = new List<MWE>();
            var sql = "select * from mwe where mwe.word1ID = " +
                         $"(select wordId from eng where word = '{w}') " +
                         $"and mwe.word2ID = (select wordId from eng where word = '{wa}') " +
                         "COLLATE NOCASE;";

            using (var conn = new SQLiteConnection(DataSource))
            {
                try
                {
                    var command = new SQLiteCommand(sql, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var mwe = new MWE
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("mweID")),
                            W1Id = reader.GetInt32(reader.GetOrdinal("word1ID")),
                            W2Id = reader.GetInt32(reader.GetOrdinal("word2ID")),
                            Text = reader.GetString(reader.GetOrdinal("text")),
                            Pos = reader.GetString(reader.GetOrdinal("pos")),
                            MeaningId = reader.GetInt32(reader.GetOrdinal("meaningID"))
                        };
                        mweList.Add(mwe);
                    }
                    conn.Close();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Get matching MWE Id's from mwe table was unsuccessful.");
                    Console.WriteLine(e.Message);
                }
            }
            return mweList;
        }

        public static void InsertMWE(int word1ID, int word2ID, string text, string pos, int meaningID)
        {
            int mweID = GetMaxMWEID() + 1;
            //escape single quote in the word string
            if (text.Contains("'")) { text = text.Replace("'", "''"); }

            string sql = $"insert into mwe (mweID, word1ID, word2ID, text, pos, meaningID) Values({mweID},{word1ID},{word2ID},'{text}','{pos}',{meaningID});";

            using (var conn = new SQLiteConnection(DataSource))
            {
                try
                {
                    var command = new SQLiteCommand(sql, conn);
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
    }
}
