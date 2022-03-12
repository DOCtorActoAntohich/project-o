using System;
using System.Text;
using System.Collections.Generic;

using OCompiler.Analyze.Syntax;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Semantics.Expression;
using OCompiler.Analyze.Semantics.Callable;

namespace OCompiler.Analyze.Semantics;

internal class TreeValidator
{
    public List<ClassInfo> ValidatedClasses => new(_knownClasses.Values);

    private readonly Dictionary<string, ClassInfo> _knownClasses = new(BuiltClassInfo.StandardClasses);

    public TreeValidator(Tree syntaxTree)
    {
        LearnAllClasses(syntaxTree);
        foreach (var @class in syntaxTree)
        {
            var parsedClass = ParsedClassInfo.GetByClass(@class);
            ValidateConstructors(parsedClass);
            ValidateMethods(parsedClass);
            ValidateFields(parsedClass);
        }
    }

    private void LearnAllClasses(Tree syntaxTree)
    {
        foreach (var @class in syntaxTree)
        {
            var parsedClass = ParsedClassInfo.GetByClass(@class);
            _knownClasses.Add(parsedClass.Name, parsedClass);
            parsedClass.Context.AddClasses(_knownClasses);
        }
    }

    public string GetValidationInfo()
    {
        StringBuilder @string = new();
        @string.AppendLine("Known classes:");
        foreach (var (name, classInfo) in _knownClasses)
        {
            @string.Append(name);
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
            field.Type = field.Expression.Type;
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
                new ExpressionInfo(expression, new Context(classInfo, _knownClasses, callable)).ValidateExpression();
                break;
            default:
                throw new Exception($"Unknown IBodyStatement: {statement}");
        }
    }

    public void ValidateVariable(Variable variable, ParsedClassInfo classInfo, CallableInfo callable)
    {
        if (!callable.LocalVariables.TryGetValue(variable.Identifier.Literal, out var varInfo))
        {
            varInfo = new ExpressionInfo(variable.Expression, new Context(classInfo, _knownClasses, callable));
            callable.LocalVariables.Add(variable.Identifier.Literal, varInfo);
        }
        if (varInfo.Type == null)
        {
            varInfo.ValidateExpression();
        }
    }

    public void ValidateAssignment(Assignment assignment, ParsedClassInfo classInfo, CallableInfo callable)
    {
        if (callable.LocalVariables.TryGetValue(assignment.Identifier.Literal, out var varInfo))
        {
            var valueInfo = new ExpressionInfo(assignment.Value, new Context(classInfo, _knownClasses, callable));
            valueInfo.ValidateExpression();
            if (valueInfo.Type != varInfo.Type)
            {
                throw new Exception($"Cannot assign value of type {valueInfo.Type} to a variable of type {varInfo.Type}");
            }
            return;
        }
        if (/* LHS is field and class has this field */false)
        {
        }
        throw new Exception($"{assignment.Identifier.Literal} must be declared before assignment");
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
            _ => throw new Exception($"Unknown CallableInfo type: {callable}"),
        };

        string returnType = "Void";
        if (@return.ReturnValue != null)
        {
            var returnInfo = new ExpressionInfo(@return.ReturnValue, new Context(classInfo, _knownClasses, callable));
            returnInfo.ValidateExpression();
            returnType = returnInfo.Type!;
        }
        if (returnType != methodReturnType)
        {
            throw new Exception(
                methodReturnType == "Void" ?
                $"Cannot use value of type {returnType} as a return value, expected return without an object" :
                $"Cannot use value of type {returnType} as a return value, expected {methodReturnType}"
            );
        }
    }

    public void ValidateCondition(Syntax.Declaration.Expression.Expression condition, ParsedClassInfo classInfo, CallableInfo callable)
    {
        var conditionInfo = new ExpressionInfo(condition, new Context(classInfo, _knownClasses, callable));
        conditionInfo.ValidateExpression();
        if (conditionInfo.Type != "Boolean")
        {
            throw new Exception($"Cannot use value of type {conditionInfo.Type} as a condition, it must be a Boolean");
        }
    }
}
