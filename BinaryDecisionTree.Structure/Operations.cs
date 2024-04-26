namespace BinaryDecisionTree.Structure
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// BDT Operations
    /// </summary>
    public static class Operations
    {
        private static readonly IDictionary<ExpressionType, string> ComparisonOperators = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.Equal, "==" },
            { ExpressionType.NotEqual, "!=" },
            { ExpressionType.GreaterThan, ">" },
            { ExpressionType.GreaterThanOrEqual, ">=" },
            { ExpressionType.LessThan, "<" },
            { ExpressionType.LessThanOrEqual, "<=" },
        };

        /// <summary>
        /// Determines whether the specified type is an AND.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>True</c> if the type is an AND; <c>False</c> otherwise.</returns>
        public static bool IsAnd(this ExpressionType type)
        {
            return type == ExpressionType.And || type == ExpressionType.AndAlso;
        }

        /// <summary>
        /// Determines whether the specified type is an OR.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>True</c> if the type is an OR; <c>False</c> otherwise.</returns>
        public static bool IsOr(this ExpressionType type)
        {
            return type == ExpressionType.Or || type == ExpressionType.OrElse;
        }

        /// <summary>
        /// Determines whether the specified type is a comparison.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>True</c> if the type is a comparison; <c>False</c> otherwise.</returns>
        public static bool IsComparison(this ExpressionType type)
        {
            return ComparisonOperators.ContainsKey(type);
        }

        /// <summary>
        /// Convert a <see cref="BinaryDecisionTree"/> to a <see cref="BooleanDecisionTree"/>.
        /// </summary>
        /// <param name="sourceBdt">BDT to convert.</param>
        /// <returns>Boolean decision tree.</returns>
        public static BooleanDecisionTree AsBooleanDecisionTree(this BinaryDecisionTree sourceBdt)
        {
            if (sourceBdt is BooleanDecisionTree)
            {
                return sourceBdt as BooleanDecisionTree; // No conversion required.
            }

            BooleanDecisionTree bdt = new BooleanDecisionTree();

            bdt.Nodes.CopyFrom(sourceBdt);
            bdt.Vertices.CopyFrom(sourceBdt);

            foreach (var leaf in bdt.Leaves())
            {
                bdt.Add(new Vertex(leaf, BooleanDecisionTree.TrueNode, VertexType.True));
                bdt.Add(new Vertex(leaf, BooleanDecisionTree.FalseNode, VertexType.False));
            }

            return bdt;
        }

        /// <summary>
        /// Build the BDT <paramref name="left"/> <paramref name="comparison"/> <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The left BDT.</param>
        /// <param name="comparison">The comparison operator.</param>
        /// <param name="right">The right BDT.</param>
        /// <returns>Left compared to Right BDT</returns>
        public static BooleanDecisionTree Compare(this BinaryDecisionTree left, ExpressionType comparison, BinaryDecisionTree right)
        {
            if (left.Nodes.Count() > 1 && right.Nodes.Count() > 1)
            {
                // Could this type of comparison be graphed?
                throw new System.InvalidOperationException("Unable to graph expression");
            }

            BinaryDecisionTree tree = null;
            Node node = null;
            string format = null;  
         
            if (right.Nodes.Count() == 1)
            {
                tree = left;
                node = right.Nodes.Single();
                format = "{0} {1} {2}";
            }
            else if (left.Nodes.Count() == 1)
            {
                tree = right;
                node = left.Nodes.Single();
                format = "{2} {1} {0}";            
            }

            var bdt = new BooleanDecisionTree();
                                    
            foreach (var leaf in tree.Leaves())
            {
                tree.Remove(leaf);
                var compare = new Node(string.Format(format, leaf.Value, ComparisonOperators[comparison], node.Value));
                bdt.Add(compare);

                bdt.Add(new Vertex(compare, BooleanDecisionTree.TrueNode, VertexType.True));
                bdt.Add(new Vertex(compare, BooleanDecisionTree.FalseNode, VertexType.False));

                foreach (var vertex in tree.IncomingVertices(leaf))
                {
                    tree.Remove(vertex);
                    bdt.Add(new Vertex(vertex.Source, compare, vertex.VertexType));
                }
            }

            bdt.Nodes.CopyFrom(tree);
            bdt.Vertices.CopyFrom(tree);

            return bdt;
        }

        /// <summary>
        /// Build the BDT: <paramref name="test" /> ? <paramref name="ifTrue" /> : <paramref name="ifFalse" />
        /// </summary>
        /// <param name="test">The test BDT.</param>
        /// <param name="ifTrue">The true branch BDT.</param>
        /// <param name="ifFalse">The false branch BDT.</param>
        /// <returns>Test ? True : False BDT.</returns>
        public static BinaryDecisionTree Conditional(this BooleanDecisionTree test, BinaryDecisionTree ifTrue, BinaryDecisionTree ifFalse)
        {
            var bdt = new BinaryDecisionTree();

            bdt.Nodes.CopyFrom(test, ifTrue, ifFalse);

            var ifTrueVertex = ifTrue.EntryVertex();
            foreach (var testVertex in test.IncomingVertices(BooleanDecisionTree.TrueNode))
            {
                test.Remove(testVertex);
                bdt.Add(new Vertex(testVertex.Source, ifTrueVertex.Target, testVertex.VertexType));
            }

            ifTrue.Remove(ifTrueVertex);

            var ifFalseVertex = ifFalse.EntryVertex();
            foreach (var testVertex in test.IncomingVertices(BooleanDecisionTree.FalseNode))
            {
                test.Remove(testVertex);
                bdt.Add(new Vertex(testVertex.Source, ifFalseVertex.Target, testVertex.VertexType));
            }

            ifFalse.Remove(ifFalseVertex);

            bdt.Vertices.CopyFrom(test, ifTrue, ifFalse);

            return bdt;
        }

        /// <summary>
        /// Build the BDT: !<paramref name="operand"/>
        /// </summary>
        /// <param name="operand">The operand BDT.</param>
        /// <returns>Not BDT</returns>
        public static BooleanDecisionTree Not(this BooleanDecisionTree operand)
        {
            var bdt = new BooleanDecisionTree();

            bdt.Nodes.CopyFrom(operand);

            foreach (var trueVertex in operand.IncomingVertices(BooleanDecisionTree.TrueNode))
            {
                operand.Remove(trueVertex);
                bdt.Add(new Vertex(trueVertex.Source, BooleanDecisionTree.FalseNode, trueVertex.VertexType));
            }

            foreach (var falseVertex in operand.IncomingVertices(BooleanDecisionTree.FalseNode))
            {
                operand.Remove(falseVertex);
                bdt.Add(new Vertex(falseVertex.Source, BooleanDecisionTree.TrueNode, falseVertex.VertexType));
            }

            bdt.Vertices.CopyFrom(operand);

            return bdt;
        }

        /// <summary>
        /// Build the BDT: <paramref name="left" /> AND <paramref name="right" />
        /// </summary>
        /// <param name="left">The left BDT.</param>
        /// <param name="right">The right BDT.</param>
        /// <returns>Left AND Right BDT.</returns>
        public static BooleanDecisionTree And(this BooleanDecisionTree left, BooleanDecisionTree right)
        {
            var bdt = new BooleanDecisionTree();

            bdt.Nodes.CopyFrom(left, right);

            var rightVertex = right.EntryVertex();
            foreach (var leftVertex in left.IncomingVertices(BooleanDecisionTree.TrueNode))
            {
                left.Remove(leftVertex);
                bdt.Add(new Vertex(leftVertex.Source, rightVertex.Target, leftVertex.VertexType));
            }

            right.Remove(rightVertex);

            bdt.Vertices.CopyFrom(left, right);

            return bdt;
        }

        /// <summary>
        /// Build the BDT: <paramref name="left" /> OR <paramref name="right" />
        /// </summary>
        /// <param name="left">The left BDT.</param>
        /// <param name="right">The right BDT.</param>
        /// <returns>Left OR Right BDT.</returns>
        public static BooleanDecisionTree Or(this BooleanDecisionTree left, BooleanDecisionTree right)
        {
            var bdt = new BooleanDecisionTree();

            bdt.Nodes.CopyFrom(left, right);

            var rightVertex = right.EntryVertex();

            foreach (var leftVertex in left.IncomingVertices(BooleanDecisionTree.FalseNode))
            {
                left.Remove(leftVertex);
                bdt.Add(new Vertex(leftVertex.Source, rightVertex.Target, leftVertex.VertexType));
            }

            right.Remove(rightVertex);

            bdt.Vertices.CopyFrom(left, right);

            return bdt;
        }

        /// <summary>
        /// Remove duplicate leave nodes from the supplied <paramref name="BDT"/>
        /// </summary>
        /// <param name="bdt">BDT to remove duplicate leaves from.</param>
        /// <returns>Updated BDT.</returns>
        public static BinaryDecisionTree RemoveDuplicateLeaves(this BinaryDecisionTree bdt)
        {
            var duplicateLeafGroups = bdt.Leaves().GroupBy(n => n.GetHashCode()).Where(g => g.Count() > 1);

            foreach (var duplicateLeafGroup in duplicateLeafGroups)
            {
                var primaryLeaf = duplicateLeafGroup.First();
                var duplicateLeaves = duplicateLeafGroup.Skip(1);

                foreach (var duplicateLeaf in duplicateLeaves)
                {
                    var sourceVertices = bdt.Vertices.Where(v => v.Source.Equals(duplicateLeaf)).ToList();
                    sourceVertices.ForEach(vertex =>
                    {
                        bdt.Remove(vertex);
                        bdt.Add(new Vertex(primaryLeaf, vertex.Target, vertex.VertexType));
                    });

                    var targetVertices = bdt.Vertices.Where(v => v.Target.Equals(duplicateLeaf)).ToList();
                    targetVertices.ForEach(vertex =>
                    {
                        bdt.Remove(vertex);
                        bdt.Add(new Vertex(vertex.Source, primaryLeaf, vertex.VertexType));
                    });
                }
            }

            return bdt;
        }

        private static void CopyFrom(this List<Vertex> vertices, params BinaryDecisionTree[] bdts)
        {
            foreach (var bdt in bdts)
            {
                vertices.AddRange(bdt.Vertices);
            }
        }

        private static void CopyFrom(this List<Node> nodes, params BinaryDecisionTree[] bdts)
        {
            foreach (var bdt in bdts)
            {
                nodes.AddRange(bdt.Nodes);
            }
        }
    }
}
