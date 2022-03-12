﻿using System;
using System.Collections.Generic;
using System.Linq;

using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Semantics.Expression;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;

namespace OCompiler.Analyze.Semantics.Callable;

internal abstract class CallableInfo
{
    public IClassMember Callable { get; }
    public Body Body { get; }
    public List<ParsedParameterInfo> Parameters { get; } = new();
    public Dictionary<string, ExpressionInfo> LocalVariables { get; } = new();
    public Context Context { get; }

    public CallableInfo(IClassMember parsedMember, Context context)
    {
        Context = context;
        switch (parsedMember)
        {
            case Constructor constructor:
                Callable = constructor;
                Body = constructor.Body;
                AddParameters(constructor.Parameters);
                AddLocalVariables();
                break;
            case Method method:
                Callable = method;
                Body = method.Body;
                AddParameters(method.Parameters);
                AddLocalVariables();
                break;
            default:
                throw new Exception("Attempt to CallableInfo not with constructor or method.");
        }
    }

    private void AddParameters(List<Parameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            var paramInfo = new ParsedParameterInfo(parameter);
            if (Parameters.Where(p => p.Name == paramInfo.Name).Any())
            {
                throw new Exception($"Parameter name {paramInfo.Name} is a duplicate");
            }
            Parameters.Add(paramInfo);
        }
    }

    private void AddLocalVariables()
    {
        foreach (var statement in Body)
        {
            if (statement is Variable variable)
            {
                LocalVariables.Add(variable.Identifier.Literal, new ExpressionInfo(variable.Expression, Context));
            }
        }
    }

    public string? GetParameterType(string name)
    {
        return Parameters.Where(p => p.Name == name).FirstOrDefault()?.Type;
    }

    public List<string> GetParameterTypes()
    {
        return Parameters.Select(p => p.Type).ToList();
    }
}