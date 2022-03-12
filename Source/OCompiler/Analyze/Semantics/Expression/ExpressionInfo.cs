using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Syntax.Declaration.Expression;

using System;
using System.Collections.Generic;

namespace OCompiler.Analyze.Semantics.Expression;

internal class ExpressionInfo
{
    public Syntax.Declaration.Expression.Expression Expression { get; }
    public string? Type { get; private set; }

    public ExpressionInfo(Syntax.Declaration.Expression.Expression expression)
    {
        Expression = expression;
    }

    public ExpressionInfo? GetChildInfo()
    {
        if (Expression.Child == null)
        {
            return null;
        }
        return new(Expression.Child);
    }

    public ExpressionInfo FromSameContext(Syntax.Declaration.Expression.Expression expression)
    {
        return new(expression);
    }

    public void ValidateExpression(ParsedClassInfo currentClass, Dictionary<string, ClassInfo> allClasses, CallableInfo? currentMethod = null)
    {
        var type = Expression.Token switch
        {
            Identifier identifier => ResolveType(identifier.Literal, currentClass, allClasses, currentMethod),
            Lexical.Tokens.Keywords.This => currentClass.Name,
            IntegerLiteral => "Integer",
            BooleanLiteral => "Boolean",
            StringLiteral => "String",
            RealLiteral => "Real",
            _ => throw new Exception($"Unexpected Primary expression: {Expression}")
        };

        var primaryClass = GetClassByName(type, allClasses);

        if (Expression is Call constructorCall)
        {
            // Get parameter types and find the matching constructor
            var argTypes = new List<string>();
            foreach (var arg in constructorCall.Arguments)
            {
                var argExpression = new ExpressionInfo(arg);
                argExpression.ValidateExpression(currentClass, allClasses, currentMethod);
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
                        var argExpression = new ExpressionInfo(arg);
                        argExpression.ValidateExpression(currentClass, allClasses, currentMethod);
                        argTypes.Add(argExpression.Type!);
                    }
                    type = primaryClass.GetMethodReturnType(call.Token.Literal, argTypes);
                    if (type == null)
                    {
                        var argsStr = string.Join(", ", argTypes);
                        throw new Exception($"Couldn't find a method for call {call.Token.Literal}({argsStr}) on type {primaryClass.Name}");
                    }
                    primaryClass = GetClassByName(type, allClasses);
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
                            fieldExpression.ValidateExpression(currentClass, allClasses);
                            fieldType = fieldExpression.Type!;
                            parsedClass.AddFieldType(fieldName, fieldType);
                            break;
                        case BuiltClassInfo builtClass:
                            fieldType = builtClass.GetFieldType(fieldName)!;
                            break;
                        default:
                            throw new Exception($"Unknown ClassInfo object: {primaryClass}");
                    }
                    primaryClass = GetClassByName(fieldType, allClasses);
                    break;
                default:
                    throw new Exception($"Unknown Expression type: {childInfo.Expression}");
            }
            childInfo = childInfo.GetChildInfo();
        }
        Type = type;
    }

    private static ClassInfo GetClassByName(string name, Dictionary<string, ClassInfo> classes)
    {
        if (!classes.TryGetValue(name, out var classInfo))
        {
            throw new Exception($"Unknown type: {name}");
        }
        return classInfo;
    }

    private static string ResolveType(
        string classOrVariable,
        ParsedClassInfo currentClass,
        Dictionary<string, ClassInfo> classes,
        CallableInfo? currentMethod = null
    )
    {
        if (classes.ContainsKey(classOrVariable))
        {
            return classOrVariable;
        }
        if (currentMethod == null)
        {
            throw new Exception($"Unknown class {classOrVariable}");
        }

        var methodParameterType = currentMethod.GetParameterType(classOrVariable);
        if (methodParameterType != null)
        {
            return methodParameterType;
        }

        if (!currentMethod.LocalVariables.ContainsKey(classOrVariable))
        {
            throw new Exception($"Use of unassigned variable {classOrVariable}");
        }

        var localVarInfo = currentMethod.LocalVariables[classOrVariable];
        if (localVarInfo.Type == null)
        {
            localVarInfo.ValidateExpression(currentClass, classes, currentMethod);
        }
        return localVarInfo.Type!;
    }
}
