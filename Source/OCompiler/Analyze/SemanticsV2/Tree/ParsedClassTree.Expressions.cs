using System;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Lexical.Tokens.BooleanLiterals;
using OCompiler.Analyze.Lexical.Tokens.Keywords;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.NameReference;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Primitive;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Exceptions;
using ParsedClassData = OCompiler.Analyze.Syntax.Declaration.Class.Class;
using ParsedConstructorData = OCompiler.Analyze.Syntax.Declaration.Class.Member.Constructor;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;
using SyntaxExpression = OCompiler.Analyze.Syntax.Declaration.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2.Tree;

internal partial class ParsedClassTree
{
    private DomExpression ParseExpression(SyntaxExpression expression)
    {
        var sourceObject = ParseExpression(expression, null);
        while (expression.Child != null)
        {
            sourceObject = ParseExpression(expression.Child, sourceObject);
            expression = expression.Child;
        }

        return sourceObject;
    }

    private DomExpression ParseExpression(SyntaxExpression expression, DomExpression? sourceObject)
    {
        return expression switch
        {
            Call call => ParseCallExpression(call, sourceObject),
            SimpleExpression simpleExpression => ParseSimpleExpression(simpleExpression, sourceObject),
            ListDefinition listDefinition => ParseListInitExpression(listDefinition, sourceObject),
            DictDefinition dictDefinition => ParseDictInitExpression(dictDefinition, sourceObject),
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }

    private DomExpression ParseCallExpression(Call call, DomExpression? sourceObject)
    {
        var arguments = ParseArguments(call);
        return call.Token switch
        {
            Base => new BaseConstructorCallExpression(arguments),
            Identifier when call.Parent == null => 
                new ObjectCreateExpression(new TypeReference(call.Token.Literal), arguments),
            _ => new MethodCallExpression(sourceObject, call.Token.Literal, arguments)
        };
    }
    
    private DomExpression ParseSimpleExpression(SimpleExpression expression, DomExpression? sourceObject)
    {
        return expression.Token switch
        {
            This => new ThisReferenceExpression(),
            
            True => new BooleanLiteralExpression(true),
            False => new BooleanLiteralExpression(false),
            
            IntegerLiteral integerLiteral => new IntegerLiteralExpression(integerLiteral.Value),
            RealLiteral realLiteral => new RealLiteralExpression(realLiteral.Value),
            StringLiteral stringLiteral => new StringLiteralExpression(stringLiteral.Literal),
            
            Identifier identifier when expression.Parent != null => 
                new FieldReferenceExpression(sourceObject, identifier.Literal),
            Identifier identifier when expression.Parent == null => 
                new VariableReferenceExpression(identifier.Literal),
            
            _ => throw new CompilerInternalError("Token type not found")
        };
    }

    private IEnumerable<DomExpression> ParseArguments(Call call)
    {
        return call.Arguments.Select(ParseExpression);
    }

    private DomExpression ParseListInitExpression(ListDefinition listDefinition, DomExpression? sourceObject)
    {
        throw new NotImplementedException();
    }
    
    private DomExpression ParseDictInitExpression(DictDefinition dictDefinition, DomExpression? sourceObject)
    {
        throw new NotImplementedException();
    }
}