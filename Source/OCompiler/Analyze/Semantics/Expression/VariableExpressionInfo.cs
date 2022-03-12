using System.Collections.Generic;

using OCompiler.Analyze.Syntax.Declaration;

namespace OCompiler.Analyze.Semantics.Expression
{
    internal class VariableExpressionInfo : MethodExpressionInfo
    {
        public Variable TargetVariable { get; }

        public VariableExpressionInfo(
            Syntax.Declaration.Expression.Expression expression,
            Variable targetVariable,
            Dictionary<string, string?>? locals = null
        ) : base(expression, locals)
        {
            TargetVariable = targetVariable;
        }

        public new VariableExpressionInfo FromSameContext(Syntax.Declaration.Expression.Expression expression)
        {
            return new(expression, TargetVariable, LocalVariables);
        }
    }
}
