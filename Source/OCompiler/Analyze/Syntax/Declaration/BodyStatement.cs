using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration;

internal abstract class BodyStatement
{
    public abstract string ToString(string prefix);

    public static bool TryParse(TokenEnumerator tokens, out BodyStatement? bodyStatement)
    {
        if (Variable.TryParse(tokens, out Variable? variable))
        {
            bodyStatement = variable!;
            return true;
        }
        if (Statement.Statement.TryParse(tokens, out Statement.Statement? statement))
        {
            bodyStatement = statement!;
            return true;
        }
        if (Expression.Expression.TryParse(tokens, out Expression.Expression? expression))
        {
            bodyStatement = expression!;
            return true;
        }

        bodyStatement = null;
        return false;
    }
}
