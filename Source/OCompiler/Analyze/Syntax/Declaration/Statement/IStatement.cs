using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Statement;

internal interface IStatement : IBodyStatement
{
    public static bool TryParse(TokenEnumerator tokens, out IStatement? statement)
    {
        if (Assignment.TryParse(tokens, out Assignment? field))
        {
            statement = field;
            return true;
        }
        
        if (If.TryParse(tokens, out If? @if))
        {
            statement = @if;
            return true;
        }
        
        if (While.TryParse(tokens, out While? @while))
        {
            statement = @while;
            return true;
        }
        
        if (Return.TryParse(tokens, out Return? @return))
        {
            statement = @return;
            return true;
        }
        
        statement = null;
        return false;
    }

    public new abstract string ToString(string prefix);
}
