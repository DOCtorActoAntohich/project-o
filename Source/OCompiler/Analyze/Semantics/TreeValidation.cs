using System;
using System.Linq;
using System.Collections.Generic;

using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Class;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Class.Member.Method;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;

namespace OCompiler.Analyze.Semantics
{
    internal class TreeValidator
    {
        private class ExpressionInfo
        {
            public Expression Expression { get; }
            public Class Class { get; }
            public IClassMember? Method { get; }
            public string? TargetVariable { get; }
            public Dictionary<string, string?> LocalVariables { get; }

            public ExpressionInfo(Expression expression, Class @class, IClassMember? method, string? targetVariable, Dictionary<string, string?> locals)
            {
                Expression = expression;
                Class = @class;
                Method = method;
                TargetVariable = targetVariable;
                LocalVariables = locals;
            }

            public ExpressionInfo(Expression expression, Class @class, string? targetVariable)
            {
                Expression = expression;
                Class = @class;
                TargetVariable = targetVariable;
                LocalVariables = new();
            }

            public ExpressionInfo? GetChildInfo()
            {
                if (Expression.Child == null)
                {
                    return null;
                }
                return new(Expression.Child, Class, Method, null, LocalVariables);
            }

            public ExpressionInfo FromSameContext(Expression newExpression)
            {
                return new(newExpression, Class, Method, null, LocalVariables);
            }
        }

        private readonly Dictionary<string, Class> _knownClasses = new();
        private readonly List<Identifier> _classReferences = new();
        private readonly List<ExpressionInfo> _expressions = new();

        public TreeValidator(Tree syntaxTree)
        {
            foreach (var @class in syntaxTree)
            {
                Validate(@class);
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

        public void Validate(Class @class)
        {
            foreach (var field in @class.Fields)
            {
                Validate(field, @class);
            }
            foreach (var constructor in @class.Constructors)
            {
                Validate(constructor, @class);
            }
            foreach (var method in @class.Methods)
            {
                Validate(method, @class);
            }
            _knownClasses.Add(@class.Name.Literal, @class);
        }

        public void Validate(Field field, Class @class)
        {
            LateValidate(new ExpressionInfo(field.Expression, @class, field.Identifier.Literal));
        }

        public void Validate(Method method, Class @class)
        {
            var locals = new Dictionary<string, string?>();
            foreach (var parameter in method.Parameters)
            {
                _classReferences.Add(parameter.Type);
            }
            _classReferences.Add(method.ReturnType!); // TODO: make ReturnType not-null
            foreach (var statement in method.Body)
            {
                Validate(statement, @class, method, ref locals);
            }
        }

        public void Validate(Constructor constructor, Class @class)
        {
            var locals = new Dictionary<string, string?>();
            foreach (var parameter in constructor.Parameters)
            {
                _classReferences.Add(parameter.Type);
            }
            foreach (var statement in constructor.Body)
            {
                Validate(statement, @class, constructor, ref locals);
            }
        }

        public void Validate(IBodyStatement statement, Class @class, IClassMember method, ref Dictionary<string, string?> locals)
        {
            switch (statement)
            {
                case Variable variable:
                    Validate(variable, @class, method, ref locals);
                    break;
                case Assignment assignment:
                    Validate(assignment, @class, method, ref locals);
                    break;
                case If conditional:
                    Validate(conditional, @class, method, ref locals);
                    break;
                case Return @return:
                    Validate(@return, @class, method, ref locals);
                    break;
                case While loop:
                    Validate(loop, @class, method, ref locals);
                    break;
                case Expression expression:
                    LateValidate(new ExpressionInfo(expression, @class, method, null, locals));
                    break;
                default:
                    throw new Exception($"Unknown IBodyStatement: {statement}");
            }
        }

        public void Validate(Variable variable, Class @class, IClassMember method, ref Dictionary<string, string?> locals)
        {
            var variableName = variable.Identifier.Literal;
            locals.Add(variableName, null);
            LateValidate(new ExpressionInfo(variable.Expression, @class, method, variableName, locals));
        }

        public void Validate(Assignment assignment, Class @class, IClassMember method, ref Dictionary<string, string?> locals)
        {
            LateValidate(new ExpressionInfo(assignment.Value, @class, method, null, locals));
        }

        public void Validate(If conditional, Class @class, IClassMember method, ref Dictionary<string, string?> locals)
        {
            LateValidate(new ExpressionInfo(conditional.Condition, @class, method, null, locals));
            foreach (var statement in conditional.Body)
            {
                Validate(statement, @class, method, ref locals);
            }
            foreach (var statement in conditional.ElseBody)
            {
                Validate(statement, @class, method, ref locals);
            }
        }

        public void Validate(Return @return, Class @class, IClassMember method, ref Dictionary<string, string?> locals)
        {
            LateValidate(new ExpressionInfo(@return.ReturnValue!, @class, method, null, locals));
        }

        public void Validate(While loop, Class @class, IClassMember method, ref Dictionary<string, string?> locals)
        {
            LateValidate(new ExpressionInfo(loop.Condition, @class, method, null, locals));
            foreach (var statement in loop.Body)
            {
                Validate(statement, @class, method, ref locals);
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
                Lexical.Tokens.Keywords.This => info.Class.Name.Literal,
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
                            throw new Exception($"Couldn't find a method for call {call} on type {primaryClass.Name.Literal}");
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

        private Class GetKnownType(string name)
        {
            if (!_knownClasses.TryGetValue(name, out var @class))
            {
                throw new Exception($"Unknown type: {name}");
            }
            return @class;
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
}
