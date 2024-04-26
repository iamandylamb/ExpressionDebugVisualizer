namespace BinaryDecisionTree.Structure
{
    using System.Diagnostics;

    /// <summary>
    /// BDT Vertex
    /// </summary>
    [DebuggerDisplay("{Source.Value} -{VertexType}-> {Target.Value}")]
    public class Vertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="vertexType">Type of the vertex.</param>
        public Vertex(Node source, Node target, VertexType vertexType = VertexType.None)
        {
            this.Source = source;
            this.Target = target;
            this.VertexType = vertexType;
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public Node Source { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        public Node Target { get; set; }

        /// <summary>
        /// Gets or sets the type of the vertex.
        /// </summary>
        public VertexType VertexType { get; set; }
    }
}
