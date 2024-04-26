namespace BinaryDecisionTree.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    public static class ExpressionExtensions
    {
        /// <summary>
        /// Logical 'OR' of two expressions.
        /// </summary>
        /// <typeparam name="T">Expression target.</typeparam>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>A combined expression.</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var arg = Expression.Parameter(typeof(T), "obj");

            return Expression.Lambda<Func<T, bool>>(
                    Expression.OrElse(Expression.Invoke(left, arg), Expression.Invoke(right, arg)),
                    arg);
        }

        /// <summary>
        /// Logical 'AND' of two expressions.
        /// </summary>
        /// <typeparam name="T">Expression target.</typeparam>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>A combined expression.</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var arg = Expression.Parameter(typeof(T), "obj");

            return Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(Expression.Invoke(left, arg), Expression.Invoke(right, arg)), arg);
        }

        /// <summary>
        /// Logical 'NOT' of an expression.
        /// </summary>
        /// <typeparam name="T">Expression target.</typeparam>
        /// <param name="expr">The expression.</param>
        /// <returns>A negated expression.</returns>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
        {
            var arg = Expression.Parameter(typeof(T), "obj");

            return Expression.Lambda<Func<T, bool>>(
                    Expression.Not(Expression.Invoke(expr, arg)), arg);
        }
    }
}
