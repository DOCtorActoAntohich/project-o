using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Exceptions;
using OCompiler.Exceptions.Semantic;

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
            Identifier identifier => ResolveType(identifier),
            Lexical.Tokens.Keywords.This => Context.Class.Name,
            Lexical.Tokens.Keywords.Base => ResolveBaseReference(),
            IntegerLiteral => "Integer",
            BooleanLiteral => "Boolean",
            StringLiteral => "String",
            RealLiteral => "Real",
            _ => throw new CompilerInternalError($"Unexpected Primary expression: {Expression}")
        };

        var primaryClass = ClassTree.TraversedClasses[type];

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
                throw new UnknownNameError(constructorCall.Token.Position, $"Couldn't find a constructor to call: {constructorCall}");
            }
        }

        var childInfo = GetChildInfo();

        while (childInfo != null)
        {
            switch (childInfo.Expression)
            {
                case Call call:
                    type = GetCallResultType(primaryClass, call);
                    primaryClass = ClassTree.TraversedClasses[type];
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
                        throw new UnknownNameError(childExpression.Token.Position, $"Couldn't find a field {fieldName} in type {type}");
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
                            throw new CompilerInternalError($"Unknown ClassInfo object: {primaryClass}");
                    }
                    primaryClass = ClassTree.TraversedClasses[type];
                    break;
                default:
                    throw new CompilerInternalError($"Unknown Expression type: {childInfo.Expression}");
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
            throw new UnknownNameError(call.Token.Position, $"Couldn't find a method for call {call.Token.Literal}({argsStr}) on type {primaryClass.Name}");
        }
        return type;
    }
    private string ResolveType(Identifier identifier)
    {
        var classOrVariable = identifier.Literal;
        if (ClassTree.ClassExists(classOrVariable))
        {
            return classOrVariable;
        }
        if (Context.Callable == null)
        {
            throw new UnknownNameError(identifier.Position, $"Unknown class {classOrVariable}");
        }

        var methodParameterType = Context.Callable.GetParameterType(classOrVariable);
        if (methodParameterType != null)
        {
            return methodParameterType;
        }

        if (!Context.Callable.LocalVariables.ContainsKey(classOrVariable))
        {
            throw new UnknownNameError(identifier.Position, $"Use of unassigned variable {classOrVariable}");
        }

        var localVarInfo = Context.Callable.LocalVariables[classOrVariable];
        if (localVarInfo.Type == null)
        {
            localVarInfo.ValidateExpression();
        }
        return localVarInfo.Type!;
    }

    private string ResolveBaseReference()
    {
        if (Context.Class.BaseClass == null)
        {
            throw new CompilerInternalError($"Class {Context.Class.Name} is not inherited from anything");
        }
        if (Context.Callable is not ParsedConstructorInfo)
        {
            throw new AnalyzeError(Expression.Token.Position, "Cannot refer to the base class outside of a class constructor.");
        }

        return Context.Class.BaseClass!.Name;
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
