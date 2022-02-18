using System;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member;



internal interface IMember
{
    public static Boolean TryParse(TokenEnumerator tokens, out IMember? member)
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

    public String ToString(String prefix);
}
