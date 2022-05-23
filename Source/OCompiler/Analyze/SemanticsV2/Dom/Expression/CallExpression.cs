using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal abstract class CallExpression : Expression, ICanHaveArguments
{
    public List<Expression> Arguments { get; } = new();

    protected CallExpression(string name) : base(name)
    {
    }

    public void AddArgument(Expression argument)
    {
        Arguments.Add(argument);
    }
    
    public void AddArguments(IEnumerable<Expression> arguments)
    {
        foreach (var argument in arguments)
        {
            AddArgument(argument);
        }
    }
}