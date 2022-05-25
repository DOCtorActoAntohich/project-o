using OCompiler.Analyze.SemanticsV2.Dom;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.NameReference;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Exceptions;
using Boolean = OCompiler.Builtins.Primitives.Boolean;
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
            CheckIfBaseCallIsFirst(@class);
            InsertFieldsInitialization(@class);
            
            ValidateConstructorBodies(@class);
            ValidateMethodBodies(@class);
        }
    }

    private void CheckIfBaseCallIsFirst(ClassDeclaration @class)
    {
        foreach (var constructor in @class.Constructors)
        {
            var index = FindBaseCall(constructor.Statements);
            if (index > 0)
            {
                throw new AnalyzeError(
                    "Call to base constructor must be the first statement of the constructor");
            }

            if (index != -1)
            {
                continue;
            }
            
            var callExpression = new BaseConstructorCallExpression();
            var baseCallStatement = new ExpressionStatement(callExpression);
            constructor.Statements.InsertBaseCall(baseCallStatement);
        }
    }

    private int FindBaseCall(StatementsCollection constructorBody)
    {
        var index = 0;
        foreach (var statement in constructorBody)
        {
            if (statement is ExpressionStatement {Expression: BaseConstructorCallExpression})
            {
                return index;
            }

            ++index;
        }

        return -1;
    }
    
    private void InsertFieldsInitialization(ClassDeclaration @class)
    {
        foreach (var constructor in @class.Constructors)
        {
            foreach (var field in @class.Fields)
            {
                var fieldReference = new FieldReferenceExpression(new ThisReferenceExpression(), field.Name);
                var assignment = new AssignStatement(fieldReference, field.InitExpression);
                constructor.Statements.InsertFieldInitialization(assignment);
            }
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
                ValidateVariableDeclaration(variableDeclarationStatement);
                break;
            
            case AssignStatement assignStatement:
                ValidateAssignStatement(assignStatement);
                break;
            
            case ConditionStatement conditionStatement:
                ValidateConditionStatement(conditionStatement);
                break;
            
            case LoopStatement loopStatement:
                ValidateLoopStatement(loopStatement);
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

    private void ValidateVariableDeclaration(VariableDeclarationStatement variableDeclaration)
    {
        var variableTable = variableDeclaration.ParentBody.VariableTable;
        if (variableTable.Has(variableDeclaration.Name))
        {
            throw new AnalyzeError($"Redefinition of a variable {variableDeclaration.Name}");
        }

        if (!variableDeclaration.HasTypeAnnotation && !variableDeclaration.HasInitExpression)
        {
            throw new AnalyzeError(
                $"The type of variable {variableDeclaration.Name} was not specified.");
        }

        if (!variableDeclaration.HasTypeAnnotation && variableDeclaration.HasInitExpression)
        {
            DetermineExpressionType(variableDeclaration.InitExpression);
            variableDeclaration.Type = variableDeclaration.InitExpression.Type;
            variableTable.Add(variableDeclaration.Name, variableDeclaration.Type);
            return;
        }

        var @class = variableDeclaration.RootHolder.Owner!;
        ValidateTypeReference(@class, variableDeclaration.Type);
        if (variableDeclaration.HasTypeAnnotation && !variableDeclaration.HasInitExpression)
        {
            variableDeclaration.InitExpression = new ObjectCreateExpression(variableDeclaration.Type);
        }
        
        DetermineExpressionType(variableDeclaration.InitExpression);
        if (variableDeclaration.Type.DifferentFrom(variableDeclaration.InitExpression.Type))
        {
            throw new AnalyzeError(
                $"Types of annotation and init expression don't match: {variableDeclaration.Name}");
        }
        
        variableTable.Add(variableDeclaration.Name, variableDeclaration.Type);
    }

    private void ValidateAssignStatement(AssignStatement assignment)
    {
        DetermineExpressionType(assignment.LValue);
        DetermineExpressionType(assignment.RValue);

        if (assignment.LValue.Type.DifferentFrom(assignment.RValue.Type))
        {
            throw new AnalyzeError($"Couldn't assign value of type {assignment.RValue.Type} " +
                                   $"to type {assignment.LValue.Type}");
        }
    }

    private void ValidateConditionStatement(ConditionStatement @if)
    {
        DetermineExpressionType(@if.Condition);
        if (@if.Condition.Type.DifferentFrom(new TypeReference(nameof(Boolean))))
        {
            throw new AnalyzeError("Condition in `if` statement must be of type Boolean");
        }
        
        ValidateBody(@if.Statements);
        ValidateBody(@if.ElseStatements);
    }

    private void ValidateLoopStatement(LoopStatement @while)
    {
        DetermineExpressionType(@while.Condition);
        if (@while.Condition.Type.DifferentFrom(new TypeReference(nameof(Boolean))))
        {
            throw new AnalyzeError("Condition in `while` statement must be of type Boolean");
        }
        
        ValidateBody(@while.Statements);
    }
}