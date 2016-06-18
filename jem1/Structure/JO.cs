﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;

namespace jem1.Structure
{
    //This class is to facilitate retrieval of JSON properties and values from a JSON Object using JSON.NET
    static class JO
    {
        //Get all properties in JSON Object
        public static string GetProps(JObject jo)
        {
            string name = JO.GetFirst(jo);
            var lst = jo[name].Select(jp => ((JProperty)jp).Name).ToList();
            return string.Join(",", lst);
        }

        //Get all values in JSON Object
        public static string GetVals(JObject jo)
        {
            string name = JO.GetFirst(jo);
            var lst = jo[name].Select(jp => ((JProperty)jp).Value).ToList();
            return string.Join(",", lst);
        }

        //Get value for one specified property in JSON Object
        public static string GetVal(JObject jo, string prop)
        {
            string name = JO.GetFirst(jo);

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

        //Get values from a flat 2D string array from JSON format
        public static string GetVal2D(JObject jo, string prop, string val)
        {
            string name = JO.GetFirst(jo);
            string answer = "";

            if (JO.HasProp(jo, prop))
            {
                string p = JO.GetVal(jo, prop);
                string[] hasArray = p.Split(',');
                for (int i = 0; i < hasArray.Length; i++)
                {
                    if (hasArray[i] == val)
                    {
                        answer = hasArray[i + 1].ToString();
                        break;
                    }
                }
            }
            return answer;
        }

        //Get value of one specified property in JSON Object using string name of JSON file
        public static string GetVal(string word, string prop)
        {
            JObject jo = GetJSONObject(word);
            string name = JO.GetFirst(jo);

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

        //Get Quantity of value given a JSON Object, property, and value
        //If value has no quantity return the value itself
        public static string GetValQtyFromAncestors(string word, string prop, string val)
        {
            string answer = "";
            List<string> anc = new List<string>();
            anc = JO.GetAncestors(word);
            foreach (string x in anc)
            {
                JObject jo = JO.GetJSONObject(x);
                if (JO.HasProp(jo, prop))
                {
                    string v = JO.GetVal(jo, prop);
                    string[] vArray = v.Split(',');
                    for (int i = 0; i < vArray.Length; i++)
                    {
                        if (vArray[i] == val || vArray.Length == 1)
                        {
                            answer = vArray.Length > 1 ? vArray[i + 1].ToString() : vArray[0].ToString();
                            if (vArray.Length > 1)
                            {
                                double num = 0;
                                bool result = double.TryParse(vArray[i + 1], out num);
                                answer = result ? vArray[i + 1] : "";
                            }
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(answer)) { break; }
            }
            return answer;
        }

        public static string GetValFromAncestors(string word, string prop, string val)
        {
            string answer = "";
            List<string> anc = new List<string>();
            anc = JO.GetAncestors(word);
            foreach (string x in anc)
            {
                JObject jo = JO.GetJSONObject(x);
                if (JO.HasProp(jo, prop))
                {
                    string v = JO.GetVal(jo, prop);
                    string[] vArray = v.Split(',');
                    for (int i = 0; i < vArray.Length; i++)
                    {
                        if (vArray[i] == val)
                        {
                            answer = vArray[i].ToString();
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(answer)) { break; }
            }
            return answer;
        }

        public static List<string> GetValsList(string word, string prop)
        {
            List<string> syn = new List<string>();
            string values = GetVal(word, prop);
            string[] valuesArray = values.Split(',');
            foreach (string val in valuesArray)
            {
                syn.Add(val);
            }

            return syn;
        }

        public static List<string> GetAncestors(string word)
        {
            List<string> anc = new List<string>();
            //don't allow an infinite loop
            int x = 0;
            while (!string.IsNullOrEmpty(GetVal(word, "is")) || x > 50)
            {
                string val = GetVal(word, "is");
                anc.Add(val);
                word = val;
                x++;
            }

            return anc;
        }


        public static bool HasProp(JObject jo, string prop)
        {
            string name = JO.GetFirst(jo);
            var lst = jo[name].Select(jp => ((JProperty)jp).Name).ToList();
            return lst.Contains(prop);
        }

        public static bool HasVal(JObject jo, string val)
        {
            string name = JO.GetFirst(jo);
            var lst = jo[name].Select(jp => ((JProperty)jp).Value).ToList();
            return lst.Contains(val);
        }

        public static string GetFirst(JObject jo)
        {
            var prop = jo.Properties().First();
            return prop.Name;
        }

        public static void PopulateJsonObjectList(Clause c)
        {
            JObject jo = new JObject();
            string json, jsonfile, filepath;
            filepath = ConfigurationManager.AppSettings["FilePath"];
            List<string> unknown = new List<string>();

            foreach (Word word in c.words)
            {
                jsonfile = !string.IsNullOrEmpty(word.name) ? filepath + word.name[0].ToString() + @"\" + word.name + ".json" : " ";

                if (File.Exists(jsonfile))
                {
                    json = File.ReadAllText(jsonfile);
                    jo = JObject.Parse(json);
                    c.jol.Add(jo);
                }
                else
                {
                    json = @"{ """ + word.name + @""": { ""pos"":""unknown"" } }";
                    jo = JObject.Parse(json);
                    c.jol.Add(jo);
                    unknown.Add(word.name);
                }
            }
        }

        //returns single json object for given string
        public static JObject GetJSONObject(string w)
        {
            w = w.Trim();
            Word word = new Word(w, 0);
            JObject jo = new JObject();
            string json, jsonfile, filepath;
            bool MWE = w.Any(x => Char.IsWhiteSpace(x));

            if (MWE)
            {
                filepath = ConfigurationManager.AppSettings["MWEFilePath"];
            }
            else { filepath = ConfigurationManager.AppSettings["FilePath"]; }

            if (!string.IsNullOrEmpty(word.possessiveTag))
            {
                word.RemovePossessiveTag();
            }

            if (MWE)
            {
                int si = word.name.IndexOf(' ');
                jsonfile = filepath + word.name[0].ToString() + @"\" + word.name.Substring(0, si) + @"\" + word.name.Replace(" ", "") + ".json";
            }
            else
            {
                jsonfile = filepath + word.name[0].ToString() + @"\" + word.name + ".json";
            }

            if (File.Exists(jsonfile))
            {
                json = File.ReadAllText(jsonfile);
                jo = JObject.Parse(json);
            }
            else
            {
                json = @"{ """ + word.name + @""": { ""pos"":""unknown"" } }";
                jo = JObject.Parse(json);
            }
            return jo;
        }

        public static void CreateNewWord(string word, string pos)
        {
            if (!string.IsNullOrEmpty(pos))
            {
                var filepath = ConfigurationManager.AppSettings["FilePath"] + word[0].ToString() + @"\" + word + ".json";

                StringBuilder sb = new StringBuilder("{").AppendLine();
                sb.AppendLine("  \"" + word + "\": {");
                sb.AppendLine("    \"pos\": [\"" + pos + "\"]");
                sb.AppendLine("  }");
                sb.AppendLine("}");

                if (!File.Exists(filepath))
                {
                    File.WriteAllText(filepath, sb.ToString());
                }
            }
        }

        //currently not in use
        public static void EditSubjectJson(Clause c)
        {
            int ID = c.subjects[0].ID;
            string subject = c.subjects[0].name;
            string verb = c.verbs[0].name;
            string pred = "";
            string json, jsonfile, filepath;
            JObject jo = new JObject();

            for (int i = 1; i <= c.predicate.Count; i++)
            {
                if (i == c.predicate.Count)
                {
                    pred += c.predicate[i].name;
                }
                else
                {
                    pred += c.predicate[i].name + " ";
                }

            }

            filepath = ConfigurationManager.AppSettings["FilePath"];

            jsonfile = filepath + subject[0].ToString() + @"\" + subject + ".json";

            if (File.Exists(jsonfile))
            {
                json = File.ReadAllText(jsonfile);
                json = Regex.Replace(json, @"}\s*}", @",""" + verb + @""" : """ + pred + @""" } }");
                jo = JObject.Parse(json);
                c.jol[ID] = jo;

            }

        }
    }
}
