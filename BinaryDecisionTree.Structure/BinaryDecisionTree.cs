namespace BinaryDecisionTree.Structure
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Binary Decision Tree
    /// </summary>
    public class BinaryDecisionTree
    {
        static BinaryDecisionTree()
        {
            EntryNode = new Node("Entry");
        }

        /// <summary>
        /// Initializes a new BASE instance of the <see cref="BinaryDecisionTree"/> class.
        /// </summary>
        /// <param name="value">The value of the only node.</param>
        public BinaryDecisionTree(string value)
            : this()
        {
            var node = new Node(value);

            this.Add(node);

            this.Add(new Vertex(BinaryDecisionTree.EntryNode, node));
        }

        internal BinaryDecisionTree()
        {
            Nodes = new List<Node>();
            Vertices = new List<Vertex>();
        }

        /// <summary>
        /// Gets the entry node.
        /// </summary>
        public static Node EntryNode { get; private set; }

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        public List<Node> Nodes { get; private set; }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        public List<Vertex> Vertices { get; private set; }
        
        /// <summary>
        /// Add a node.
        /// </summary>
        /// <param name="node">Nodes to add.</param>
        public void Add(params Node[] node)
        {
            Nodes.AddRange(node);
        }

        /// <summary>
        /// Remove a new node.
        /// </summary>
        /// <param name="node">Node to remove.</param>
        public void Remove(Node node)
        {
            Nodes.Remove(node);
        }

        /// <summary>
        /// Add a vertex.
        /// </summary>
        /// <param name="vertex">Vertices to add.</param>
        public void Add(params Vertex[] vertex)
        {
            Vertices.AddRange(vertex);
        }

        /// <summary>
        /// Remove a vertex.
        /// </summary>
        /// <param name="vertex">Vertex to remove.</param>
        public void Remove(Vertex vertex)
        {
            Vertices.Remove(vertex);
        }
    }
}
