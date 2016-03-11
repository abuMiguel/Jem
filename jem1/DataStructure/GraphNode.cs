using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1
{
    public class GraphNode<T> : Node<T>
    {
        private List<int> costs;

        public GraphNode() : base() { }
        public GraphNode(T value) : base(value) { }
        public GraphNode(T value, NodeList<T> links) : base(value, links) { }

        new public NodeList<T> Links
        {
            get
            {
                if (base.Links == null)
                    base.Links = new NodeList<T>();

                return base.Links;
            }
        }

        public List<int> Costs
        {
            get
            {
                if (costs == null)
                    costs = new List<int>();

                return costs;
            }
        }
    }
}
