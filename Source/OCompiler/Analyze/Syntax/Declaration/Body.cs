using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration;

internal static class Body
{
    public static Boolean TryParse(TokenEnumerator tokens, out List<INestable>? body)
    {
        body = new List<INestable>();

        while (true)
        {
            if (Variable.TryParse(tokens, out Variable? variable))
            {
                body.Add(variable!);
                continue;
            }
            
            if (Statement.Statement.TryParse(tokens, out Statement.Statement? statement))
            {
                body.Add(statement!);
                continue;
            }

            if (Expression.Expression.TryParse(tokens, out Expression.Expression? expression))
            {
                body.Add(expression!);
                continue;
            }

            return true;
        }
    }

    public static String ToString(List<INestable>? body, String prefix)
    {
        if (body is null)
        {
            return "";
        }

        StringBuilder @string = new StringBuilder();
        for (Int32 i = 0; i < body.Count; ++i)
        {
            @string.Append(prefix);
            
            if (i + 1 == body.Count)
            {
                @string.Append("└── ");
                @string.Append(body[i].ToString(prefix + "    "));
                break;
            }
            
            @string.Append("├── ");
            @string.AppendLine(body[i].ToString(prefix + "│   "));
        }

        return @string.ToString();
    }
}
