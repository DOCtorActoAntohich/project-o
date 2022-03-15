using System.Collections;
using System.Collections.Generic;
using System.Text;

using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration;

internal class Body : IEnumerable<IBodyStatement>
{
    private readonly List<IBodyStatement> _members = new();

    public Body(TokenEnumerator tokens)
    {
        while (IBodyStatement.TryParse(tokens, out IBodyStatement? bodyStatement))
        {
            _members.Add(bodyStatement!);
        }
    }

    public Body()
    {

    }

    public void AddTrailingReturn()
    {
        if (_members[^1] is not Return)
        {
            _members.Add(Return.EmptyReturn);
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

    public IEnumerator<IBodyStatement> GetEnumerator()
    {
        return _members.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_members).GetEnumerator();
    }
}
