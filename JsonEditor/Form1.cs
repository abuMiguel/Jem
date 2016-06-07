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
        List<string> checkedItems = new List<string>();
        public Form1()
        {
            InitializeComponent();

            comboBox2.DisplayMember = "Text";
            comboBox2.ValueMember = "Value";

            //pos
            listView2.Items.Add(new ListViewItem("All"));
            listView2.Items.Add(new ListViewItem("verb"));
            listView2.Items.Add(new ListViewItem("noun"));
            listView2.Items.Add(new ListViewItem("adjective"));
            listView2.Items.Add(new ListViewItem("adverb"));
            listView2.Items.Add(new ListViewItem("preposition"));
            listView2.Items.Add(new ListViewItem("conjunction"));
            listView2.Items.Add(new ListViewItem("pronoun"));
            listView2.Items.Add(new ListViewItem("determiner"));
            listView2.Items.Add(new ListViewItem("proper noun"));
            listView2.Items.Add(new ListViewItem("linking verb"));
            listView2.Items.Add(new ListViewItem("helper verb"));
            listView2.Items.Add(new ListViewItem("subordinating conjunction"));
            listView2.Items.Add(new ListViewItem("coordinating conjunction"));
            listView2.Items.Add(new ListViewItem("possessive determiner"));
            listView2.Items.Add(new ListViewItem("interjection"));
            listView2.Items.Add(new ListViewItem("relative pronoun"));

            var items2 = new[] {
                new { Text = "Verb", Value = "verb" },
                new { Text = "Noun", Value = "noun" },
                new { Text = "Adjectives", Value = "adjective" },
                new { Text = "Adverb", Value = "adverb" },
                new { Text = "Preposition", Value = "preposition" },
                new { Text = "Pronoun", Value = "pronoun" },
                new { Text = "Determiner", Value = "determiner" },
                new { Text = "Subordinating Conjunction", Value = "subordinatingConjunction" },
                new { Text = "Interjection", Value = "interjection" },
                new { Text = "Gerund", Value = "gerund" }
                };

            //templates
            comboBox2.DataSource = items2;


        }

        private void listView2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            label4.Text = "";
            listView1.Items.Clear();
            if (e.NewValue == CheckState.Checked)
            {
                checkedItems.Add(listView2.Items[e.Index].Text);
            }
            else
            {
                checkedItems.Remove(listView2.Items[e.Index].Text);
            }

            if (checkedItems.Contains("All"))
            {
                AddAllItemsToListView();
            }
            else
            {
                AddMultipleItemsToListView(checkedItems, "pos");
            }

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            textBox2.Text = "";
            label4.Text = "";

            string filepath = ConfigurationManager.AppSettings["FilePath"];
            string json;

            var items = listView1.SelectedItems;

            for (int i = 0; i < items.Count; i++)
            {
                try
                {
                    json = File.ReadAllText(filepath + items[i].Text[0].ToString() + @"\" + items[i].Text + ".json");
                    textBox2.Text += items[i].Text;
                    textBox2.ReadOnly = true;
                    richTextBox1.Text += json + "\r\n";
                }
                catch
                {
                    label4.Text += "File Not Found";
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void AddSelectedItemToListView(string val, string attr)
        {
            int cnt = 0;
            var items = listView1.Items;
            List<string> errList = new List<string>();

            string filepath = ConfigurationManager.AppSettings["FilePath"];
            var d = new DirectoryInfo(filepath);

            foreach (DirectoryInfo dir in d.GetDirectories())
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    var json = File.ReadAllText(filepath + fi.Name[0].ToString() + @"\" + fi.Name);
                    try
                    {
                        var jo = JObject.Parse(json);

                        //special case listing verbs in order to not pick up adverbs
                        if (val == "verb")
                        {
                            var posList = CSVToList(GetVal(jo, attr));

                            foreach (string pos in posList)
                            {
                                if (posList.Contains("verb") || posList.Contains("linking verb") || posList.Contains("helper verb"))
                                {
                                    cnt++;
                                    items.Add(fi.Name.Remove(fi.Name.Length - 5));
                                    break;
                                }
                            }
                        }  //special case for nouns in order to not add pronouns
                        else if (val == "noun")
                        {
                            var posList = CSVToList(GetVal(jo, attr));

                            foreach (string pos in posList)
                            {
                                if (posList.Contains("noun") || posList.Contains("proper noun"))
                                {
                                    cnt++;
                                    items.Add(fi.Name.Remove(fi.Name.Length - 5));
                                    break;
                                }
                            }
                        }
                        else  //all other pos work best this way
                        {
                            var posCSV = GetVal(jo, attr);
                            if (posCSV.Contains(val))
                            {
                                cnt++;
                                items.Add(fi.Name.Remove(fi.Name.Length - 5));
                            }
                        }
                    }
                    catch
                    {
                        errList.Add(fi.Name);
                    }

                }
            }
            label1.Text = cnt.ToString();
        }

        //TO DO: Make this work
        private void AddMultipleItemsToListView(List<string> val, string attr)
        {
            int cnt = 0;
            var items = listView1.Items;
            List<string> errList = new List<string>();

            string filepath = ConfigurationManager.AppSettings["FilePath"];
            var d = new DirectoryInfo(filepath);

            foreach (DirectoryInfo dir in d.GetDirectories())
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    var json = File.ReadAllText(filepath + fi.Name[0].ToString() + @"\" + fi.Name);
                    try
                    {
                        var jo = JObject.Parse(json);
                        var posList = CSVToList(GetVal(jo, attr));
                        bool listIt = true;

                        foreach (var pos in val)
                        {
                            if(posList.Contains(pos))
                            {
                                listIt = true;
                            }
                            else { listIt = false; break; }
                            
                        }
                        if (listIt) { cnt++; items.Add(fi.Name.Remove(fi.Name.Length - 5)); }

                    }
                    catch
                    {
                        errList.Add(fi.Name);
                    }

                }
            }
            label1.Text = cnt.ToString();
        }

        private void AddAllItemsToListView()
        {
            string filepath = ConfigurationManager.AppSettings["FilePath"];
            List<string> wordList = new List<string>();
            var items = listView1.Items;
            int cnt = 0;

            var d = new DirectoryInfo(filepath);
            foreach (DirectoryInfo dir in d.GetDirectories())
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    cnt++;
                    items.Add(fi.Name.Remove(fi.Name.Length - 5));
                }
            }
            label1.Text = cnt.ToString();
        }

        //Get value for one specified property in JSON Object
        private static string GetVal(JObject jo, string prop)
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

        private static bool HasProp(JObject jo, string prop)
        {
            string name = GetFirst(jo);
            var lst = jo[name].Select(jp => ((JProperty)jp).Name).ToList();
            return lst.Contains(prop);
        }

        private static string GetFirst(JObject jo)
        {
            var prop = jo.Properties().First();
            return prop.Name;
        }

        private static List<string> CSVToList(string csv)
        {
            List<string> x = new List<string>();
            if (csv.Contains(","))
            {
                x = csv.Split(',').ToList();
                return x;
            }
            else
            {
                x.Add(csv);
                return x;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Search Word text box
        }

        //Search Button
        private void button1_Click(object sender, EventArgs e)
        {
            label4.Text = "";
            richTextBox1.Text = "";
            textBox2.Text = "";

            string filepath = ConfigurationManager.AppSettings["FilePath"];
            string json;

            try
            {
                json = File.ReadAllText(filepath + textBox1.Text[0].ToString() + @"\" + textBox1.Text + ".json");
                richTextBox1.Text = json + "\r\n";
                textBox2.Text = textBox1.Text;
                textBox2.ReadOnly = true;
            }
            catch
            {
                label4.Text += textBox1.Text + Environment.NewLine + "NOT" + Environment.NewLine + "FOUND";
            }

        }

        //Save Button
        private void button2_Click(object sender, EventArgs e)
        {
            label4.Text = "";
            textBox2.Text = textBox2.Text.Trim();

            string json;
            string filepath = ConfigurationManager.AppSettings["FilePath"] + textBox2.Text[0].ToString() + @"\" + textBox2.Text + ".json";
            if (File.Exists(filepath) || textBox2.ReadOnly == false)
            {
                json = richTextBox1.Text;
                if (json.Contains(textBox2.Text))
                {
                    try
                    {
                        File.WriteAllText(filepath, json);
                        label4.Text += textBox2.Text + Environment.NewLine + "save" + Environment.NewLine + "successful";
                        textBox2.ReadOnly = true;
                    }
                    catch
                    {
                        label4.Text += textBox2.Text + Environment.NewLine + "could not" + Environment.NewLine + "be saved";
                    }
                }
                else { label4.Text += textBox2.Text + Environment.NewLine + "is not" + Environment.NewLine + "a file"; }
            }
            else { label4.Text += textBox2.Text + Environment.NewLine + "NOT" + Environment.NewLine + "FOUND"; }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label4.Text = "";
            richTextBox1.Text = "";
            textBox2.Text = "";

            string filepath = ConfigurationManager.AppSettings["FilePathTemplates"];
            string json;

            try
            {
                json = File.ReadAllText(filepath + comboBox2.SelectedValue.ToString() + ".json");
                richTextBox1.Text = json;
                textBox2.Text = comboBox2.SelectedValue.ToString();
                textBox2.ReadOnly = false;
            }
            catch
            {
                label4.Text += comboBox2.SelectedValue.ToString() + Environment.NewLine + "NOT" + Environment.NewLine + "FOUND";
            }
        }


    }
}
