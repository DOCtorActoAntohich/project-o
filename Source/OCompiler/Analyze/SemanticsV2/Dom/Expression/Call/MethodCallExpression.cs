using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;

internal class MethodCallExpression : CallExpression
{
    public Expression SourceObject { get; set; }

    public MemberMethod Method { get; set; } = null!;

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