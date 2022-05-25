using System.Collections.Generic;
using System.Text;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;

internal abstract class CallExpression : Expression, ICanHaveArguments
{
    public List<Expression> Arguments { get; } = new();

    public override DomStatement ParentStatement
    {
        get => base.ParentStatement;
        set
        {
            base.ParentStatement = value;
            foreach (var argument in Arguments)
            {
                argument.ParentStatement = value;
            }
        }
    }

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

    public override string ToString()
    {
        return new StringBuilder(Name)
            .Append('(')
            .Append(string.Join(", ", Arguments))
            .Append(')')
            .ToString();
    }
}