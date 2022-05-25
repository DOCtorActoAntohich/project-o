using System;
using OCompiler.Analyze.SemanticsV2.Dom;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Exceptions;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;
using Void = OCompiler.Builtins.Primitives.Void;

namespace OCompiler.Analyze.SemanticsV2;

internal partial class AnnotatedSyntaxTreeV2
{
    private void ValidateMemberBodies()
    {
        foreach (var @class in ParsedClasses.Values)
        {
            ValidateConstructorBodies(@class);
            ValidateMethodBodies(@class);
        }
    }

    private void ValidateConstructorBodies(ClassDeclaration @class)
    {
        foreach (var constructor in @class.Constructors)
        {
            ValidateBody(constructor.Statements);
        }
    }

    private void ValidateMethodBodies(ClassDeclaration @class)
    {
        foreach (var method in @class.Methods)
        {
            ValidateBody(method.Statements);
        }
    }

    private void ValidateBody(StatementsCollection body)
    {
        foreach (var statement in body)
        {
            ValidateStatement(statement);
        }
    }

    private void ValidateStatement(DomStatement statement)
    {
        switch (statement)
        {
            case ReturnStatement returnStatement:
                ValidateReturnStatement(returnStatement);
                break;
            
            case ExpressionStatement expressionStatement:
                ValidateExpressionStatement(expressionStatement);
                break;
            
            case VariableDeclarationStatement variableDeclarationStatement:
                break;
            
            case AssignStatement assignStatement:
                break;
            case ConditionStatement conditionStatement:
                break;
            case LoopStatement loopStatement:
                break;
            default:
                throw new CompilerInternalError($"Unknown statement type: {statement}");
        }
    }

    private void ValidateReturnStatement(ReturnStatement @return)
    {
        var rootHolder = @return.RootHolder;
        if (rootHolder is MemberConstructor && @return.HasValue)
        {
            throw new AnalyzeError($"Cannot return a value from the constructor: {@return.Expression}");
        }

        if (!@return.HasValue)
        {
            var voidReference = new TypeReference(nameof(Void));
            @return.Expression = new ObjectCreateExpression(voidReference);
        }

        DetermineExpressionType(@return.Expression);
        
        if (rootHolder is MemberMethod method && method.ReturnType.DifferentFrom(@return.Expression.Type))
        {
            throw new AnalyzeError("The return type of the method differs from the type of returned expression");
        }
    }
    
    private void ValidateExpressionStatement(ExpressionStatement expressionStatement)
    {
        DetermineExpressionType(expressionStatement.Expression);
    }

    private void ValidateVariableDeclaration()
    {
        
    }
}