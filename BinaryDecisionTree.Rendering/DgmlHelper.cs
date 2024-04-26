namespace BinaryDecisionTree.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using BinaryDecisionTree.Structure;

    /// <summary>
    /// DGML Helper
    /// </summary>
    public static class DgmlHelper
    {
        private static string ns = "http://schemas.microsoft.com/vs/2009/dgml";

        /// <summary>
        /// Convert a BDT to DGML.
        /// </summary>
        /// <param name="bdt">The BDT.</param>
        /// <returns>DGML representation of the BDT.</returns>
        public static XElement ToDgml(this BinaryDecisionTree bdt)
        {
            var nodes = bdt.ConnectedNodes().Select(n => n.ToDgml());
            var vertices = bdt.Vertices.Select(v => v.ToDgml());

            return CreateGraph(nodes, vertices);
        }

        /// <summary>
        /// Convert a <see cref="Node"/> to a DGML element.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>DGML representation of the <see cref="Node"/>.</returns>
        public static XElement ToDgml(this Node node)
        {
            return CreateDgmlNode(node.Id, node.Value);
        }

        /// <summary>
        /// Convert a <see cref="Link"/> to a DGML element.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>DGML representation of the <see cref="Vertex"/>.</returns>
        public static XElement ToDgml(this Vertex vertex)
        {
            return CreateDgmlLink(vertex.Source.Id, vertex.Target.Id, vertex.VertexType.Label());
        }

        private static XElement CreateGraph(IEnumerable<XElement> nodes, IEnumerable<XElement> links)
        {
            var dgml = new XElement(XName.Get("DirectedGraph", ns));
            var nodeCollection = new XElement(XName.Get("Nodes", ns));
            var linkCollection = new XElement(XName.Get("Links", ns));

            nodeCollection.Add(nodes.Cast<object>().ToArray());
            linkCollection.Add(links.Cast<object>().ToArray());

            dgml.Add(nodeCollection);
            dgml.Add(linkCollection);

            return dgml;
        }

        private static XElement CreateDgmlNode(Guid id, string label = "")
        {
            return new XElement(XName.Get("Node", ns), new XAttribute("Label", label), new XAttribute("Id", id));
        }

        private static XElement CreateDgmlLink(Guid sourceId, Guid targetId, string label = "")
        {
            return new XElement(XName.Get("Link", ns), new XAttribute("Label", label), new XAttribute("Source", sourceId), new XAttribute("Target", targetId));
        }
    }
}
