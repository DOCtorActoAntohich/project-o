using System;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

internal class Parameter
{
    public Identifier Name { get; }
    public Identifier Type { get; }

    public static bool TryParse(TokenEnumerator tokens, out Parameter? parameter)
    {
        // Name.
        if (tokens.Current() is not Identifier name)
        {
            parameter = null;
            return false;
        }
        
        // Colon.
        if (tokens.Next() is not Lexical.Tokens.Delimiters.Colon)
        {
            throw new Exception($"Expected ':' at line {tokens.Current().Position.Line}.");
        }
        
        // Type.
        if (tokens.Next() is not Identifier type)
        {
            throw new Exception($"Expected class name at line {tokens.Current().Position.Line}.");
        }
        
        // Get next token.
        tokens.Next();
        
        parameter = new Parameter(name, type);
        return true;
    }

    private Parameter(Identifier name, Identifier type)
    {
        Name = name;
        Type = type;
    }

    public override string ToString()
    {
        return $"{Name.Literal}: {Type.Literal}";
    }
}
