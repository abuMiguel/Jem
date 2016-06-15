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
            if (qwords.Contains(s.words[0].name.ToLower()))
            {
                q1 = s.words[0].name;
                q2 = s.WordCount() > 1 ? s.words[1].name : null;
                q3 = s.WordCount() > 2 ? s.words[2].name : null;
                q4 = s.WordCount() > 3 ? s.words[3].name : null;
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
                    if (JO.GetVal(s.clauses[0].jol[s.clauses[0].i2.ID], "root") == "be")
                    {
                        Identify(s);
                    }
                    break;
            }
        }

        //Where
        private void FindLocation(Sentence s)
        {
            var c = s.clauses[0];
            if (c.pN.Count > 0)
            {
                if (c.pN[0].ID > 0)
                {
                    string loc = "";
                    bool iknow = JO.HasProp(c.jol[c.pN[0].ID], "location");
                    if (iknow)
                    {
                        loc = JO.GetVal(c.jol[c.pN[0].ID], "location");
                        answer = loc;
                    }
                    else
                    {
                        loc = JO.GetValFromAncestors(c.pN[0].name, "location", "location");
                        answer = !string.IsNullOrEmpty(loc) ? loc : "I've heard of it, but I don't know where it is.";
                    }
                }
            }
            else
            {
                if (c.predicate.Count > 0)
                {
                    answer = "I have never heard of " + c.predicate[c.predicate.Count - 1].name + ".";
                }
                else { answer = "I have never heard of it."; }
            }
        }

        //How many
        private void FindNumber(Sentence s)
        {
            var c = s.clauses[0];
            //get singular for How Many q3
            var sing = JO.GetVal(q3, "singular");
            if (string.IsNullOrEmpty(sing)) { sing = q3; }

            string iknow = JO.GetVal2D(c.jol[c.pN[0].ID], "has", sing);
            if (string.IsNullOrEmpty(iknow))
            {
                //check if property inherited
                answer = JO.GetValQtyFromAncestors(c.pN[0].name, "has", sing);
                if (string.IsNullOrEmpty(answer)) { answer = "I don't know."; }
            }
            else
            {
                string num = JO.GetVal(c.jol[c.pN[0].ID], q3);
                answer = num;
            }

        }

        //What is (are,am)
        private void Identify(Sentence s)
        {
            var c = s.clauses[0];

            if (c.i3 != null && c.i3.ID > 0)
            {
                JObject toID;
                bool i3wasPlural = false;
                if (JO.HasProp(c.jol[c.i3.ID], "singular"))
                {
                    toID = JO.GetJSONObject(JO.GetVal(c.jol[c.i3.ID], "singular"));
                    i3wasPlural = true;
                }
                else
                {
                    toID = c.jol[c.i3.ID];
                }

                var prop = string.IsNullOrEmpty(JO.GetVal(c.i2.name, "singular")) ? c.i2.name : JO.GetVal(c.i2.name, "singular");


                if (c.words[c.i3.ID - 1].pos.Contains("possessive"))
                {
                    var root = JO.GetVal(c.jol[c.i3.ID - 1], "root");
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
