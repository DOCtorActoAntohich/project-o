using System;
using System.Collections.Generic;
using System.Linq;

using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;

namespace OCompiler.Pipeline;

internal class Invoker
{
    public static ParsedConstructorInfo GetEntryPoint(List<ClassInfo> allClasses, string className, string[] cmdArgs)
    {
        var @class = allClasses.Where(c => c.Name == className).First();
        if (@class is not ParsedClassInfo)
        {
            throw new Exception("Constructor of a built class cannot be used as an entry point.");
        }

        var types = new List<string>();
        foreach (var arg in cmdArgs)
        {
            if (Token.TryParse(0, arg, out var token))
            {
                types.Add(token switch
                {
                    BooleanLiteral => "Boolean",
                    RealLiteral => "Real",
                    IntegerLiteral => "Integer",
                    _ => "String"
                });
                continue;
            }
            types.Add("String");
        }

        var constructor = ((ParsedClassInfo)@class).GetConstructor(types);
        if (constructor == null)
        {
            var argsStr = string.Join(", ", types);
            throw new Exception($"Couldn't find a constructor to call: {className}({argsStr})");
        }

        return constructor;
    }
}
