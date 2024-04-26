namespace BinaryDecisionTree.Visitors
{
    using System;
    using System.Linq.Expressions;
    using BinaryDecisionTree.Structure;

    /// <summary>
    /// Expression to BDT generator
    /// </summary>
    public class BinaryDecisionTreeExpressionVisitor : ExpressionVisitor
    {
        private BinaryDecisionTree bdt;

        private BinaryDecisionTreeExpressionVisitor()
        {
        }

        /// <summary>
        /// Builds the binary decision tree from the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>A BDD built from the expression.</returns>
        public static BinaryDecisionTree Build(Expression expression)
        {
            var renamer = new ParameterRenameVisitor();

            // Apply any preparation visitors.
            expression = renamer.Visit(expression);

            var bdt = BuildBinaryDecisionTree(expression);

            // Sometimes this makes the BDT easier to read, sometimes not.
            // bdt = bdt.RemoveDuplicateLeaves();

            return bdt;
        }

        /// <summary>
        /// Builds the binary decision tree from the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>A BDT built from the expression.</returns>
        private static BinaryDecisionTree BuildBinaryDecisionTree(Expression expression)
        {
            var visitor = new BinaryDecisionTreeExpressionVisitor();

            expression = visitor.Visit(expression);

            return visitor.bdt ?? new BinaryDecisionTree(TrimParenthesis(expression));
        }

        /// <summary>
        /// Builds the boolean decision tree from the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>A BDT built from the expression.</returns>
        private static BooleanDecisionTree BuildBooleanDecisionTree(Expression expression)
        {
            var visitor = new BinaryDecisionTreeExpressionVisitor();

            expression = visitor.Visit(expression);

            return visitor.bdt.AsBooleanDecisionTree();
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = TrimParenthesis(node);

            this.bdt = node.Type == typeof(bool)
                ? new BooleanDecisionTree(value)
                : new BinaryDecisionTree(value);

            return node;
        }

        // Without this method the method call is hiddent but the argument is rendered.
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var value = node.Method.IsStatic
                ? string.Format("{0}.{1}", node.Method.DeclaringType.Name, node)
                : node.ToString();

            this.bdt = node.Type == typeof(bool)
                ? new BooleanDecisionTree(value)
                : new BinaryDecisionTree(value);

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            // Mask calls to nullable members with the underlying member.
            var expression = IsNullableMember(node) ? node.Expression : node;

            var value = TrimParenthesis(expression);

            this.bdt = node.Type == typeof(bool)
                ? new BooleanDecisionTree(value)
                : new BinaryDecisionTree(value);

            return node;
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            var test = BuildBooleanDecisionTree(node.Test);
            var ifTrue = BuildBinaryDecisionTree(node.IfTrue);
            var ifFalse = BuildBinaryDecisionTree(node.IfFalse);

            this.bdt = test.Conditional(ifTrue, ifFalse);

            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                var operand = BuildBooleanDecisionTree(node.Operand);

                this.bdt = operand.Not();
                return node;
            }

            return base.VisitUnary(node);
        }
        
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType.IsComparison())
            {
                var expression = VisitComparison(node);
                this.bdt = BuildComparison(expression);
            }
            else if (node.NodeType.IsAnd())
            {
                var left = BuildBooleanDecisionTree(node.Left);
                var right = BuildBooleanDecisionTree(node.Right);
                this.bdt = left.And(right);
            }
            else if (node.NodeType.IsOr())
            {
                var left = BuildBooleanDecisionTree(node.Left);
                var right = BuildBooleanDecisionTree(node.Right);
                this.bdt = left.Or(right);
            }
            else
            {
                // Any other binary expression just needs to be 
                this.bdt = new BinaryDecisionTree(TrimParenthesis(node));
            }

            return node;
        }

        private static BinaryExpression VisitComparison(BinaryExpression expression)
        {
            if (expression.Left.NodeType == ExpressionType.Convert
             && expression.Right.NodeType == ExpressionType.Constant)
            {
                return BuildEnumComparison(expression.NodeType, expression.Left as UnaryExpression, expression.Right as ConstantExpression);
            }

            if (expression.Right.NodeType == ExpressionType.Convert
             && expression.Left.NodeType == ExpressionType.Constant)
            {
                return BuildEnumComparison(expression.NodeType, expression.Right as UnaryExpression, expression.Left as ConstantExpression);
            }

            return expression;
        }

        private static BinaryExpression BuildEnumComparison(ExpressionType nodeType, UnaryExpression unary, ConstantExpression constant)
        {
            // By default an enum comparison will use its underlying type:   (int)x.Value == 1
            // We want the comparison to be based on the enum type directly:  x.Value == One

            var left = unary.Operand;
            var value = constant.Value.ToString();
            var right = Expression.Constant(Enum.Parse(left.Type, value));
            return Expression.MakeBinary(nodeType, left, right);
        }

        private static BinaryDecisionTree BuildComparison(BinaryExpression expression)
        {
            var left = BuildBinaryDecisionTree(expression.Left);
            var right = BuildBinaryDecisionTree(expression.Right);

            return left.Compare(expression.NodeType, right);
        }
        
        private static string TrimParenthesis(Expression expression)
        {
            var value = expression.ToString();

            return value.StartsWith("(") && value.EndsWith(")")
                 ? value.Substring(1, value.Length - 2)
                 : value;
        }

        private static bool IsNullableMember(MemberExpression expression)
        {
            var declaringType = expression.Member.DeclaringType;

            return declaringType != null
                && declaringType.IsGenericType
                && declaringType.GetGenericTypeDefinition() == typeof(Nullable<>)
                && declaringType.GetGenericArguments()[0] == expression.Type;
        }
    }
}
