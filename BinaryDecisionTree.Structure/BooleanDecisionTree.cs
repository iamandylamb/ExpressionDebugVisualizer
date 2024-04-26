namespace BinaryDecisionTree.Structure
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Boolean Decision Tree
    /// </summary>
    public class BooleanDecisionTree : BinaryDecisionTree
    {
        static BooleanDecisionTree()
        {
            TrueNode = new Node("True");
            FalseNode = new Node("False");
        }

        /// <summary>
        /// Initializes a new BASE instance of the <see cref="BinaryDecisionDiagram"/> class.
        /// </summary>
        /// <param name="value">The value of the only node.</param>
        public BooleanDecisionTree(string value)
            : base(value)
        {
            this.Add(new Vertex(Nodes.Single(), BooleanDecisionTree.TrueNode, VertexType.True));
            this.Add(new Vertex(Nodes.Single(), BooleanDecisionTree.FalseNode, VertexType.False));
        }

        internal BooleanDecisionTree()
        {
        }

        /// <summary>
        /// Gets the true node.
        /// </summary>
        public static Node TrueNode { get; private set; }

        /// <summary>
        /// Gets the false node.
        /// </summary>
        public static Node FalseNode { get; private set; }
    }
}
