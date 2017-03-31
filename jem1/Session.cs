using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1
{
    internal class Session
    {
        //short-term memory
        public List<Sentence> Stm { get; set; }

        public Session ()
        {
            Stm = new List<Sentence>();
        }
    }
}
