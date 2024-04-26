namespace BinaryDecisionTree.Visitors
{
    using System.Linq.Expressions;

    /// <summary>
    /// Parameter rename visitor.
    /// </summary>
    public class ParameterRenameVisitor : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            // By matching the parameter name to the type name, the generated BDT is clearer.
            var parameter = Expression.Parameter(node.Type, node.Type.Name);

            return base.VisitParameter(parameter);
        }
    }
}
