using System.Collections.Generic;

using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Lexical.Tokens.Delimiters;
using OCompiler.Exceptions;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration;

internal class TypeAnnotation
{
    public Identifier Name { get; }
    public List<Identifier> GenericTypes { get; } = new();

    public TypeAnnotation(Identifier name)
    {
        Name = name;
    }

    public TypeAnnotation(Identifier name, IEnumerable<Identifier> generics)
    {
        Name = name;
        GenericTypes = new(generics);
    }

    public static bool TryParse(TokenEnumerator tokens, out TypeAnnotation? type)
    {
        if (tokens.Current() is not Identifier name)
        {
            type = null;
            return false;
        }

        var genericTypes = new List<Identifier>();
        if (tokens.Next() is LeftAngleBracket)
        {
            ParseGenericsList(tokens);
        }

        type = new TypeAnnotation(name, genericTypes);
        return true;
    }

    private static List<Identifier> ParseGenericsList(TokenEnumerator tokens)
    {
        var types = new List<Identifier>();

        while (tokens.Current() is not RightAngleBracket)
        {
            if (tokens.Next() is not Identifier type)
            {
                throw new SyntaxError(tokens.Current().Position, "Expected a type in the generics list");
            }
            types.Add(type);

            if (tokens.Next() is not (Comma or RightAngleBracket))
            {
                throw new SyntaxError(tokens.Current().Position, "Expected a comma or end of generics list");
            }
        }

        return types;
    }
}
