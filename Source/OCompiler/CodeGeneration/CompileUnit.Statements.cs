using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Semantics.Expression;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.StandardLibrary.CodeDom;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit
{
    private IEnumerable<CodeStatement> ParsedBody(Body body)
    {
        // LINQ evil.
        return body.Select(ParsedCodeStatement).ToArray();
    }
    
    private CodeStatement ParsedCodeStatement(IBodyStatement statement)
    {
        return statement switch
        {
            Return @return => ParsedReturnStatement(@return),
            If @if => ParsedIfStatement(@if),
            While @while => ParsedWhileStatement(@while),
            Variable variable => ParsedVariableDeclarationStatement(variable),
            Assignment assignment => ParsedAssignmentStatement(assignment),
            Expression expression => ParsedRvalueExpressionStatement(expression),
            _ => throw new Exception($"Unknown statement: {statement}")
        };
    }
    

    private CodeStatement ParsedReturnStatement(Return @return)
    {
        if (@return.ReturnValue == null)
        {
            return new CodeMethodReturnStatement();
        }

        var returnValue = ParsedRvalueExpression(@return.ReturnValue);
        return new CodeMethodReturnStatement(returnValue);
    }
    
    private CodeStatement ParsedIfStatement(If @if)
    {
        var condition = ParsedRvalueExpression(@if.Condition);
        var readBoolResult = new CodeFieldReferenceExpression(condition, Base.InternalValueVariableName);
        var ifCode = new CodeConditionStatement
        {
            Condition = readBoolResult
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

    private CodeStatement ParsedWhileStatement(While @while)
    {
        var condition = ParsedRvalueExpression(@while.Condition);
        var readBoolResult = new CodeFieldReferenceExpression(condition, Base.InternalValueVariableName);
        var whileCode = new CodeIterationStatement
        {
            TestExpression = readBoolResult
        };

        foreach (var statement in ParsedBody(@while.Body))
        {
            whileCode.Statements.Add(statement);
        }

        return whileCode;
    }


    private CodeStatement ParsedVariableDeclarationStatement(Variable variable)
    {
        var variableExpressionInfo = ExpressionInfoInCurrentContext(variable.Expression);

        return new CodeVariableDeclarationStatement
        {
            Name = variable.Identifier.Literal,
            Type = new CodeTypeReference(variableExpressionInfo.Type),
            InitExpression = ParsedRvalueExpression(variable.Expression)
        };
    }

    private CodeStatement ParsedAssignmentStatement(Assignment assignment)
    {
        var lvalue = ParsedLvalueExpression(assignment.Variable);
        var rvalue = ParsedRvalueExpression(assignment.Value);
        return new CodeAssignStatement(lvalue, rvalue);
    }

    private CodeStatement ParsedRvalueExpressionStatement(Expression expression)
    {
        return new CodeExpressionStatement(ParsedRvalueExpression(expression));
    }
}