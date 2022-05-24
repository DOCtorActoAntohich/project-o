using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;

internal class ObjectCreateExpression : CallExpression
{
    public ObjectCreateExpression(string name) : base(name)
    {
    }

    public ObjectCreateExpression(string name, IEnumerable<Expression> arguments) : this(name)
    {
        AddArguments(arguments);
    }
}