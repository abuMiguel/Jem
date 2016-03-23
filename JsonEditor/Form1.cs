using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.IO;

namespace JsonEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";

            var items = new[] {
                new { Text = "All", Value = "All" },
                new { Text = "Verbs", Value = "Verbs" },
                new { Text = "Nouns", Value = "Nouns" },
                new { Text = "Adjectives", Value = "Adjectives" },
                new { Text = "Adverbs", Value = "Adverbs" },
                new { Text = "Prepositions", Value = "Prepositions" },
                new { Text = "Conjunctions", Value = "Conjunctions" },
                new { Text = "Pronouns", Value = "Pronouns" },
                new { Text = "Determiners", Value = "Determiners" }
                };

            comboBox1.DataSource = items;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AddItemsToListView();
        }

        private void AddItemsToListView()
        {
            string filepath = ConfigurationManager.AppSettings["FilePath"];
            List<string> wordList = new List<string>();

            var d = new DirectoryInfo(filepath);
            foreach (DirectoryInfo dir in d.GetDirectories())
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    wordList.Add(fi.Name.Remove(fi.Name.Length - 5));
                }
            }

            var items = listView1.Items;
            foreach (var value in wordList)
            {
                items.Add(value);
            }
        }

        //Get value for one specified property in JSON Object
        public static string GetVal(JObject jo, string prop)
        {
            string name = GetFirst(jo);

            if (HasProp(jo, prop))
            {
                if (jo[name][prop].Type == JTokenType.Array)
                {
                    return string.Join(",", jo[name][prop]);
                }
                else { return (string)jo[name][prop]; }

            }
            else { return ""; }
        }

        public static bool HasProp(JObject jo, string prop)
        {
            string name = GetFirst(jo);
            var lst = jo[name].Select(jp => ((JProperty)jp).Name).ToList();
            return lst.Contains(prop);
        }

        public static string GetFirst(JObject jo)
        {
            var prop = jo.Properties().First();
            return prop.Name;
        }
    }
}
