﻿using OCompiler.Analyze.Semantics.Expression;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;

namespace OCompiler.Analyze.Semantics;

internal class ParsedFieldInfo
{
    public Field Field { get; }
    public string Name => Field.Identifier.Literal;
    public ExpressionInfo Expression { get; }
    public string? Type { get; set; }

    public ParsedFieldInfo(Field parsedField)
    {
        Field = parsedField;
        Expression = new ExpressionInfo(parsedField.Expression);
    }
}
