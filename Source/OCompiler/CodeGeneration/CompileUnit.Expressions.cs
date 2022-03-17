using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Lexical.Tokens.Keywords;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using DomIO = OCompiler.StandardLibrary.CodeDom.Reference.IO;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit
{
    private CodeExpression FirstLvalueExpression(Expression expression)
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
    
    private CodeExpression ParsedLvalueExpression(Expression expression)
    {
        var referredObject = FirstLvalueExpression(expression);

        var currentExpression = expression;
        while (currentExpression.Child != null)
        {
            var fieldName = currentExpression.Child.Token.Literal;
            referredObject = new CodeFieldReferenceExpression(referredObject, fieldName);
            currentExpression = currentExpression.Child;
        }
        
        return referredObject;
    }


    private CodeExpression FirstRvalueExpression(Expression expression)
    {
        if (expression.Token is This)
        {
            return new CodeThisReferenceExpression();
        }
        
        if (CurrentCallableHasParameter(expression.Token.Literal))
        {
            return new CodeArgumentReferenceExpression(expression.Token.Literal);
        }
        
        // Primitive type = Implicit call by ctor.
        if (IsPrimitiveTypeToken(expression.Token))
        {
            return ParsedPrimitiveToken(expression.Token);
        }
        
        // Type name = Explicit call by ctor.
        if (IsClassName(expression.Token.Literal))
        {
            return ParsedConstructorCall(expression);
        }

        return new CodeVariableReferenceExpression(expression.Token.Literal);
    }
    
    private CodeExpression ParsedRvalueExpression(Expression expression)
    {
        if (expression.Token is Base && expression is Call call)
        {
            return ParsedBaseCall(call);
        }
        
        var referredObject = FirstRvalueExpression(expression);
        var currentExpression = expression.Child;
        while (currentExpression != null)
        {
            referredObject = ParsedCurrentRvalueExpression(referredObject, currentExpression);
            currentExpression = currentExpression.Child;
        }

        return referredObject;
    }

    // Only chained access via dot.
    private CodeExpression ParsedCurrentRvalueExpression(CodeExpression root, Expression expression)
    {
        if (expression is Call call)
        {
            return ParsedRvalueCall(root, call);
        }
        
        return new CodeFieldReferenceExpression(root, expression.Token.Literal);
    }

    private CodeExpression ParsedPrimitiveToken(Token token)
    {
        var type = ConvertPrimitiveTokenToType(token);
        return new CodeObjectCreateExpression(
            type, TokenToPrimitiveExpression(token));
    }

    private CodeExpression ParsedBaseCall(Call call)
    {
        var ctor = (CodeConstructor?) _currentCallable;
        foreach (var expression in ParsedRvalueCallArguments(call.Arguments))
        {
            ctor?.BaseConstructorArgs.Add(expression);
        }

        return EmptyExpression();
    }
    private CodeExpression ParsedRvalueCall(CodeExpression root, Call call)
    {
        return new CodeMethodInvokeExpression(
            root, call.Token.Literal, ParsedRvalueCallArguments(call.Arguments));
    }

    private CodeExpression ParsedConstructorCall(Expression expression)
    {
        // Explicit call (has brackets).
        if (expression is Call call)
        {
            return new CodeObjectCreateExpression(
                call.Token.Literal, ParsedRvalueCallArguments(call.Arguments));
        }

        // Implicit call (no brackets).
        return new CodeObjectCreateExpression(expression.Token.Literal);
    }

    private CodeExpression[] ParsedRvalueCallArguments(IEnumerable<Expression> arguments)
    {
        return arguments.Select(ParsedRvalueExpression).ToArray();
    }

    private CodeExpression EmptyExpression()
    {
        return new CodeObjectCreateExpression(DomIO.FullTypeName);
    }
}