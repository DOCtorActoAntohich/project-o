using OCompiler.Analyze.Semantics.Expression;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Exceptions;

namespace OCompiler.Analyze.Semantics;

internal class ParsedFieldInfo
{
    public Field Field { get; }
    public string Name => Field.Identifier.Literal;
    public ExpressionInfo Expression { get; }
    public string? Type => Expression.Type;
    public Context Context { get; }

    public ParsedFieldInfo(Field parsedField, Context context)
    {
        if (parsedField.Expression is null)
        {
            throw new CompilerInternalError(
                $"Field {parsedField.Identifier.Literal} is not assigned. This is not supported in the current version of the compiler."
            );
        }

        Context = context;
        Field = parsedField;
        Expression = new ExpressionInfo(parsedField.Expression, Context);
    }
}
