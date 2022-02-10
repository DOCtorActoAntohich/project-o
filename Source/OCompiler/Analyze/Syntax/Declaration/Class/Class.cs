using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;
using OCompiler.Extensions;

namespace OCompiler.Analyze.Syntax.Declaration.Class;

internal class Class
{
    public Identifier Name { get; }
    public List<Field> Fields { get; }
    public List<Method> Methods { get; }
    public List<Constructor> Constructors { get; }

    public static Boolean TryParse(IEnumerator<Token> tokens, out Class? @class)
    {
        // Class.
        if (tokens.Current() is not Lexical.Tokens.Keywords.Class)
        {
            @class = null;
            return false;
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

        List<Field> fields = new List<Field>();
        List<Method> methods = new List<Method>();
        List<Constructor> constructors = new List<Constructor>();

        // Parse members.
        while (IMember.TryParse(tokens, out IMember? member))
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
        
        @class = new Class(name, fields, methods, constructors);
        return true;
    }

    private Class(Identifier name, List<Field> fields, List<Method> methods, List<Constructor> constructors)
    {
        Name = name;
        Fields = fields;
        Methods = methods;
        Constructors = constructors;
    }
}
