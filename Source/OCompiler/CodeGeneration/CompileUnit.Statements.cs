using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Semantics.Expression;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;

namespace OCompiler.CodeGeneration;

internal static partial class CompileUnit
{
    private static IEnumerable<CodeStatement> ParsedBody(Body body)
    {
        var statements = new List<CodeStatement>();
        foreach (var statement in body)
        {
            var parsedStatement = ParsedCodeStatement(statement);
            statements.Add(parsedStatement);
        }
        
        return statements.ToArray();
    }
    
    private static CodeStatement ParsedCodeStatement(IBodyStatement statement)
    {
        // Todo remove prints
        Console.WriteLine(statement);
        switch (statement)
        {
            case Return @return:
                Console.WriteLine("3 done");
                return ParsedReturnStatement(@return);
            
            case If @if:
                Console.WriteLine("4 done");
                return ParsedIfStatement(@if);
            
            case While @while:
                Console.WriteLine("5 done");
                return ParsedWhileStatement(@while);
            
            case Variable variable:
                Console.WriteLine(1);
                return ParsedVariableDeclaration(variable);
            
            case Assignment assignment:
                Console.WriteLine(2);
                break;
            
            case Expression expression:
                Console.WriteLine(7);
                break;
            
            default:
                throw new Exception($"Unknown statement: {statement}");
        }

        return new CodeStatement();
    }
    

    private static CodeStatement ParsedReturnStatement(Return @return)
    {
        if (@return.ReturnValue == null)
        {
            return new CodeMethodReturnStatement();
        }

        var returnValue = ParsedRvalueExpression(@return.ReturnValue);
        return new CodeMethodReturnStatement(returnValue);
    }
    
    private static CodeStatement ParsedIfStatement(If @if)
    {
        var ifCode = new CodeConditionStatement
        {
            Condition = ParsedRvalueExpression(@if.Condition)
        };

        foreach (var statement in ParsedBody(@if.Body))
        {
            ifCode.TrueStatements.Add(statement);
        }

        if (@if.ElseBody.IsEmpty)
        {
            return ifCode;
        }
        
        foreach (var statement in ParsedBody(@if.ElseBody))
        {
            ifCode.FalseStatements.Add(statement);
        }

        return ifCode;
    }

    private static CodeStatement ParsedWhileStatement(While @while)
    {
        var whileCode = new CodeIterationStatement
        {
            TestExpression = ParsedRvalueExpression(@while.Condition)
        };

        foreach (var statement in ParsedBody(@while.Body))
        {
            whileCode.Statements.Add(statement);
        }

        return whileCode;
    }


    private static CodeStatement ParsedVariableDeclaration(Variable variable)
    {
        var a = new CodeVariableDeclarationStatement
        {
            Name = variable.Identifier.Literal,
            Type = variable.Expression.Type,
            InitExpression = ParsedRvalueExpression(variable.Expression)
        };
        var aa = new ExpressionInfo(variable.Expression, new Context())

        Console.WriteLine(variable.Expression);
        //a.Name = variable.Identifier.

        return a;
    }
}