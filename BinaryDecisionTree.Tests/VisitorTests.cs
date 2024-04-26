namespace BinaryDecisionTree.Tests
{
    using BinaryDecisionTree.Debugger;
    using BinaryDecisionTree.Rendering;
    using BinaryDecisionTree.Structure;
    using BinaryDecisionTree.Visitors;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    [TestClass]
    public class VisitorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void SingleBinaryExpression()
        {
            Expression<Func<Entity, int>> expr = e => e.X + e.Y + e.Z;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BinaryDecisionTree();
            var node1 = new Node("(Entity.X + Entity.Y) + Entity.Z");

            expected.Add(node1);
            expected.Add(new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void BooleanExpression()
        {
            Expression<Func<Entity, bool>> expr = e => e.Flag;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Flag");

            expected.Add(node1);
            expected.Add(new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                         new Vertex(node1, BooleanDecisionTree.TrueNode, VertexType.True),
                         new Vertex(node1, BooleanDecisionTree.FalseNode, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void BooleanAndExpression()
        {
            Expression<Func<Entity, bool>> expr = e => e.Flag && e.Length == 0;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Flag");
            var node2 = new Node("Entity.Length == 0");

            expected.Add(node1, node2);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node2, VertexType.True),
                new Vertex(node1, BooleanDecisionTree.FalseNode, VertexType.False),
                new Vertex(node2, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.FalseNode, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void BooleanOrExpression()
        {
            Expression<Func<Entity, bool>> expr = e => e.Flag || e.Length == 0;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Flag");
            var node2 = new Node("Entity.Length == 0");

            expected.Add(node1, node2);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node2, VertexType.False),
                new Vertex(node1, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.FalseNode, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void ConstantComparisonExpression()
        {
            const int A = 1;
            Expression<Func<Entity, bool>> expr = e => e.X > A;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.X > 1");

            expected.Add(node1);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, BooleanDecisionTree.FalseNode, VertexType.False),
                new Vertex(node1, BooleanDecisionTree.TrueNode, VertexType.True));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void ValueTypeExpression()
        {
            Expression<Func<int, int>> expr = i => i > 0 ? i + 1 : i - 1;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Int32 > 0");
            var node2 = new Node("Int32 + 1");
            var node3 = new Node("Int32 - 1");

            expected.Add(node1, node2, node3);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node3, VertexType.False),
                new Vertex(node1, node2, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.FalseNode, VertexType.False), 
                new Vertex(node3, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node3, BooleanDecisionTree.FalseNode, VertexType.False));
            
            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void NotExpression()
        {
            Expression<Func<Entity, bool>> expr = e => !e.Flag;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Flag");

            expected.Add(node1);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, BooleanDecisionTree.FalseNode, VertexType.True),
                new Vertex(node1, BooleanDecisionTree.TrueNode, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void StaticMethodCallExpression()
        {
            Expression<Func<Entity, bool>> expr = e => string.IsNullOrEmpty(e.Name);

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("String.IsNullOrEmpty(Entity.Name)");

            expected.Add(node1);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node1, BooleanDecisionTree.FalseNode, VertexType.False));
            
            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void InstanceMethodCallExpression()
        {
            Expression<Func<Entity, bool>> expr = e => e.X.ToString("0.000") == "1.234";

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.X.ToString(\"0.000\") == \"1.234\"");

            expected.Add(node1);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node1, BooleanDecisionTree.FalseNode, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void NullableExpression()
        {
            Expression<Func<Entity, int>> expr = e => e.NullableFlag == null ? e.X : e.Y;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.NullableFlag == null");
            var node2 = new Node("Entity.X");
            var node3 = new Node("Entity.Y");

            expected.Add(node1, node2, node3);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node2, VertexType.True),
                new Vertex(node1, node3, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void ConstantEnumExpression()
        {
            Expression<Func<Entity, int>> expr = e => e.Day == DayOfWeek.Monday ? e.X : e.Y;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Day == Monday");
            var node2 = new Node("Entity.X");
            var node3 = new Node("Entity.Y");

            expected.Add(node1, node2, node3);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node2, VertexType.True),
                new Vertex(node1, node3, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void ComplexLeftComparisonExpression()
        {
            Expression<Func<Entity, bool>> expr = e => (e.Flag ? e.X : e.Y) > 1;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Flag");
            var node2 = new Node("Entity.X > 1");
            var node3 = new Node("Entity.Y > 1");

            expected.Add(node1, node2, node3);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node3, VertexType.False),
                new Vertex(node1, node2, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.FalseNode, VertexType.False),
                new Vertex(node3, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node3, BooleanDecisionTree.FalseNode, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void ComplexRightComparisonExpression()
        {
            Expression<Func<Entity, bool>> expr = e => 1 <= (e.Flag ? e.X : e.Y);

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Flag");
            var node2 = new Node("1 <= Entity.X");
            var node3 = new Node("1 <= Entity.Y");

            expected.Add(node1, node2, node3);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node3, VertexType.False),
                new Vertex(node1, node2, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.FalseNode, VertexType.False),
                new Vertex(node3, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node3, BooleanDecisionTree.FalseNode, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void ComplexLeftBooleanExpression()
        {
            Expression<Func<Entity, bool>> expr = e => (e.Flag ? e.X > 0 : e.Y <= 1) && e.Z == 2;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Flag");
            var node2 = new Node("Entity.X > 0");
            var node3 = new Node("Entity.Y <= 1");
            var node4 = new Node("Entity.Z == 2");

            expected.Add(node1, node2, node3, node4);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node3, VertexType.False),
                new Vertex(node1, node2, VertexType.True),
                new Vertex(node2, node4, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.FalseNode, VertexType.False),
                new Vertex(node3, node4, VertexType.True),
                new Vertex(node3, BooleanDecisionTree.FalseNode, VertexType.False),
                new Vertex(node4, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node4, BooleanDecisionTree.FalseNode, VertexType.False));
            
            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void ComplexRightBooleanExpression()
        {
            Expression<Func<Entity, bool>> expr = e => e.Z != 1 || (e.Name == "Bob" ? e.X > 0 : !(e.Y <= 1 && e.Flag));

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Z != 1");
            var node2 = new Node("Entity.Name == \"Bob\"");
            var node3 = new Node("Entity.Y <= 1");
            var node4 = new Node("Entity.X > 0");
            var node5 = new Node("Entity.Flag");

            expected.Add(node1, node2, node3, node4, node5);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node2, VertexType.False),
                new Vertex(node1, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node2, node4, VertexType.True),
                new Vertex(node2, node3, VertexType.False),
                new Vertex(node3, node5, VertexType.True),
                new Vertex(node3, BooleanDecisionTree.TrueNode, VertexType.False),
                new Vertex(node4, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node4, BooleanDecisionTree.FalseNode, VertexType.False),
                new Vertex(node5, BooleanDecisionTree.FalseNode, VertexType.True),
                new Vertex(node5, BooleanDecisionTree.TrueNode, VertexType.False));
            
            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void ComplexExpression()
        {
            Expression<Func<Entity, bool>> expr = e => (1 <= (e.Flag ? e.X : !(e.Y > 0) ? e.Y : e.Y + e.Z)) && e.Length == 2;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr);

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Flag");
            var node2 = new Node("1 <= Entity.X");
            var node3 = new Node("Entity.Y > 0");
            var node4 = new Node("1 <= Entity.Y");
            var node5 = new Node("1 <= Entity.Y + Entity.Z");
            var node6 = new Node("Entity.Length == 2");

            expected.Add(node1, node2, node3, node4, node5, node6);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node3, VertexType.False),
                new Vertex(node1, node2, VertexType.True),
                new Vertex(node3, node4, VertexType.False),
                new Vertex(node3, node5, VertexType.True),
                new Vertex(node2, node6, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.FalseNode, VertexType.False),
                new Vertex(node4, node6, VertexType.True),
                new Vertex(node4, BooleanDecisionTree.FalseNode, VertexType.False),
                new Vertex(node5, node6, VertexType.True),
                new Vertex(node5, BooleanDecisionTree.FalseNode, VertexType.False),
                new Vertex(node6, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node6, BooleanDecisionTree.FalseNode, VertexType.False));
            
            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void SimpleBuiltExpression()
        {
            Expression<Func<Entity, bool>> expr1 = e => e.Flag;
            Expression<Func<Entity, bool>> expr2 = e => e.Length == 0;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(expr1.Or(expr2));

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Flag");
            var node2 = new Node("Entity.Length == 0");

            expected.Add(node1, node2);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node2, VertexType.False),
                new Vertex(node1, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.FalseNode, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        [TestMethod]
        public void ComplexBuiltExpression()
        {
            Expression<Func<Entity, bool>> expr1 = e => e.Flag;
            Expression<Func<Entity, bool>> expr2 = e => e.X > 2;
            Expression<Func<Entity, bool>> expr3 = e => 3 >= e.Y;

            var actual = BinaryDecisionTreeExpressionVisitor.Build(
                                expr1.Not().And(expr2).Or(expr3));

            var expected = new BooleanDecisionTree();
            var node1 = new Node("Entity.Flag");
            var node2 = new Node("Entity.X > 2");
            var node3 = new Node("3 >= Entity.Y");

            expected.Add(node1, node2, node3);
            expected.Add(
                new Vertex(BinaryDecisionTree.EntryNode, node1, VertexType.None),
                new Vertex(node1, node2, VertexType.False),
                new Vertex(node1, node3, VertexType.True),
                new Vertex(node2, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node2, node3, VertexType.False),
                new Vertex(node3, BooleanDecisionTree.TrueNode, VertexType.True),
                new Vertex(node3, BooleanDecisionTree.FalseNode, VertexType.False));

            Assert.IsTrue(expected.SameAs(actual));
        }

        private void WriteToFile(BinaryDecisionTree bdd)
        {
            var dgmlFileName = string.Format("{0}.dgml", TestContext.TestName);

            bdd.ToDgml().Save(dgmlFileName);

            Process.Start(dgmlFileName);
        }
    }
}
