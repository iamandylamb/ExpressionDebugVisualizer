namespace BinaryDecisionTree.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Extensions
    {
        public static IEnumerable<Node> ConnectedNodes(this BinaryDecisionTree bdt)
        {
            return bdt.Vertices.SelectMany(v => new[] { v.Source, v.Target })
                               .Distinct().ToList();
        }

        public static IEnumerable<Node> Leaves(this BinaryDecisionTree bdt)
        {
            return bdt.Nodes.Where(n => !bdt.Vertices.Any(v => v.Source.Equals(n))).ToList();
        }

        public static IEnumerable<Vertex> IncomingVertices(this BinaryDecisionTree bdt, Node node)
        {
            return bdt.Vertices.Where(v => v.Target.Equals(node))
                               .OrderBy(v => v.VertexType)
                               .ThenBy(v => v.Target.Value).ToList();
        }

        public static IEnumerable<Vertex> OutgoingVertices(this BinaryDecisionTree bdt, Node node)
        {
            return bdt.Vertices.Where(v => v.Source.Equals(node))
                               .OrderBy(v => v.VertexType)
                               .ThenBy(v => v.Target.Value).ToList();
        }

        public static Vertex EntryVertex(this BinaryDecisionTree bdt)
        {
            return bdt.Vertices.Single(v => v.Source.Equals(BinaryDecisionTree.EntryNode));
        }

        public static bool SameAs(this BinaryDecisionTree expected, BinaryDecisionTree actual)
        {
            var expectedEntry = expected.EntryVertex();
            var actualEntry = actual.EntryVertex();

            return SameAs(expected, expectedEntry, actual, actualEntry);
        }

        private static bool SameAs(BinaryDecisionTree treeA, Vertex vertexA, BinaryDecisionTree treeB, Vertex vertexB)
        {
            return vertexA.VertexType == vertexB.VertexType 
                && string.Equals(vertexA.Target.Value, vertexB.Target.Value, StringComparison.InvariantCultureIgnoreCase)
                && treeA.OutgoingVertices(vertexA.Target)
                        .Zip(treeB.OutgoingVertices(vertexB.Target), (a, b) => new { a, b })
                        .All(nodes => SameAs(treeA, nodes.a, treeB, nodes.b));
        }
    }
}
