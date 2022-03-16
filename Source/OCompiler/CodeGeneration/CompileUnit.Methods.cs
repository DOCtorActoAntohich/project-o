using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;

namespace OCompiler.CodeGeneration;

internal static partial class CompileUnit
{
    private static CodeExpression[] ParseCallArgumentList(IEnumerable<Expression> arguments)
    {
        return arguments.Select(ParseRvalueExpression).ToArray();
    }
    
    private static CodeStatement ParseCodeStatement(IBodyStatement statement)
    {
        // Todo remove prints
        switch (statement)
        {
            case Return @return:
                Console.WriteLine("3 done");
                return ParseReturnStatement(@return);
            
            case If @if:
                Console.WriteLine("4 done");
                return ParseIfStatement(@if);
            
            case While @while:
                Console.WriteLine("5 done");
                return ParseWhileStatement(@while);
            
            case Variable variable:
                Console.WriteLine(1);
                break;
            
            case Assignment assignment:
                Console.WriteLine(2);
                break;
            
            case Call call:
                Console.WriteLine(6);
                break;
            
            case Expression expression:
                Console.WriteLine(7);
                break;
            
            default:
                throw new Exception($"Unknown statement: {statement}");
        }

        return new CodeStatement();
    }
    

    private static CodeStatement ParseIfStatement(If @if)
    {
        var ifCode = new CodeConditionStatement
        {
            Condition = ParseRvalueExpression(@if.Condition)
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

    private static CodeStatement ParseWhileStatement(While @while)
    {
        var whileCode = new CodeIterationStatement
        {
            TestExpression = ParseRvalueExpression(@while.Condition)
        };

        foreach (var statement in ParsedBody(@while.Body))
        {
            whileCode.Statements.Add(statement);
        }

        return whileCode;
    }

    private static CodeStatement ParseReturnStatement(Return @return)
    {
        if (@return.ReturnValue == null)
        {
            return new CodeMethodReturnStatement();
        }

        var returnValue = ParseRvalueExpression(@return.ReturnValue);
        return new CodeMethodReturnStatement(returnValue);
    }

    private static IEnumerable<CodeStatement> ParsedBody(Body body)
    {
        var statements = new List<CodeStatement>();
        foreach (var statement in body)
        {
            var parsedStatement = ParseCodeStatement(statement);
            statements.Add(parsedStatement);
        }
        
        return statements.ToArray();
    }
}