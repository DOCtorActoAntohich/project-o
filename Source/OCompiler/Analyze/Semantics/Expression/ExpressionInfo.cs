using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Syntax.Declaration.Expression;

using System;
using System.Collections.Generic;

namespace OCompiler.Analyze.Semantics.Expression;

internal class ExpressionInfo
{
    public Syntax.Declaration.Expression.Expression Expression { get; }
    public Context Context { get; }
    public string? Type { get; private set; }

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
                argExpression.ValidateExpression();
                argTypes.Add(argExpression.Type!);
            }
            if (!primaryClass.HasConstructor(argTypes))
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
                    // Get parameter types and find the matching method
                    var argTypes = new List<string>();
                    foreach (var arg in call.Arguments)
                    {
                        var argExpression = FromSameContext(arg);
                        argExpression.ValidateExpression();
                        argTypes.Add(argExpression.Type!);
                    }
                    type = primaryClass.GetMethodReturnType(call.Token.Literal, argTypes);
                    if (type == null)
                    {
                        var argsStr = string.Join(", ", argTypes);
                        throw new Exception($"Couldn't find a method for call {call.Token.Literal}({argsStr}) on type {primaryClass.Name}");
                    }
                    primaryClass = Context.GetClassByName(type);
                    break;
                case Syntax.Declaration.Expression.Expression expression:
                    var fieldName = expression.Token.Literal;
                    if (!primaryClass.HasField(fieldName))
                    {
                        throw new Exception($"Couldn't find a field {fieldName} in type {type}");
                    }

                    string fieldType;
                    switch (primaryClass)
                    {
                        case ParsedClassInfo parsedClass:
                            var fieldExpression = parsedClass.GetFieldInfo(fieldName)!.Expression;
                            fieldExpression.ValidateExpression();
                            fieldType = fieldExpression.Type!;
                            parsedClass.AddFieldType(fieldName, fieldType);
                            break;
                        case BuiltClassInfo builtClass:
                            fieldType = builtClass.GetFieldType(fieldName)!;
                            break;
                        default:
                            throw new Exception($"Unknown ClassInfo object: {primaryClass}");
                    }
                    primaryClass = Context.GetClassByName(fieldType);
                    break;
                default:
                    throw new Exception($"Unknown Expression type: {childInfo.Expression}");
            }
            childInfo = childInfo.GetChildInfo();
        }
        Type = type;
    }

    private string ResolveType(string classOrVariable)
    {
        if (Context.Classes!.ContainsKey(classOrVariable))
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
}
