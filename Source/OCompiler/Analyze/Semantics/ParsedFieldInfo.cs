using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Expression;

namespace OCompiler.Analyze.Semantics;

internal class ParsedFieldInfo
{
    public Field Field { get; }
    public string Name => Field.Identifier.Literal;
    public Expression Expression => Field.Expression;

    public ParsedFieldInfo(Field parsedField)
    {
        Field = parsedField;
    }
}
