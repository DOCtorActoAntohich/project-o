using System;
using System.Text;
using System.Collections.Generic;

using OCompiler.Analyze.Syntax;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Semantics.Expression;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Exceptions;
using OCompiler.Exceptions.Semantic;

namespace OCompiler.Analyze.Semantics;

internal class TreeValidator
{
    private readonly ClassTree _knownClasses;
    public List<ClassInfo> ValidatedClasses => new(_knownClasses);

    public TreeValidator(Tree syntaxTree)
    {
        _knownClasses = new ClassTree(syntaxTree);
        foreach (var @class in _knownClasses)
        {
            if (@class is not ParsedClassInfo parsedClass)
            {
                continue;
            }
            ValidateConstructors(parsedClass);
            ValidateMethods(parsedClass);
            ValidateFields(parsedClass);
        }
    }

    public string GetValidationInfo()
    {
        StringBuilder @string = new();
        @string.AppendLine("Known classes:");
        foreach (var classInfo in _knownClasses)
        {
            @string.Append(classInfo.Name);
            @string.Append(" (");
            @string.Append(classInfo.ToString());
            @string.AppendLine(")");
        }
        return @string.ToString();
    }

    public void ValidateConstructors(ParsedClassInfo classInfo)
    {
        foreach (var constructor in classInfo.Constructors)
        {
            ValidateConstructor(constructor, classInfo);
        }
    }

    public void ValidateMethods(ParsedClassInfo classInfo)
    {
        foreach (var method in classInfo.Methods)
        {
            ValidateMethod(method, classInfo);
        }
    }

    public void ValidateFields(ParsedClassInfo classInfo)
    {
        foreach (var field in classInfo.Fields)
        {
            if (field.Type != null)
            {
                continue;
            }
            field.Expression.ValidateExpression();
            Console.WriteLine($"Warning: unused field {field.Name} of type {field.Type}");
        }
    }

    public void ValidateConstructor(ParsedConstructorInfo constructor, ParsedClassInfo classInfo)
    {
        foreach (var statement in constructor.Body)
        {
            ValidateStatement(statement, classInfo, constructor);
        }
    }

    public void ValidateMethod(ParsedMethodInfo method, ParsedClassInfo classInfo)
    {
        foreach (var statement in method.Body)
        {
            ValidateStatement(statement, classInfo, method);
        }
    }

    public void ValidateStatement(IBodyStatement statement, ParsedClassInfo classInfo, CallableInfo callable)
    {
        switch (statement)
        {
            case Variable variable:
                ValidateVariable(variable, classInfo, callable);
                break;
            case Assignment assignment:
                ValidateAssignment(assignment, classInfo, callable);
                break;
            case If conditional:
                ValidateIf(conditional, classInfo, callable);
                break;
            case While loop:
                ValidateLoop(loop, classInfo, callable);
                break;
            case Return @return:
                ValidateReturn(@return, classInfo, callable);
                break;
            case Syntax.Declaration.Expression.Expression expression:
                new ExpressionInfo(expression, new Context(classInfo, callable)).ValidateExpression();
                break;
            default:
                throw new CompilerInternalError($"Unknown IBodyStatement: {statement}");
        }
    }

    public void ValidateVariable(Variable variable, ParsedClassInfo classInfo, CallableInfo callable)
    {
        var variableName = variable.Identifier.Literal;
        if (ClassTree.ClassExists(variableName))
        {
            throw new Exception($"Cannot create variable, name {variableName} is already used by a class");
        }
        if (callable.HasParameter(variableName))
        {
            throw new NameCollisionError(
                variable.Identifier.Position,
                $"Cannot create variable, name {variable.Identifier.Literal} is already used by a class"
            );
        }
        if (!callable.LocalVariables.TryGetValue(variableName, out var varInfo))
        {
            varInfo = new ExpressionInfo(variable.Expression, new Context(classInfo, callable));
            callable.LocalVariables.Add(variableName, varInfo);
        }
        varInfo.ValidateExpression();
    }

