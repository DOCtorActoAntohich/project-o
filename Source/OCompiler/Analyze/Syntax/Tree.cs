using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Syntax.Declaration.Class;
using OCompiler.Utils;


namespace OCompiler.Analyze.Syntax;

internal class Tree
{
    private readonly List<Class> _tree = new();
    public bool IsEmpty => _tree.Count == 0;

    public Tree(TokenEnumerator tokens)
    {
        while (tokens.Current() is not Lexical.Tokens.EndOfFile)
        {
            _tree.Add(Class.Parse(tokens));
        }
    }

    public override string ToString()
    {        
        var @string = new StringBuilder();
        for (var i = 0; i < _tree.Count; ++i)
        {
            if (i + 1 == _tree.Count)
            {
                @string.Append("└── ");
                @string.Append(_tree[i].ToString("    "));
                break;
            }
            
            @string.Append("├── ");
            @string.AppendLine(_tree[i].ToString("│   "));
        }

        return @string.ToString();
    }
}
