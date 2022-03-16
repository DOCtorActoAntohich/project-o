using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;
using OCompiler.Utils;
using OCompiler.Exceptions;

namespace OCompiler.Analyze.Syntax.Declaration.Class;

internal class Class
{
    public Identifier Name { get; }
    public Identifier? Extends { get; }
    public List<Field> Fields { get; }
    public List<Method> Methods { get; }
    public List<Constructor> Constructors { get; }

    public static Class Parse(TokenEnumerator tokens)
    {
        // Class.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Class)
        {
            throw new SyntaxError(tokens.Current().Position, $"Expected 'class' keyword");
        }
    
        // Class name.
        if (tokens.Next() is not Identifier name)
        {
            throw new SyntaxError(tokens.Current().Position, $"Class identifier expected at line {tokens.Current().Position.Line}.");
        }
        
        // Extends.
        Identifier? extends = null;
        if (tokens.Next() is Lexical.Tokens.Keywords.Extends)
        {
            if (tokens.Next() is not Identifier)
            {
                throw new SyntaxError(tokens.Current().Position, $"Class identifier expected at line {tokens.Current().Position.Line}.");
            }
            
            // Save.
            extends = (Identifier)tokens.Current();
            
            // Move forward.
            tokens.Next();
        }
        
        // Is.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Is)
        {
            throw new SyntaxError(tokens.Current().Position, $"Expected 'is' keyword");
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
            throw new SyntaxError(tokens.Current().Position, $"Expected 'end' keyword");
        }
        
        // Get next token.
        tokens.Next();

        return new Class(name, fields, methods, constructors, extends);
    }

    private Class(
        Identifier name, 
        List<Field> fields, 
        List<Method> methods, 
        List<Constructor> constructors, 
        Identifier? extends = null
        )
    {
        Name = name;
        Fields = fields;
        Methods = methods;
        Constructors = constructors;
        Extends = extends;
    }

    public string ToString(string prefix = "")
    {
        var @string = new StringBuilder();
        var members = new List<IClassMember>();

        members.AddRange(Fields);
        members.AddRange(Constructors);
        members.AddRange(Methods);
        
        // Name.
        @string.AppendLine(Extends is null ? Name.Literal : $"{Name.Literal} extends {Extends.Literal}");

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
