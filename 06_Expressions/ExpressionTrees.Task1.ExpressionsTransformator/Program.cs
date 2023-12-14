using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Expression Visitor for increment/decrement.");
            Console.WriteLine();

            Expression<Func<int, int>> exprInc = x => x + 1;
            Expression<Func<int, int>> exprDec = x => x - 1;
            var parametersReplacement = new Dictionary<string, object> { { "k", 5 } };

            var visitor = new IncDecExpressionVisitor(parametersReplacement);
            var modifiedExprInc = (Expression<Func<int, int>>)visitor.Visit(exprInc);
            var modifiedExprDec = (Expression<Func<int, int>>)visitor.Visit(exprDec);

            Console.WriteLine("Original ExpressionInc: " + exprInc);
            Console.WriteLine("Modified ExpressionInc: " + modifiedExprInc);
            Console.WriteLine();
            Console.WriteLine("Original ExpressionDec: " + exprDec);
            Console.WriteLine("Modified ExpressionDec: " + modifiedExprDec);

            Console.ReadLine();
        }
    }
}
