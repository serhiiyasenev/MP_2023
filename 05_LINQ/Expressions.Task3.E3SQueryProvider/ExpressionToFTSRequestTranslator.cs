using System;
using System.Linq.Expressions;
using System.Text;

namespace Expressions.Task3.E3SQueryProvider
{
    public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        readonly StringBuilder _resultStringBuilder;

        public ExpressionToFtsRequestTranslator()
        {
            _resultStringBuilder = new StringBuilder();
        }

        public string Translate(Expression exp)
        {
            Visit(exp);

            return _resultStringBuilder.ToString();
        }

        #region protected methods

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(string))
            {
                var method = node.Method.Name;
                var obj = (MemberExpression)node.Object;
                var arg = (ConstantExpression)node.Arguments[0];

                Visit(obj);
                _resultStringBuilder.Append("(");

                switch (method)
                {
                    case "StartsWith":
                        _resultStringBuilder.Append(arg.Value + "*");
                        break;
                    case "EndsWith":
                        _resultStringBuilder.Append("*" + arg.Value);
                        break;
                    case "Contains":
                        _resultStringBuilder.Append("*" + arg.Value + "*");
                        break;
                    case "Equals":
                        _resultStringBuilder.Append(arg.Value);
                        break;
                    default:
                        throw new NotSupportedException($"Method '{method}' is not supported");
                }

                _resultStringBuilder.Append(")");
                return node;
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Equal)
            {
                MemberExpression memberExpr = null;
                ConstantExpression constExpr = null;

                // Select MemberExpression and ConstantExpression
                if (node.Left.NodeType == ExpressionType.MemberAccess && node.Right.NodeType == ExpressionType.Constant)
                {
                    memberExpr = (MemberExpression)node.Left;
                    constExpr = (ConstantExpression)node.Right;
                }
                else if (node.Left.NodeType == ExpressionType.Constant && node.Right.NodeType == ExpressionType.MemberAccess)
                {
                    memberExpr = (MemberExpression)node.Right;
                    constExpr = (ConstantExpression)node.Left;
                }

                if (memberExpr == null || constExpr == null)
                    throw new NotSupportedException("One of the operands should be constant and other should be property or field");

                Visit(memberExpr);
                _resultStringBuilder.Append("(");
                Visit(constExpr);
                _resultStringBuilder.Append(")");
            }
            else if (node.NodeType == ExpressionType.AndAlso)
            {
                Visit(node.Left);
                _resultStringBuilder.Append(" AND ");
                Visit(node.Right);
            }
            else
            {
                throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
            }

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _resultStringBuilder.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _resultStringBuilder.Append(node.Value);

            return node;
        }

        #endregion
    }
}
