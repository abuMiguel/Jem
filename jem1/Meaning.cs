using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1
{
    public class Meaning : IMeaning
    {
        public string Name { get; set; }
        public IEnumerable<string> Is { get; set; }
        public string Definition { get; set; }
        public IEnumerable<string> Synonyms { get; set; }
    }
}
