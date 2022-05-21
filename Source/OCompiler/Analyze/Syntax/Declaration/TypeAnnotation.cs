using System.Collections.Generic;

using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Lexical.Tokens.Delimiters;
using OCompiler.Exceptions;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration;

internal class TypeAnnotation
{
    public Identifier Name { get; }
    public List<TypeAnnotation> GenericTypes { get; } = new();

    public TypeAnnotation(Identifier name)
    {
        Name = name;
    }

    public TypeAnnotation(Identifier name, IEnumerable<TypeAnnotation> generics)
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

        var genericTypes = new List<TypeAnnotation>();
        if (tokens.Next() is LeftAngleBracket)
        {
            genericTypes = ParseGenericsList(tokens);
        }

        type = new TypeAnnotation(name, genericTypes);
        return true;
    }

    private static List<TypeAnnotation> ParseGenericsList(TokenEnumerator tokens)
    {
        // Skip the left brace
        tokens.Next();

        var types = new List<TypeAnnotation>();

        while (tokens.Current() is not RightAngleBracket)
        {
            if (!TryParse(tokens, out var type))
            {
                throw new SyntaxError(tokens.Current().Position, "Expected a type in the generics list");
            }
            types.Add(type!);

            switch (tokens.Current())
            {
                case Comma:
                    tokens.Next();
                    break;
                case not RightAngleBracket:
                    throw new SyntaxError(tokens.Current().Position, "Expected a comma or end of generics list");
            }
        }

        tokens.Next();
        return types;
    }

    public override string ToString()
    {
        string @string = Name.Literal;

        if (GenericTypes.Count > 0)
        {
            @string += $"<{string.Join(", ", GenericTypes)}>";
        }

        return @string;
    }
}
