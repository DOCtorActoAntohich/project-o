using System;
using System.Linq;
using System.Collections.Generic;

using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.Analyze.Semantics.Class;
using System.Text;

namespace OCompiler.Analyze.Semantics;

internal class TreeValidator
{
    public List<ClassInfo> ValidatedClasses => new(_knownClasses.Values);

    private readonly Dictionary<string, ClassInfo> _knownClasses = new(StandardClassInfo.StandardClasses);
    private readonly List<Identifier> _classReferences = new();
    private readonly List<ExpressionInfo> _expressions = new();

    public TreeValidator(Tree syntaxTree)
    {
        foreach (var @class in syntaxTree)
        {
            Validate(new ParsedClassInfo(@class));
        }

        foreach (var expression in _expressions)
        {
            ValidateExpression(expression);
        }

        foreach (var identifier in _classReferences)
        {
            var className = identifier.Literal;
            if (!_knownClasses.ContainsKey(className))
            {
                throw new Exception($"Unknown type {className} was referenced at position {identifier.StartOffset}");
            }
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
        @string.AppendLine("\nExplicitly referenced classes:");
        foreach (var identifier in _classReferences)
        {
            @string.AppendLine(identifier.Literal);
        }
        return @string.ToString();
    }

    public void Validate(ParsedClassInfo classInfo)
    {
        foreach (var field in classInfo.Fields)
        {
            Validate(field, classInfo);
        }
        foreach (var constructor in classInfo.Constructors)
        {
            Validate(constructor, classInfo);
        }
        foreach (var method in classInfo.Methods)
        {
            Validate(method, classInfo);
        }
        _knownClasses.Add(classInfo.Name, classInfo);
    }

    public void Validate(Field field, ParsedClassInfo classInfo)
    {
        LateValidate(new ExpressionInfo(field.Expression, classInfo, field.Identifier.Literal));
    }

    public void Validate(Method method, ParsedClassInfo classInfo)
    {
        var locals = new Dictionary<string, string?>();
        foreach (var parameter in method.Parameters)
        {
            _classReferences.Add(parameter.Type);
        }
        _classReferences.Add(method.ReturnType!); // TODO: make ReturnType not-null
        foreach (var statement in method.Body)
        {
            Validate(statement, classInfo, method, ref locals);
        }
    }

    public void Validate(Constructor constructor, ParsedClassInfo classInfo)
    {
        var locals = new Dictionary<string, string?>();
        foreach (var parameter in constructor.Parameters)
        {
            _classReferences.Add(parameter.Type);
        }
        foreach (var statement in constructor.Body)
        {
            Validate(statement, classInfo, constructor, ref locals);
        }
    }

    public void Validate(IBodyStatement statement, ParsedClassInfo classInfo, IClassMember method, ref Dictionary<string, string?> locals)
    {
        switch (statement)
        {
            case Variable variable:
                Validate(variable, classInfo, method, ref locals);
                break;
            case Assignment assignment:
                Validate(assignment, classInfo, method, ref locals);
                break;
            case If conditional:
                Validate(conditional, classInfo, method, ref locals);
                break;
            case Return @return:
                Validate(@return, classInfo, method, ref locals);
                break;
            case While loop:
                Validate(loop, classInfo, method, ref locals);
                break;
            case Expression expression:
                LateValidate(new ExpressionInfo(expression, classInfo, method, null, locals));
                break;
            default:
                throw new Exception($"Unknown IBodyStatement: {statement}");
        }
    }

    public void Validate(Variable variable, ParsedClassInfo classInfo, IClassMember method, ref Dictionary<string, string?> locals)
    {
        var variableName = variable.Identifier.Literal;
        locals.Add(variableName, null);
        LateValidate(new ExpressionInfo(variable.Expression, classInfo, method, variableName, locals));
    }

    public void Validate(Assignment assignment, ParsedClassInfo classInfo, IClassMember method, ref Dictionary<string, string?> locals)
    {
        LateValidate(new ExpressionInfo(assignment.Value, classInfo, method, null, locals));
    }

    public void Validate(If conditional, ParsedClassInfo classInfo, IClassMember method, ref Dictionary<string, string?> locals)
    {
        LateValidate(new ExpressionInfo(conditional.Condition, classInfo, method, null, locals));
        foreach (var statement in conditional.Body)
        {
            Validate(statement, classInfo, method, ref locals);
        }
        foreach (var statement in conditional.ElseBody)
        {
            Validate(statement, classInfo, method, ref locals);
        }
    }

    public void Validate(Return @return, ParsedClassInfo classInfo, IClassMember method, ref Dictionary<string, string?> locals)
    {
        LateValidate(new ExpressionInfo(@return.ReturnValue!, classInfo, method, null, locals));
    }

    public void Validate(While loop, ParsedClassInfo classInfo, IClassMember method, ref Dictionary<string, string?> locals)
    {
        LateValidate(new ExpressionInfo(loop.Condition, classInfo, method, null, locals));
        foreach (var statement in loop.Body)
        {
            Validate(statement, classInfo, method, ref locals);
        }
    }

    private void LateValidate(ExpressionInfo info)
    {
        _expressions.Add(info);
    }

    private string ValidateExpression(ExpressionInfo info)
    {
        var type = info.Expression.Token switch
        {
            Identifier identifier => ResolveType(identifier.Literal, info),
            Lexical.Tokens.Keywords.This => info.Class.Name,
            IntegerLiteral => "Integer",
            BooleanLiteral => "Boolean",
            StringLiteral => "String",
            RealLiteral => "Real",
            _ => throw new Exception($"Unexpected Primary expression: {info.Expression}")
        };

        var primaryClass = GetKnownType(type);
        
        if (info.Expression is Call constructorCall)
        {
            var argTypes = new List<string>();
            foreach (var arg in constructorCall.Arguments)
            {
                argTypes.Add(ValidateExpression(info.FromSameContext(arg)));
            }
            if (!primaryClass.HasConstructor(argTypes))
            {
                throw new Exception($"Couldn't find a constructor to call: {constructorCall}");
            }
        }

        var childInfo = info.GetChildInfo();

        while (childInfo != null)
        {
            switch (childInfo.Expression)
            {
                case Call call:
                    var argTypes = new List<string>();
                    foreach (var arg in call.Arguments)
                    {
                        argTypes.Add(ValidateExpression(info.FromSameContext(arg)));
                    }
                    type = primaryClass.GetMethodReturnType(call.Token.Literal, argTypes);
                    if (type == null)
                    {
                        var argsStr = string.Join(", ", argTypes);
                        throw new Exception($"Couldn't find a method for call {call.Token.Literal}({argsStr}) on type {primaryClass.Name}");
                    }
                    primaryClass = GetKnownType(type);
                    break;
                case Expression expression:
                    var fieldName = expression.Token.Literal;
                    if (!primaryClass.HasField(fieldName))
                    {
                        throw new Exception($"Couldn't find a field {fieldName} in type {type}");
                    }
                    var candidates = _expressions.Where(exprInfo => exprInfo.TargetVariable == fieldName && exprInfo.Class == primaryClass).ToList();
                    if (candidates.Count > 1)
                    {
                        throw new Exception($"Field {fieldName} defined more than once in class {primaryClass}");
                    }
                    if (candidates.Count == 0)
                    {
                        throw new Exception($"Field {fieldName} is not found in class {primaryClass}");
                    }
                    var fieldType = ValidateExpression(candidates[0]);
                    primaryClass = GetKnownType(fieldType);
                    break;
                default:
                    throw new Exception($"Unknown Expression type: {childInfo.Expression}");
            }
            childInfo = childInfo.GetChildInfo();
        }
        return type;
    }

    private Class.ClassInfo GetKnownType(string name)
    {
        if (!_knownClasses.TryGetValue(name, out var classInfo))
        {
            throw new Exception($"Unknown type: {name}");
        }
        return classInfo;
    }

    private string ResolveType(string classOrVariable, ExpressionInfo info)
    {
        if (_knownClasses.ContainsKey(classOrVariable))
        {
            return classOrVariable;
        }
        if (!info.LocalVariables.ContainsKey(classOrVariable))
        {
            throw new Exception($"Use of unassigned variable {classOrVariable}");
        }

        var type = info.LocalVariables[classOrVariable];
        if (type == null)
        {
            foreach (var otherInfo in _expressions)
            {
                if (otherInfo.Method == info.Method && otherInfo.TargetVariable == classOrVariable)
                {
                    type = ValidateExpression(otherInfo);
                    info.LocalVariables[classOrVariable] = type;
                    break;
                }
            }

            if (type == null)
            {
                throw new Exception($"Unknown symbol {classOrVariable}");
            }
        }
        return type;
    }
}
