using System;
using System.Linq.Expressions;

namespace KLog.Api.Extensions
{
    public static class ExpressionExtensions
    {
        // https://stackoverflow.com/questions/52190020/net-core-combine-a-list-of-func-with-or-to-a-single-func
        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2
        )
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T));

            ReplaceExpressionVisitor leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            Expression left = leftVisitor.Visit(expr1.Body);

            ReplaceExpressionVisitor rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            Expression right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
}
