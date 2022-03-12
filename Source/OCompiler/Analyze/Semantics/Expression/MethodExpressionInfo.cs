using System.Collections.Generic;

namespace OCompiler.Analyze.Semantics.Expression
{
    internal class MethodExpressionInfo : ExpressionInfo
    {
        public Dictionary<string, string?> LocalVariables { get; } = new();

        public MethodExpressionInfo(
            Syntax.Declaration.Expression.Expression expression,
            Dictionary<string, string?>? locals = null
        ) : base(expression)
        {
            if (locals != null)
            {
                LocalVariables = new(locals);
            }
        }

        public new MethodExpressionInfo FromSameContext(Syntax.Declaration.Expression.Expression expression)
        {
            return new(expression, LocalVariables);
        }
    }
}
