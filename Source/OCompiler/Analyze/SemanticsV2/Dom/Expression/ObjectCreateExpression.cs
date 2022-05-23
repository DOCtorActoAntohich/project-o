using System.Collections;
using System.Collections.Generic;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression;

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