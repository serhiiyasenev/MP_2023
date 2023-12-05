using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
    public class IncDecExpressionVisitor : ExpressionVisitor
    {
        private readonly Dictionary<string, object> _parametersReplacement;

        public IncDecExpressionVisitor(Dictionary<string, object> parametersReplacement)
        {
            _parametersReplacement = parametersReplacement;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if ((node.NodeType == ExpressionType.Add || node.NodeType == ExpressionType.Subtract) &&
                node.Right is ConstantExpression rightConst)
            {
                if (node.Left is ParameterExpression leftParam && rightConst.Value.Equals(1))
                {
                    switch (node.NodeType)
                    {
                        case ExpressionType.Add:
                            return Expression.Increment(leftParam);
                        case ExpressionType.Subtract:
                            return Expression.Decrement(leftParam);
                    }
                }
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_parametersReplacement.TryGetValue(node.Name, out var value))
            {
                return Expression.Constant(value, node.Type);
            }

            return base.VisitParameter(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var newParameters = new List<ParameterExpression>();
            foreach (var param in node.Parameters)
            {
                if (_parametersReplacement.ContainsKey(param.Name))
                {
                    newParameters.Add(Expression.Parameter(param.Type, param.Name));
                }
                else
                {
                    newParameters.Add(param);
                }
            }

            var newBody = Visit(node.Body);
            return Expression.Lambda(newBody, newParameters);
        }
    }
}
