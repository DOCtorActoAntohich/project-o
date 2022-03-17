using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Lexical.Tokens.Keywords;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Syntax.Declaration.Expression;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit
{
    private CodeExpression ParsedLvalueExpression(Expression expression)
    {
        var referredObject = ReferencedRootObjectExpression(expression);

        var currentExpression = expression;
        while (currentExpression.Child != null)
        {
            var fieldName = currentExpression.Child.Token.Literal;
            referredObject = new CodeFieldReferenceExpression(referredObject, fieldName);
            currentExpression = currentExpression.Child;
        }

        return referredObject;
    }

    private CodeExpression ReferencedRootObjectExpression(Expression expression)
    {
        if (expression.Token is This)
        {
            return new CodeThisReferenceExpression();
        }

        var objectName = expression.Token.Literal;
        if (CurrentCallableHasParameter(objectName))
        {
            return new CodeArgumentReferenceExpression(objectName);
        }

        if (IsClassName(objectName))
        {
            return new CodeTypeReferenceExpression(objectName);
        }

        return new CodeVariableReferenceExpression(objectName);
    }

    private bool IsClassName(string name)
    {
        var classInfo = ParsedClassInfo.GetByName(name);
        return classInfo is not EmptyParsedClassInfo;
    }
    
    private CodeExpression ParsedRvalueExpression(Expression expression)
    {
        var referredObject = ReferencedRootObjectExpression(expression);
        var currentExpression = expression;
        while (currentExpression != null)
        {
            if (currentExpression is Call call && call.Token is Base)
            {
                Console.WriteLine(currentExpression.Token + " " + currentExpression.Token.Literal);
                foreach (var arg in call.Arguments)
                {
                    Console.Write(arg + "");
                }
                Console.WriteLine();
            }
            referredObject = ParsedCurrentRvalueExpression(referredObject, currentExpression);
            currentExpression = currentExpression.Child;
        }
        
        /*Console.WriteLine("MOGUSMOGUSMOGUS");
        int s = 0;
        //Console.WriteLine(s + " "  + expression.Token + " " + expression.Token.Literal);
        var expr = expression;
        while (expr != null)
        {
            s += 1;
            Console.WriteLine(s + " "  + expr.Token + " " + expr.Token.Literal);
            var info = ExpressionInfoInCurrentContext(expr);
            Console.WriteLine("info: " + (info.Type ?? "idk"));
            if (expr is Call c)
            {
                Console.Write(c.Token.Literal + " ");
                foreach (var arg in c.Arguments)
                {
                    Console.Write("[" + arg.Parent + "] " + arg + " ");
                }
                Console.WriteLine();
            }
            expr = expr.Child;
        }*/

        /*return expression switch
        {
            Call call => ParseConstructorCall(call),
            not null => new CodePrimitiveExpression(expression.Token),
            _ => throw new Exception("Uncool init expression XCG1")
        };*/
        
        return new CodeExpression();
    }

    private CodeExpression ParsedCurrentRvalueExpression(CodeExpression root, Expression expression)
    {
        if (expression is Call call)
        {
            return ParsedRvalueCall(root, call);
        }
        return new CodeExpression();
    }

    private CodeExpression ParsedRvalueCall(CodeExpression root, Call call)
    {
        if (IsClassName(call.Token.Literal))
        {
            return new CodeObjectCreateExpression(
                call.Token.Literal, ParsedRvalueCallArguments(call.Arguments));
        }

        if (call.Token is Base)
        {
            return root; // TODO is it param redirection when inheritance?
        }

        return new CodeMethodInvokeExpression(
            root, call.Token.Literal, ParsedRvalueCallArguments(call.Arguments));
    }

    private CodeExpression[] ParsedRvalueCallArguments(IEnumerable<Expression> arguments)
    {
        return arguments.Select(ParsedRvalueExpression).ToArray();
    }

    /*private CodeExpression ParseConstructorCall(Call call)
    {
        var type = new CodeTypeReference(call.Token.Literal);
        var arguments = ParseCallArgumentList(call.Arguments);

        var ctorCall = new CodeObjectCreateExpression(type, arguments);

        var chainedAccess = ParseChainAccess(ctorCall, call.Child);
        return chainedAccess;
    }
    
    private CodeExpression ParseChainAccess(CodeExpression starterObject, Expression? expression)
    {
        switch (expression)
        {
            case null:
                return starterObject;
            
            case Call call:
            {
                var objectAfterCall = new CodeMethodInvokeExpression(
                    starterObject, call.Token.Literal, ParseCallArgumentList(call.Arguments));
                return ParseChainAccess(objectAfterCall, call.Child);
            }

            default:
            {
                var accessedField = new CodeFieldReferenceExpression(starterObject, expression.Token.Literal);
                return ParseChainAccess(accessedField, expression.Child);
            }
        }
    }

    private CodeExpression[] ParseCallArgumentList(IEnumerable<Expression> arguments)
    {
        return arguments.Select(ParsedRvalueExpression).ToArray();
    }*/
}