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
            case Variable variable:
                Console.WriteLine(1);
                break;
            
            case Assignment assignment:
                Console.WriteLine(2);
                break;
            
            case Return @return:
                Console.WriteLine("3 done");
                return ParseReturnStatement(@return);
            
            case If @if:
                Console.WriteLine(4);
                break;
            
            case While @while:
                Console.WriteLine(5);
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
    
    
    private static void AddMethod(this CodeTypeDeclaration typeDeclaration, ParsedMethodInfo methodInfo)
    {
        foreach (var statement in methodInfo.Body)
        {
            // TODO rm
            Console.WriteLine(statement);
            var todo = ParseCodeStatement(statement);
        }
    }

    private static CodeStatement ParseIfStatement(If @if)
    {
        var condition = ParseRvalueExpression(@if.Condition);
        //var trueBody = ParseBody
        
        var a = new CodeConditionStatement();

        return new CodeStatement();
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
}