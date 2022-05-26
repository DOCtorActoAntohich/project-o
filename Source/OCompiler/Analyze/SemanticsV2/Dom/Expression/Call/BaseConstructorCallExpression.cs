using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using BaseToken = OCompiler.Analyze.Lexical.Tokens.Keywords.Base;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;

internal class BaseConstructorCallExpression : CallExpression
{
    public MemberConstructor Constructor { get; set; } = null!;
    
    public BaseConstructorCallExpression() : base(BaseToken.Literal)
    {
    }

    public BaseConstructorCallExpression(IEnumerable<Expression> arguments) : this()
    {
        AddArguments(arguments);
    }
}