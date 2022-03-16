using System;
using System.CodeDom;
using OCompiler.Analyze.Lexical.Tokens.Keywords;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;

namespace OCompiler.CodeGeneration;

internal static partial class CompileUnit
{
    // Very versatile; can be used not only as init expression, but
    // also as a method argument, if-while bool expression, etc.
    private static CodeExpression ParseRvalueExpression(Expression expression)
    {
        return expression switch
        {
            Call call => ParseConstructorCall(call),
            not null => new CodePrimitiveExpression(expression.Token),
            _ => throw new Exception("Uncool init expression XCG1")
        };
    }
    
    private static CodeExpression ParseConstructorCall(Call call)
    {
        var type = new CodeTypeReference(call.Token.Literal);
        var arguments = ParseCallArgumentList(call.Arguments);

        var ctorCall = new CodeObjectCreateExpression(type, arguments);

        var chainedAccess = ParseChainAccess(ctorCall, call.Child);
        return chainedAccess;
    }
    
    private static CodeExpression ParseChainAccess(CodeExpression starterObject, Expression? expression)
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
    
    private static CodeExpression ParseAssignmentExpression(Assignment assignment)
    {
        CodeExpression lhs;
        if (assignment.Variable.Token is This)
        {
            lhs = new CodeThisReferenceExpression();
        }

        return new CodeExpression();
    }
}