    public void ValidateAssignment(Assignment assignment, ParsedClassInfo classInfo, CallableInfo callable)
    {
        var variableOrField = assignment.Variable;
        if (variableOrField.Child == null)
        {
            ValidateLocalAssignment(variableOrField.Token.Literal, assignment.Value, classInfo, callable);
            return;
        }

        if (variableOrField.Token is not Lexical.Tokens.Keywords.This || variableOrField.Child.Child != null)
        {
            throw new AccessViolationError(variableOrField.Token.Position, $"Fields of other classes cannot be changed directly");
        }

        ValidateFieldAssignment(variableOrField.Child.Token.Literal, assignment.Value, classInfo, callable);
    }

    public void ValidateLocalAssignment(
        string variableName,
        Syntax.Declaration.Expression.Expression value,
        ParsedClassInfo classInfo,
        CallableInfo callable
    )
    {
        if (callable.HasParameter(variableName))
        {
            throw new Exception($"Cannot assign a value to the method parameter {variableName}");
        }
        if (!callable.LocalVariables.TryGetValue(variableName, out var varInfo))
        {
            throw new UnknownNameError(value.Token.Position, $"Variable {variableName} must be declared before assignment");
        }

        var valueInfo = new ExpressionInfo(value, new Context(classInfo, callable));
        if (valueInfo.Type != varInfo.Type)
        {
            throw new TypeError(value.Token.Position, $"Cannot assign value of type {valueInfo.Type} to a variable of type {varInfo.Type}");
        }
    }

    public void ValidateFieldAssignment(
        string fieldName,
        Syntax.Declaration.Expression.Expression value,
        ParsedClassInfo classInfo,
        CallableInfo callable
    )
    {
        var field = classInfo.GetFieldInfo(fieldName);
        if (field == null)
        {
            throw new UnknownNameError(value.Token.Position, $"Field {fieldName} must be declared before assignment");
        }

        var valueInfo = new ExpressionInfo(value, new Context(classInfo, callable));
        if (valueInfo.Type != field.Expression.Type)
        {
            throw new TypeError(value.Token.Position, $"Cannot assign value of type {valueInfo.Type} to a field of type {field.Expression.Type}");
        }
    }

    public void ValidateIf(If conditional, ParsedClassInfo classInfo, CallableInfo callable)
    {
        ValidateCondition(conditional.Condition, classInfo, callable);
        foreach (var statement in conditional.Body)
        {
            ValidateStatement(statement, classInfo, callable);
        }
        foreach (var statement in conditional.ElseBody)
        {
            ValidateStatement(statement, classInfo, callable);
        }
    }

    public void ValidateLoop(While loop, ParsedClassInfo classInfo, CallableInfo callable)
    {
        ValidateCondition(loop.Condition, classInfo, callable);
        foreach (var statement in loop.Body)
        {
            ValidateStatement(statement, classInfo, callable);
        }
    }

    public void ValidateReturn(Return @return, ParsedClassInfo classInfo, CallableInfo callable)
    {
        string methodReturnType = callable switch
        {
            ParsedConstructorInfo => "Void",
            ParsedMethodInfo method => method.ReturnType,
            _ => throw new CompilerInternalError($"Unknown CallableInfo type: {callable}"),
        };

        string returnType = "Void";
        if (@return.ReturnValue != null)
        {
            var returnInfo = new ExpressionInfo(@return.ReturnValue, new Context(classInfo, callable));
            returnInfo.ValidateExpression();
            returnType = returnInfo.Type!;
        }
        if (returnType != methodReturnType)
        {
            throw new TypeError(
                @return.Position,
                methodReturnType == "Void" ?
                $"Cannot use value of type {returnType} as a return value, expected return without an object" :
                $"Cannot use value of type {returnType} as a return value, expected {methodReturnType}"
            );
        }
    }

    public void ValidateCondition(Syntax.Declaration.Expression.Expression condition, ParsedClassInfo classInfo, CallableInfo callable)
    {
        var conditionInfo = new ExpressionInfo(condition, new Context(classInfo, callable));
        conditionInfo.ValidateExpression();
        if (conditionInfo.Type != "Boolean")
        {
            throw new TypeError(condition.Token.Position, $"Cannot use value of type {conditionInfo.Type} as a condition, it must be a Boolean");
        }
    }
}
