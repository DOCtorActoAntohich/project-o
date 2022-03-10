using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;
using OCompiler.Utils;

namespace OCompiler.Analyze.Syntax.Declaration.Class;

internal class Class
{
    public Identifier Name { get; }
    public List<Field> Fields { get; }
    public List<Method> Methods { get; }
    public List<Constructor> Constructors { get; }

    public static Class Parse(TokenEnumerator tokens)
    {
        // Class.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Class)
        {
            throw new Exception($"Keyword 'class' expected at position {tokens.Current().StartOffset}.");
        }
    
        // Class name.
        if (tokens.Next() is not Identifier name)
        {
            throw new Exception($"Class identifier expected at position {tokens.Current().StartOffset}.");
        }
        
        // Is.
        if (tokens.Next() is not Lexical.Tokens.Keywords.Is)
        {
            throw new Exception($"Keyword 'is' expected at position {tokens.Current().StartOffset}.");
        }

        // Get next token.
        tokens.Next();

        var fields = new List<Field>();
        var methods = new List<Method>();
        var constructors = new List<Constructor>();

        // Parse members.
        while (IClassMember.TryParse(tokens, out IClassMember? member))
        {
            switch (member)
            {
                case Field field:
                    fields.Add(field);
                    continue;
                
                case Constructor constructor:
                    constructors.Add(constructor);
                    continue;
                
                case Method method:
                    methods.Add(method);
                    continue;
            }
        }
            
        // End.
        if (tokens.Current() is not Lexical.Tokens.Keywords.End)
        {
            throw new Exception($"Keyword 'end' expected at position {tokens.Current().StartOffset}.");
        }
        
        // Get next token.
        tokens.Next();

        return new Class(name, fields, methods, constructors);
    }

    private Class(Identifier name, List<Field> fields, List<Method> methods, List<Constructor> constructors)
    {
        Name = name;
        Fields = fields;
        Methods = methods;
        Constructors = constructors;
    }

    public string ToString(string prefix = "")
    {
        var @string = new StringBuilder();
        var members = new List<IClassMember>();

        members.AddRange(Fields);
        members.AddRange(Constructors);
        members.AddRange(Methods);
        
        // Name.
        @string.AppendLine(Name.Literal);
        
        for (var i = 0; i < members.Count; ++i)
        {
            @string.Append(prefix);
            
            if (i + 1 == members.Count)
            {
                @string.Append("└── ");
                @string.Append(members[i].ToString(prefix + "    "));
                break;
            }
            
            @string.Append("├── ");
            @string.AppendLine(members[i].ToString(prefix + "│   "));
        }

        return @string.ToString();
    }
}
