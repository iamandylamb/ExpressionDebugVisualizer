namespace BinaryDecisionTree.Visitors
{
    using System.Linq.Expressions;

    /// <summary>
    /// Reduces 'Not' expressions within an expression.
    /// </summary>
    public class NotExpressionReducer : ExpressionVisitor
    {
        protected bool IsInNotContext { get; set; }

        protected NotExpressionReducer Not
        {
            get { return new NotExpressionReducer { IsInNotContext = true }; }
        }

        protected NotExpressionReducer NotNot
        {
            // Nested not's jump in then immediately out of a not context.
            get { return new NotExpressionReducer { IsInNotContext = false }; }
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return node.NodeType != ExpressionType.Not
                       ? base.VisitUnary(node)
                       : node.Operand.NodeType != ExpressionType.Not
                             ? Not.Visit(node.Operand)
                             : NotNot.Visit(((UnaryExpression)node.Operand).Operand);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (!IsInNotContext)
            {
                return base.VisitBinary(node);
            }

            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    return base.VisitBinary(Expression.NotEqual(node.Left, node.Right));
                case ExpressionType.NotEqual:
                    return base.VisitBinary(Expression.Equal(node.Left, node.Right));
                case ExpressionType.GreaterThan:
                    return base.VisitBinary(Expression.LessThanOrEqual(node.Left, node.Right));
                case ExpressionType.GreaterThanOrEqual:
                    return base.VisitBinary(Expression.LessThan(node.Left, node.Right));
                case ExpressionType.LessThan:
                    return base.VisitBinary(Expression.GreaterThanOrEqual(node.Left, node.Right));
                case ExpressionType.LessThanOrEqual:
                    return base.VisitBinary(Expression.GreaterThan(node.Left, node.Right));

                case ExpressionType.And:
                    return base.VisitBinary(Expression.Or(Expression.Not(node.Left), Expression.Not(node.Right)));
                case ExpressionType.AndAlso:
                    return base.VisitBinary(Expression.OrElse(Expression.Not(node.Left), Expression.Not(node.Right)));
                case ExpressionType.Or:
                    return base.VisitBinary(Expression.And(Expression.Not(node.Left), Expression.Not(node.Right)));
                case ExpressionType.OrElse:
                    return base.VisitBinary(Expression.AndAlso(Expression.Not(node.Left), Expression.Not(node.Right)));
                default:
                    return base.VisitBinary(node);
            }
        }
    }
}
