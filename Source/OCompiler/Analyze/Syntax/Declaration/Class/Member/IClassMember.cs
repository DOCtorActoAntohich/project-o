using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member;

internal interface IClassMember
{
    public static bool TryParse(TokenEnumerator tokens, out IClassMember? member)
    {
        if (Field.TryParse(tokens, out Field? field))
        {
            member = field;
            return true;
        }
        
        if (Method.Method.TryParse(tokens, out Method.Method? method))
        {
            member = method;
            return true;
        }
        
        if (Constructor.TryParse(tokens, out Constructor? constructor))
        {
            member = constructor;
            return true;
        }

        member = null;
        return false;
    }

    public string ToString(string prefix);
}
