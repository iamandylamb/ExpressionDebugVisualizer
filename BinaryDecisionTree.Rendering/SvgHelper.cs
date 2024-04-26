namespace BinaryDecisionTree.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Msagl;
    using Microsoft.Msagl.Splines;
    using BinaryDecisionTree.Structure;
    using Bdt = BinaryDecisionTree.Structure;

    /// <summary>
    /// SVG Helper
    /// </summary>
    public static class SvgHelper
    {
        private static string ns = "http://www.w3.org/2000/svg";

        // Arbitrary values. Rubbish!
        private const double NodeSeparation = 50;
        private const int LabelOffset = 6;
        private const int LineSegmentCount = 50;
        private const string FontFamily = "Verdana";
        private const int FontSize = 12;

        private static readonly PlaneTransformation Orientation = new PlaneTransformation(-1, 0, 0, 0, -1, 0);

        /// <summary>
        /// Convert a BDT to SVG
        /// </summary>
        /// <param name="bdt">The BDT.</param>
        /// <returns>SVG representation of the BDT.</returns>
        public static XDocument ToSvg(this Structure.BinaryDecisionTree bdt)
        {
            var graph = new GeometryGraph();

            var nodes = bdt.ConnectedNodes().Select(n => n.CreateNode()).ToList();
            var edges = bdt.Vertices.Select(v => v.CreateEdge(nodes)).ToList();

            nodes.ForEach(graph.AddNode);
            edges.ForEach(graph.AddEdge);

            // Flip so orientation matches DGML.
            graph.Transformation = Orientation;

            graph.NodeSeparation = NodeSeparation;
            graph.LayerSeparation = NodeSeparation;

            graph.CalculateLayout();

            // Move the centre away from 0,0 as that's 'top left' in SVG coords.
            graph.Translate(new Point(-graph.Left, -graph.Bottom));

            var svg = new XElement(
                XName.Get("svg", ns),
                new XAttribute("version", "1.1"));

            edges.ForEach(e =>
            {
                svg.Add(CreateLink(e));
                svg.Add(CreateArrowHead(e));

                // Storing the label in 'UserData' is rubbish.
                svg.Add(CreateLabel(e.UserData.ToString(), e.Label.Center));
            });

            nodes.ForEach(n =>
            {
                var bound = n.BoundingBox;
                var labelOrigin = new Point(bound.Left + LabelOffset, bound.Top - LabelOffset);

                svg.Add(CreateRectangle(bound));

                // Storing the label in 'UserData' is rubbish.
                svg.Add(CreateLabel(n.UserData.ToString(), labelOrigin));
            });

            return new XDocument(new XDeclaration("1.0", null, "yes"), svg);
        }

        private static Microsoft.Msagl.Node CreateNode(this Bdt.Node node)
        {
            var textSize = TextSize(node.Value);
            var size = System.Drawing.SizeF.Add(textSize, new System.Drawing.SizeF(LabelOffset * 2, LabelOffset));

            var element = new Microsoft.Msagl.Node(
                node.Id.ToString(),
                CurveFactory.CreateBox(size.Width, size.Height, LabelOffset / 2, LabelOffset / 2, new Point()))
            {
                UserData = node.Value
            };

            return element;
        }

        private static Edge CreateEdge(this Bdt.Vertex vertex, IList<Microsoft.Msagl.Node> nodes)
        {
            var edge = new Microsoft.Msagl.Edge(
                nodes.Single(n => n.Id == vertex.Source.Id.ToString()),
                nodes.Single(n => n.Id == vertex.Target.Id.ToString()))
            {
                UserData = vertex.VertexType.Label()
            };

            edge.Label = new Label(0, 0, edge);

            return edge;
        }

        private static XElement CreateRectangle(Rectangle bound)
        {
            return new XElement(
                XName.Get("rect", ns),
                new XAttribute("x", bound.Left),
                new XAttribute("y", bound.Bottom),
                new XAttribute("rx", LabelOffset / 2),
                new XAttribute("ry", LabelOffset / 2),
                new XAttribute("width", bound.Width),
                new XAttribute("height", bound.Height),
                new XAttribute("stroke", "#999999"),
                new XAttribute("stroke-width", "1"),
                new XAttribute("fill", "white"));
        }

        private static XElement CreateLabel(string text, Point point)
        {
            return new XElement(
                XName.Get("text", ns),
                new XAttribute("x", point.X),
                new XAttribute("y", point.Y),
                new XAttribute("font-family", FontFamily),
                new XAttribute("font-size", FontSize),
                text);
        }

        private static XElement CreateArrowHead(Edge e)
        {
            var path = CreateArrow(e).Format();

            return new XElement(
                XName.Get("polygon", ns),
                new XAttribute("points", path),
                new XAttribute("fill", "#BBBBBB"));
        }

        private static XElement CreateLink(Edge e)
        {
            var path = CreateCurve(e).Format();

            return new XElement(
                XName.Get("polyline", ns),
                new XAttribute("points", path),
                new XAttribute("fill", "none"),
                new XAttribute("stroke", "#BBBBBB"),
                new XAttribute("stroke-width", "1"));
        }

        private static IEnumerable<Point> CreateCurve(Edge e)
        {
            var curve = e.Curve;
            var delta = (curve.ParEnd - curve.ParStart) / LineSegmentCount;
            var points = new List<Point>();

            for (var d = curve.ParStart; d <= curve.ParEnd; d += delta)
            {
                points.Add(curve[d]);
            }

            points.Add(curve.End);

            return points;
        }

        private static IEnumerable<Point> CreateArrow(Edge e)
        {
            var tip = e.ArrowheadAtTargetPosition;
            var end = e.Curve.End;

            double dx = end.X - tip.X, dy = end.Y - tip.Y;
            var dist = Math.Sqrt((dx * dx) + (dy * dy));
            dx /= dist;
            dy /= dist;

            var n = e.ArrowheadLength / 2;
            var p1 = new Point(end.X + (n * dy), end.Y - (n * dx));
            var p2 = new Point(end.X - (n * dy), end.Y + (n * dx));

            return new[] { tip, p1, p2 };
        }

        private static System.Drawing.SizeF TextSize(string text)
        {
            using (var font = new System.Drawing.Font(FontFamily, FontSize, System.Drawing.GraphicsUnit.Pixel))
            {
                using (System.Drawing.Image fakeImage = new System.Drawing.Bitmap(1, 1))
                {
                    using (var graphics = System.Drawing.Graphics.FromImage(fakeImage))
                    {
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                        return graphics.MeasureString(text, font, int.MaxValue, System.Drawing.StringFormat.GenericTypographic);
                    }
                }
            }
        }

        private static string Format(this IEnumerable<Point> points)
        {
            return points.Aggregate(string.Empty, (s, p) => s + " " + Format(p)).Trim();
        }

        private static string Format(Point point)
        {
            return string.Format("{0}, {1}", point.X, point.Y);
        }
    }
}
