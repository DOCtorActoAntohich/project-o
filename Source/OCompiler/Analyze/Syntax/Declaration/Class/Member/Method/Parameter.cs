using System;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Exceptions;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

internal class Parameter
{
    public Identifier Name { get; }
    public TypeAnnotation Type { get; }

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
            throw new SyntaxError(tokens.Current().Position, "Expected ':'");
        }
        tokens.Next();

        // Type.
        if (!TypeAnnotation.TryParse(tokens, out var type))
        {
            throw new SyntaxError(tokens.Current().Position, "Expected class name");
        }
        
        parameter = new Parameter(name, type!);
        return true;
    }

    private Parameter(Identifier name, TypeAnnotation type)
    {
        Name = name;
        Type = type;
    }

    public override string ToString()
    {
        return $"{Name.Literal}: {Type}";
    }
}
