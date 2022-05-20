using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Lexical.Tokens.BooleanLiterals;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Exceptions;

using Boolean = OCompiler.Builtins.Primitives.Boolean;
using Integer = OCompiler.Builtins.Primitives.Integer;
using Real    = OCompiler.Builtins.Primitives.Real;
using String  = OCompiler.Builtins.Primitives.String;

namespace OCompiler.Pipeline;

internal class Invoker
{
    public Type TargetClass { get; }
    public object[] Arguments { get; }

    public Invoker(Assembly assembly, string className, string[] args)
    {
        var @class = assembly.GetType(className);
        if (@class == null)
        {
            throw new InvocationError($"Class {className} was not found in the resulting assembly.");
        }
        TargetClass = @class;
        Arguments = ParseCommandLineArgs(args).Select(pair => pair.Item2).ToArray();
    }

    public void Run()
    {
        try
        {
            Activator.CreateInstance(TargetClass, args: Arguments);
        }
        catch (Exception exception)
        {
            if (exception.InnerException is not null)
            {
                exception = exception.InnerException!;
            }
            
            Console.WriteLine($"Error: {exception.GetType().Name}(\"{exception.Message})\"");
        }
    }

    public static ParsedConstructorInfo GetEntryPoint(List<ClassInfo> allClasses, string className, string[] args)
    {
        var @class = allClasses.Where(c => c.Name == className).First();
        if (@class is not ParsedClassInfo)
        {
            throw new InvocationError("Constructor of a built class cannot be used as an entry point.");
        }

        var types = new List<string>(ParseCommandLineArgs(args).Select(pair => pair.Item1));

        var constructor = ((ParsedClassInfo)@class).GetConstructor(types);
        if (constructor == null)
        {
            var argsStr = string.Join(", ", types);
            throw new InvocationError($"Couldn't find a constructor to call: {className}({argsStr})");
        }

        return constructor;
    }

    private static List<(string, object)> ParseCommandLineArgs(string[] args)
    {
        var parsedArgs = new List<(string, object)>();
        foreach (var arg in args)
        {
            if (!Token.TryParse(new(), arg, out var token))
            {
                parsedArgs.Add(("String", arg));
                continue;
            }
            parsedArgs.Add(token switch
            {
                BooleanLiteral boolean => ("Boolean", new Boolean(boolean is True)),
                RealLiteral real       => ("Real",    new Real(real.Value)),
                IntegerLiteral integer => ("Integer", new Integer(integer.Value)),
                _                      => ("String",  new String(token.Literal))
            });
        }
        return parsedArgs;
    }
}
