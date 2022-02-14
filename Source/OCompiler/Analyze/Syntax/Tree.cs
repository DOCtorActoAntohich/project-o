using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax.Declaration.Class;


namespace OCompiler.Analyze.Syntax;

internal static class Tree
{
    public static Boolean TryParse(IEnumerator<Token> tokens, out List<Class>? tree)
    {
        tree = new List<Class>();

        while (Class.TryParse(tokens, out Class? @class))
        {
            tree.Add(@class!);
        }

        return true;
    }
}
