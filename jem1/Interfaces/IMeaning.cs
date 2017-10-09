using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1
{
    public interface IMeaning
    {
        string Name { get; set; }
        IEnumerable<string> Is { get; set; }
        string Definition { get; set; }
        IEnumerable<string> Synonyms { get; set; }
    }
}
