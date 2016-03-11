using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1
{
    public class Node<T>
    {
        // Private member-variables
        private T data;
        private NodeList<T> links = null;

        public Node() { }
        public Node(T data) : this(data, null) { }
        public Node(T data, NodeList<T> links)
        {
            this.data = data;
            this.links = links;
        }

        public T Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        protected NodeList<T> Links
        {
            get
            {
                return links;
            }
            set
            {
                links = value;
            }
        }
    }
}

