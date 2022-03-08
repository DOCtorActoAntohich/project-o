using System;
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
        private readonly List<string> _knownClasses = new();
        private readonly List<Identifier> _classReferences = new();

        public TreeValidator(Tree syntaxTree)
        {
            foreach (var @class in syntaxTree)
            {
                Validate(@class);
            }
            foreach (var identifier in _classReferences)
            {
                var className = identifier.Literal;
                if (!_knownClasses.Contains(className))
                {
                    throw new Exception($"Unknown type {className} was referenced at position {identifier.StartOffset}");
                }
            }
        }

        public void Validate(Class @class)
        {
            foreach (var field in @class.Fields)
            {
                Validate(field);
            }
            foreach (var constructor in @class.Constructors)
            {
                Validate(constructor);
            }
            foreach (var method in @class.Methods)
            {
                Validate(method);
            }
            _knownClasses.Add(@class.Name.Literal);
        }

        public void Validate(Field field)
        {
            Validate(field.Expression);
        }

        public void Validate(Method method)
        {
            foreach (var parameter in method.Parameters)
            {
                _classReferences.Add(parameter.Type);
            }
            _classReferences.Add(method.ReturnType!); // TODO: make ReturnType not-null
            foreach (var statement in method.Body)
            {
                Validate(statement);
            }
        }

        public void Validate(Constructor constructor)
        {
            foreach (var parameter in constructor.Parameters)
            {
                _classReferences.Add(parameter.Type);
            }
            foreach (var statement in constructor.Body)
            {
                Validate(statement);
            }
        }

        public void Validate(IBodyStatement statement)
        {
            switch (statement)
            {
                case Variable variable:
                    Validate(variable);
                    break;
                case Assignment assignment:
                    Validate(assignment);
                    break;
                case If conditional:
                    Validate(conditional);
                    break;
                case Return @return:
                    Validate(@return);
                    break;
                case While loop:
                    Validate(loop);
                    break;
                default:
                    throw new Exception($"Unknown IBodyStatement: {statement}");
            }
        }

        public void Validate(Variable variable)
        {
            Validate(variable.Expression);
        }

        public void Validate(Assignment assignment)
        {
            Validate(assignment.Value);
        }

        public void Validate(If conditional)
        {
            Validate(conditional.Condition);
            foreach (var statement in conditional.Body)
            {
                Validate(statement);
            }
            foreach (var statement in conditional.ElseBody)
            {
                Validate(statement);
            }
        }

        public void Validate(Return @return)
        {
            Validate(@return.ReturnValue!);
        }

        public void Validate(While loop)
        {
            Validate(loop.Condition);
            foreach (var statement in loop.Body)
            {
                Validate(statement);
            }
        }

        public void Validate(Expression expression)
        {
            // TODO
        }
    }
}
