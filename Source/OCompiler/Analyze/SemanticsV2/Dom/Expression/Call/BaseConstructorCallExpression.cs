using System.Collections.Generic;
using BaseToken = OCompiler.Analyze.Lexical.Tokens.Keywords.Base;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;

internal class BaseConstructorCallExpression : CallExpression
{
    public BaseConstructorCallExpression() : base(BaseToken.Literal)
    {
    }

    public BaseConstructorCallExpression(IEnumerable<Expression> arguments) : this()
    {
        AddArguments(arguments);
    }
}