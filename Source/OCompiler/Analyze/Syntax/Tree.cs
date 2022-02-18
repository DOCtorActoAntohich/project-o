using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Syntax.Declaration.Class;
using OCompiler.Utils;


namespace OCompiler.Analyze.Syntax;

internal static class Tree
{
    public static Boolean TryParse(TokenEnumerator tokens, out List<Class>? tree)
    {
        tree = new List<Class>();

        while (Class.TryParse(tokens, out Class? @class))
        {
            tree.Add(@class!);
        }

        return true;
    }

    public static String ToString(List<Class>? tree)
    {
        if (tree is null)
        {
            return "";
        }
        
        StringBuilder @string = new StringBuilder();
        for (Int32 i = 0; i < tree.Count; ++i)
        {
            if (i + 1 == tree.Count)
            {
                @string.Append("└── ");
                @string.Append(tree[i].ToString("    "));
                break;
            }
            
            @string.Append("├── ");
            @string.AppendLine(tree[i].ToString("│   "));
        }

        return @string.ToString();
    }
}
