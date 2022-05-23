using System;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using ParsedClassData = OCompiler.Analyze.Syntax.Declaration.Class.Class;
using ParsedConstructorData = OCompiler.Analyze.Syntax.Declaration.Class.Member.Constructor;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;
using SyntaxExpression = OCompiler.Analyze.Syntax.Declaration.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2;

internal partial class AnnotatedSyntaxTreeV2
{
    private DomExpression ParseLValueExpression(SyntaxExpression expression)
    {
        return new ThisReferenceExpression(); // TODO
    }
    
    private DomExpression ParseRValueExpression(SyntaxExpression expression)
    {
        return expression switch
        {
            DictDefinition dictDefinition => throw new NotImplementedException(),
            Call call => throw new NotImplementedException(),
            ListDefinition listDefinition => throw new NotImplementedException(),
            SimpleExpression simpleExpression => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }
}