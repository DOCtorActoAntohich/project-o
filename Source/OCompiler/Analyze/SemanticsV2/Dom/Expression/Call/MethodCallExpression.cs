using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;

internal class MethodCallExpression : CallExpression
{
    public Expression SourceObject { get; set; }


    public MethodCallExpression(Expression sourceObject, string name) : base(name)
    {
        SourceObject = sourceObject;
    }

    public MethodCallExpression(
        Expression sourceObject, 
        string name, 
        IEnumerable<Expression> arguments) 
        : this(sourceObject, name)
    {
        Arguments.AddRange(arguments);
    }

    public override string ToString()
    {
        return $"{SourceObject}.{base.ToString()}";
    }
}