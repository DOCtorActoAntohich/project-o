using System.Collections.Generic;

using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Semantics.ClassInfo;

namespace OCompiler.Analyze.Semantics;

internal class ExpressionInfo
{
    public Expression Expression { get; }
    public ParsedClassInfo Class { get; }
    public IClassMember? Method { get; }
    public string? TargetVariable { get; }
    public Dictionary<string, string?> LocalVariables { get; }

    public ExpressionInfo(Expression expression, ParsedClassInfo classInfo, IClassMember? method, string? targetVariable, Dictionary<string, string?> locals)
    {
        Expression = expression;
        Class = classInfo;
        Method = method;
        TargetVariable = targetVariable;
        LocalVariables = locals;
    }

    public ExpressionInfo(Expression expression, ParsedClassInfo classInfo, string? targetVariable)
    {
        Expression = expression;
        Class = classInfo;
        TargetVariable = targetVariable;
        LocalVariables = new();
    }

    public ExpressionInfo? GetChildInfo()
    {
        if (Expression.Child == null)
        {
            return null;
        }
        return new(Expression.Child, Class, Method, null, LocalVariables);
    }

    public ExpressionInfo FromSameContext(Expression newExpression)
    {
        return new(newExpression, Class, Method, null, LocalVariables);
    }
}
