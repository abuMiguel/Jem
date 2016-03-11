using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jem1
{
    public class Graph<T> : IEnumerable<T>
    {
        private NodeList<T> nodeSet;

        public Graph() : this(null) { }
        public Graph(NodeList<T> nodeSet)
        {
            if (nodeSet == null)
                this.nodeSet = new NodeList<T>();
            else
                this.nodeSet = nodeSet;
        }

        public void AddNode(GraphNode<T> node)
        {
            // adds a node to the graph
            nodeSet.Add(node);
        }

        public void AddNode(T value)
        {
            // adds a node to the graph
            nodeSet.Add(new GraphNode<T>(value));
        }

        public void AddDirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            from.Links.Add(to);
            from.Costs.Add(cost);
        }

        public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            from.Links.Add(to);
            from.Costs.Add(cost);

            to.Links.Add(from);
            to.Costs.Add(cost);
        }

        public bool Contains(T value)
        {
            return nodeSet.FindByValue(value) != null;
        }

        public bool Remove(T value)
        {
            // first remove the node from the nodeset
            GraphNode<T> nodeToRemove = (GraphNode<T>)nodeSet.FindByValue(value);
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            // otherwise, the node was found
            nodeSet.Remove(nodeToRemove);

            // enumerate through each node in the nodeSet, removing edges to this node
            foreach (GraphNode<T> gnode in nodeSet)
            {
                int index = gnode.Links.IndexOf(nodeToRemove);
                if (index != -1)
                {
                    // remove the reference to the node and associated cost
                    gnode.Links.RemoveAt(index);
                    gnode.Costs.RemoveAt(index);
                }
            }

            return true;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public NodeList<T> Nodes
        {
            get
            {
                return nodeSet;
            }
        }

        public int Count
        {
            get { return nodeSet.Count; }
        }
    }
}
