using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1
{
    class Session
    {
        //short-term memory
        public List<Sentence> stm { get; set; }

        public Session ()
        {
            stm = new List<Sentence>();
        }
    }
}
