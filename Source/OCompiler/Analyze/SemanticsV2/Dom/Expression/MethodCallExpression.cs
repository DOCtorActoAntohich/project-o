using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal class MethodCallExpression : Expression, ICanHaveArguments
{
    public List<Expression> Arguments { get; } = new();
    
    
    public MethodCallExpression(string name) : base(name)
    {
    }

    public MethodCallExpression(string name, IEnumerable<Expression> arguments) : base(name)
    {
        Arguments.AddRange(arguments);
    }
}