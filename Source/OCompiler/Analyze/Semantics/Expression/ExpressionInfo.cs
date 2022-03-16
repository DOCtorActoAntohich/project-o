using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Syntax.Declaration.Expression;

using System;
using System.Collections.Generic;

namespace OCompiler.Analyze.Semantics.Expression;

internal class ExpressionInfo
{
    private string? _type;

    public Syntax.Declaration.Expression.Expression Expression { get; private set; }
    public Context Context { get; }
    public string Type { get => ValidateAndGetType(); private set => _type = value; }

    public ExpressionInfo(Syntax.Declaration.Expression.Expression expression, Context context)
    {
        Expression = expression;
        Context = context;
    }

    public ExpressionInfo? GetChildInfo()
    {
        if (Expression.Child == null)
        {
            return null;
        }
        return new(Expression.Child, Context);
    }

    public ExpressionInfo FromSameContext(Syntax.Declaration.Expression.Expression expression)
    {
        return new(expression, Context);
    }

    public void ValidateExpression()
    {
        var type = Expression.Token switch
        {
            Identifier identifier => ResolveType(identifier.Literal),
            Lexical.Tokens.Keywords.This => Context.CurrentClass.Name,
            Lexical.Tokens.Keywords.Base => ResolveBaseReference(),
            IntegerLiteral => "Integer",
            BooleanLiteral => "Boolean",
            StringLiteral => "String",
            RealLiteral => "Real",
            _ => throw new Exception($"Unexpected Primary expression: {Expression}")
        };

        var primaryClass = Context.GetClassByName(type);

        if (Expression is Call constructorCall)
        {
            // Get parameter types and find the matching constructor
            var argTypes = new List<string>();
            foreach (var arg in constructorCall.Arguments)
            {
                var argExpression = FromSameContext(arg);
                argTypes.Add(argExpression.Type);
            }
            if (argTypes.Count > 0 && !primaryClass.HasConstructor(argTypes))
            {
                throw new Exception($"Couldn't find a constructor to call: {constructorCall}");
            }
        }

        var childInfo = GetChildInfo();

        while (childInfo != null)
        {
            switch (childInfo.Expression)
            {
                case Call call:
                    type = GetCallResultType(primaryClass, call);
                    primaryClass = Context.GetClassByName(type);
                    break;
                case Syntax.Declaration.Expression.Expression childExpression:
                    // Check if there is a method with this name and no parameters
                    if (primaryClass.GetMethodReturnType(childExpression.Token.Literal, new()) != null)
                    {
                        childInfo.Expression = childExpression.ReplaceWithCall();
                        continue;
                    }
                    var fieldName = childExpression.Token.Literal;
                    if (!primaryClass.HasField(fieldName))
                    {
                        throw new Exception($"Couldn't find a field {fieldName} in type {type}");
                    }

                    switch (primaryClass)
                    {
                        case ParsedClassInfo parsedClass:
                            var fieldExpression = parsedClass.GetFieldInfo(fieldName)!.Expression;
                            type = fieldExpression.Type;
                            parsedClass.AddFieldType(fieldName, type);
                            break;
                        case BuiltClassInfo builtClass:
                            type = builtClass.GetFieldType(fieldName)!;
                            break;
                        default:
                            throw new Exception($"Unknown ClassInfo object: {primaryClass}");
                    }
                    primaryClass = Context.GetClassByName(type);
                    break;
                default:
                    throw new Exception($"Unknown Expression type: {childInfo.Expression}");
            }
            childInfo = childInfo.GetChildInfo();
        }
        Type = type;
    }

    private string GetCallResultType(ClassInfo primaryClass, Call call)
    {
        // Get parameter types and find the matching method
        var argTypes = new List<string>();
        foreach (var arg in call.Arguments)
        {
            var argExpression = FromSameContext(arg);
            argTypes.Add(argExpression.Type);
        }
        var type = primaryClass.GetMethodReturnType(call.Token.Literal, argTypes);
        if (type == null)
        {
            var argsStr = string.Join(", ", argTypes);
            throw new Exception($"Couldn't find a method for call {call.Token.Literal}({argsStr}) on type {primaryClass.Name}");
        }
        return type;
    }

    private string ResolveType(string classOrVariable)
    {
        if (Context.Classes!.ClassExists(classOrVariable))
        {
            return classOrVariable;
        }
        if (Context.CurrentMethod == null)
        {
            throw new Exception($"Unknown class {classOrVariable}");
        }

        var methodParameterType = Context.CurrentMethod.GetParameterType(classOrVariable);
        if (methodParameterType != null)
        {
            return methodParameterType;
        }

        if (!Context.CurrentMethod.LocalVariables.ContainsKey(classOrVariable))
        {
            throw new Exception($"Use of unassigned variable {classOrVariable}");
        }

        var localVarInfo = Context.CurrentMethod.LocalVariables[classOrVariable];
        if (localVarInfo.Type == null)
        {
            localVarInfo.ValidateExpression();
        }
        return localVarInfo.Type!;
    }

    private string ResolveBaseReference()
    {
        if (Context.CurrentClass.BaseClass == null)
        {
            throw new Exception($"Class {Context.CurrentClass.Name} is not inherited from anything");
        }
        if (Context.CurrentMethod is not ParsedConstructorInfo)
        {
            throw new Exception("Cannot refer to base class outside of a class constructor.");
        }

        return Context.CurrentClass.BaseClass!.Name;
    }

    private string ValidateAndGetType()
    {
        if (_type == null)
        {
            ValidateExpression();
        }
        return _type!;
    }
}
