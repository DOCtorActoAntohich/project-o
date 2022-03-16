using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Lexical.Tokens.Keywords;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit
{
    private CodeExpression ParsedLvalueExpression(Expression expression)
    {
        return new CodeExpression();
    }
    
    private CodeExpression ParsedRvalueExpression(Expression expression)
    {
        return expression switch
        {
            Call call => ParseConstructorCall(call),
            not null => new CodePrimitiveExpression(expression.Token),
            _ => throw new Exception("Uncool init expression XCG1")
        };
    }
    
    private CodeExpression ParseConstructorCall(Call call)
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
    }
}