using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Syntax.Declaration.Class;
using OCompiler.Utils;


namespace OCompiler.Analyze.Syntax;

internal class Tree
{
    private readonly List<Class> _classes = new();
    public bool IsEmpty => _classes.Count == 0;

    public Tree(TokenEnumerator tokens)
    {
        while (tokens.Current() is not Lexical.Tokens.EndOfFile)
        {
            _classes.Add(Class.Parse(tokens));
        }
    }

    public override string ToString()
    {        
        var @string = new StringBuilder();
        for (var i = 0; i < _classes.Count; ++i)
        {
            if (i + 1 == _classes.Count)
            {
                @string.Append("└── ");
                @string.Append(_classes[i].ToString("    "));
                break;
            }
            
            @string.Append("├── ");
            @string.AppendLine(_classes[i].ToString("│   "));
        }

        return @string.ToString();
    }

    public IEnumerator<Class> GetEnumerator()
    {
        return _classes.GetEnumerator();
    }
}
