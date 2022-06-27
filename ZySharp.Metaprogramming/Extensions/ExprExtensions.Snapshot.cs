﻿using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

using ZySharp.Metaprogramming.Internal;

namespace ZySharp.Metaprogramming.Extensions
{
    public static partial class ExprExtensions
    {
        /// <summary>
        /// Replaces all captured value-type variables with a constant snapshot of their current values.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the lambda expression delegate.</typeparam>
        /// <param name="expression">The input expression tree.</param>
        /// <returns>The transformed expression tree.</returns>
        /// <remarks>
        /// Consider using <see cref="Expr.Capture{T}"/> and <see cref="Expr.Expand{TDelegate}"/> to only take
        /// snapshots of certain values.
        /// </remarks>
        [Pure]
        public static Expression<TDelegate>? Snapshot<TDelegate>(this Expression<TDelegate>? expression)
        {
            return Snapshot((Expression?)expression) as Expression<TDelegate>;
        }

        /// <inheritdoc cref="Snapshot{TDelegate}(Expression{TDelegate}?)"/>
        [Pure]
        public static Expression? Snapshot(this Expression? expression)
        {
            return new SnapshotVisitor().Visit(expression);
        }

        #region Visitor

        private sealed class SnapshotVisitor :
            ExpressionVisitor
        {
            protected override Expression VisitMember(MemberExpression node)
            {
                if (!node.IsClosureMember())
                {
                    return base.VisitMember(node);
                }

                var closure = (ConstantExpression)node.Expression!;
                var field = (FieldInfo)node.Member;

                return Expression.Constant(field.GetValue(closure.Value), field.FieldType);
            }
        }

        #endregion Visitor
    }
}