using jem1.Structure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1.Grammar
{
    //This class needs a lot of reworking after change to JSON structure and to make it more organized.
    class Question
    {
        public string[] qwords = { "do", "did", "does", "what", "where", "when", "why", "how", "which", "who", "whose", "can", "will", "have", "has", "had", "are" };
        public string q1 { get; set; }
        public string q2 { get; set; }
        public string q3 { get; set; }
        public string q4 { get; set; }
        public string answer { get; set; }

        public Question(Sentence s)
        {
            if (qwords.Contains(s.Words[0].Name.ToLower()))
            {
                q1 = s.Words[0].Name;
                q2 = s.WordCount > 1 ? s.Words[1].Name : null;
                q3 = s.WordCount > 2 ? s.Words[2].Name : null;
                q4 = s.WordCount > 3 ? s.Words[3].Name : null;
            }

            switch (q1.ToLower())
            {
                case "where":
                    FindLocation(s);
                    break;
                case "how":
                    if (q2 == "many")
                    {
                        FindNumber(s);
                    }
                    break;
                case "what":
                    if (JO.GetVal(s.Clauses[0].Jol[s.Clauses[0].I2.Id], "root") == "be")
                    {
                        Identify(s);
                    }
                    break;
            }
        }

        //Where
        private void FindLocation(Sentence s)
        {
            var c = s.Clauses[0];
            if (c.PN.Count > 0)
            {
                if (c.PN[0].Id > 0)
                {
                    string loc = "";
                    bool iknow = JO.HasProp(c.Jol[c.PN[0].Id], "location");
                    if (iknow)
                    {
                        loc = JO.GetVal(c.Jol[c.PN[0].Id], "location");
                        answer = loc;
                    }
                    else
                    {
                        loc = JO.GetValFromAncestors(c.PN[0].Name, "location", "location");
                        answer = !string.IsNullOrEmpty(loc) ? loc : "I've heard of it, but I don't know where it is.";
                    }
                }
            }
            else
            {
                if (c.Predicate.Count > 0)
                {
                    answer = "I have never heard of " + c.Predicate[c.Predicate.Count - 1].Name + ".";
                }
                else { answer = "I have never heard of it."; }
            }
        }

        //How many
        private void FindNumber(Sentence s)
        {
            var c = s.Clauses[0];
            //get singular for How Many q3
            var sing = JO.GetVal(q3, "singular");
            if (string.IsNullOrEmpty(sing)) { sing = q3; }

            string iknow = JO.GetVal2D(c.Jol[c.PN[0].Id], "has", sing);
            if (string.IsNullOrEmpty(iknow))
            {
                //check if property inherited
                answer = JO.GetValQtyFromAncestors(c.PN[0].Name, "has", sing);
                if (string.IsNullOrEmpty(answer)) { answer = "I don't know."; }
            }
            else
            {
                string num = JO.GetVal(c.Jol[c.PN[0].Id], q3);
                answer = num;
            }

        }

        //What is (are,am)
        private void Identify(Sentence s)
        {
            var c = s.Clauses[0];

            if (c.I3 != null && c.I3.Id > 0)
            {
                JObject toID;
                bool i3wasPlural = false;
                if (JO.HasProp(c.Jol[c.I3.Id], "singular"))
                {
                    toID = JO.GetJSONObject(JO.GetVal(c.Jol[c.I3.Id], "singular"));
                    i3wasPlural = true;
                }
                else
                {
                    toID = c.Jol[c.I3.Id];
                }

                var prop = string.IsNullOrEmpty(JO.GetVal(c.I2.Name, "singular")) ? c.I2.Name : JO.GetVal(c.I2.Name, "singular");


                if (c.Words[c.I3.Id - 1].Pos.Contains("possessive"))
                {
                    var root = JO.GetVal(c.Jol[c.I3.Id - 1], "root");
                    prop = JO.GetFirst(toID);
                    toID = JO.GetJSONObject(root);

                }

                bool iknow = JO.HasProp(toID, prop);
                if (iknow)
                {
                    string identity = JO.GetVal(toID, prop);
                    string description = "";
                    if (JO.HasProp(toID, "description"))
                    {
                        description = JO.GetVal(toID, "description");
                        //correct the spacing between words/commas in description
                        string[] dA = description.Split(',');
                        if(dA.Length > 1)
                        {
                            description = "";
                            for(int i = 0; i < dA.Length; i++)
                            {
                                description += i == dA.Length - 1 ? dA[i] : dA[i] + ", ";
                            }
                        }
                    }
                    //A plural word does not need a/an
                    if (i3wasPlural)
                    {
                        identity = string.IsNullOrEmpty(JO.GetVal(identity, "plural")) ? identity + "s" : JO.GetVal(identity, "plural");
                        answer = string.IsNullOrEmpty(description) ? identity : description + " " + identity;
                    }
                    else
                    {  //singular word requires a or an
                        if (string.IsNullOrEmpty(description))
                        {
                            answer = Determiner.AorAn(identity) + " " + identity;

                        }else
                        {
                            answer = Determiner.AorAn(description) + " " + description + " " + identity;
                        }
                        
                    }
                         
                }
                else
                {
                    answer = JO.GetValFromAncestors(JO.GetFirst(toID), prop, "description");
                    answer = string.IsNullOrEmpty(answer) ? "I've heard of it, but I don't know what it is." : answer;
                }
            }

        }

    }
}
