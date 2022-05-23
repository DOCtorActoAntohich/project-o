using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

internal class MethodCallExpression : CallExpression
{
    public Expression? ObjectReference { get; set; }


    public MethodCallExpression(Expression? @object, string name) : base(name)
    {
        ObjectReference = @object;
    }

    public MethodCallExpression(
        Expression? @object, 
        string name, 
        IEnumerable<Expression> arguments) 
        : this(@object, name)
    {
        Arguments.AddRange(arguments);
    }
}