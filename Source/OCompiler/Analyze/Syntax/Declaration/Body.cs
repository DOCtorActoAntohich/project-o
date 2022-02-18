using System.Collections.Generic;
using System.Text;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration;

internal class Body
{
    private readonly List<BodyStatement> _members = new();
    public bool IsEmpty => _members.Count == 0;

    public Body(TokenEnumerator tokens)
    {
        while (BodyStatement.TryParse(tokens, out BodyStatement? bodyStatement))
        {
            _members.Add(bodyStatement!);
        }
    }

    public string ToString(string prefix)
    {
        var @string = new StringBuilder();
        for (var i = 0; i < _members.Count; ++i)
        {
            @string.Append(prefix);
            
            if (i + 1 == _members.Count)
            {
                @string.Append("└── ");
                @string.Append(_members[i].ToString(prefix + "    "));
                break;
            }
            
            @string.Append("├── ");
            @string.AppendLine(_members[i].ToString(prefix + "│   "));
        }

        return @string.ToString();
    }
}
