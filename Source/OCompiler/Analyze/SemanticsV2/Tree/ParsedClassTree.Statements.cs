using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.Exceptions;
using OCompiler.Exceptions.Semantic;
using ParsedClassData = OCompiler.Analyze.Syntax.Declaration.Class.Class;
using ParsedConstructorData = OCompiler.Analyze.Syntax.Declaration.Class.Member.Constructor;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;
using SyntaxExpression = OCompiler.Analyze.Syntax.Declaration.Expression.Expression;
using CallableParameter = OCompiler.Analyze.Syntax.Declaration.Class.Member.Method.Parameter;

namespace OCompiler.Analyze.SemanticsV2.Tree;

internal partial class ParsedClassTree
{
    private void CreateDeclarationsFrom(Syntax.Tree syntaxTree)
    {
        foreach (var parsedClass in syntaxTree)
        {
            var declaration = CreateEmptyDeclaration(parsedClass);
            AddClassDeclaration(declaration);

            AddClassGenericTypes(declaration, parsedClass);
            AddBaseTypeReference(declaration, parsedClass.Extends);

            CreateFields(declaration, parsedClass);
            CreateMethods(declaration, parsedClass);
            CreateConstructors(declaration, parsedClass);
        }
    }

    private ClassDeclaration CreateEmptyDeclaration(ParsedClassData parsedClass)
    {
        if (HasClass(parsedClass.NameLiteral))
        {
            throw new NameCollisionError(
                parsedClass.TokenPosition, $"Name {parsedClass.NameLiteral} is already defined");
        }

        return new ClassDeclaration(parsedClass.NameLiteral);
    }
    
    private void AddClassDeclaration(ClassDeclaration declaration)
    {
        ParsedClasses.Add(declaration.Name, declaration);
    }

    private static void AddClassGenericTypes(ClassDeclaration declaration, ParsedClassData parsedClass)
    {
        foreach (var genericType in parsedClass.Name.GenericTypes)
        {
            var genericTypeReference = ParseRawTypeReference(genericType);
            genericTypeReference.IsGeneric = true;
            declaration.GenericTypes.Add(genericTypeReference);
        }
    }

    private static TypeReference ParseRawTypeReference(TypeAnnotation typeAnnotation)
    {
        var reference = new TypeReference(typeAnnotation.Name.Literal);
        foreach (var genericType in typeAnnotation.GenericTypes)
        {
            reference.GenericTypes.Add(ParseRawTypeReference(genericType));
        }
        return reference;
    }
    
    private static void AddBaseTypeReference(ClassDeclaration declaration, TypeAnnotation? baseType)
    {
        if (baseType == null)
        {
            declaration.BaseType = new TypeReference(InheritanceTree.RootClassName);
            return;
        }
        
        declaration.BaseType = ParseRawTypeReference(baseType);
    }

    private void CreateFields(ClassDeclaration declaration, ParsedClassData parsedClass)
    {
        foreach (var field in parsedClass.Fields)
        {
            var memberField = new MemberField(field.Identifier.Literal);
            declaration.AddField(memberField);

            memberField.InitExpression = ParseExpression(field.Expression);

            if (field.Type != null)
            {
                memberField.Type = ParseRawTypeReference(field.Type);
            }
        }
    }
    
    private void CreateConstructors(ClassDeclaration declaration, ParsedClassData parsedClass)
    {
        foreach (var constructor in parsedClass.Constructors)
        {
            var memberConstructor = new MemberConstructor(declaration.Name);
            declaration.AddConstructor(memberConstructor);
            
            CreateParameters(memberConstructor, constructor.Parameters);
            FillBlock(memberConstructor.Statements, constructor.Body);
        }
        
        CreateDefaultConstructor(declaration);
    }

    private void CreateDefaultConstructor(ClassDeclaration declaration)
    {
        foreach (var constructor in declaration.Constructors)
        {
            if (constructor.Parameters.Count == 0)
            {
                return;
            }
        }

        var defaultConstructor = new MemberConstructor(declaration.Name);
        declaration.AddConstructor(defaultConstructor);
    }

    private void CreateMethods(ClassDeclaration declaration, ParsedClassData parsedClass)
    {
        foreach (var method in parsedClass.Methods)
        {
            var memberMethod = new MemberMethod(method.Name.Literal);
            declaration.AddMethod(memberMethod);
            
            CreateParameters(memberMethod, method.Parameters);
            
            // TODO generics.
            
            if (method.ReturnType != null)
            {
                memberMethod.ReturnType = ParseRawTypeReference(method.ReturnType);
            }
            
            FillBlock(memberMethod.Statements, method.Body);
        }
    }
    
    private void CreateParameters(CallableMember callable, IEnumerable<CallableParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            var parameterType = ParseRawTypeReference(parameter.Type);

            var parameterDeclaration = new ParameterDeclarationExpression(parameter.Name.Literal, parameterType);
            callable.Parameters.Add(parameterDeclaration);
        }
    }

    private void FillBlock(StatementsCollection block, Body parsedBody)
    {
        foreach (var parsedStatement in parsedBody)
        {
            var statement = ParseStatement(parsedStatement);
            block.Add(statement);
        }
    }

    private DomStatement ParseStatement(IBodyStatement statement)
    {
        return statement switch
        {
            Return @return => ParseReturnStatement(@return),
            If @if => ParseIfStatement(@if),
            While @while => ParseWhileLoop(@while),
            Assignment assignment => ParseAssignmentStatement(assignment),
            Variable variable => ParseVariableDeclaration(variable),
            SyntaxExpression rvalueExpression => ParseSingleExpressionStatement(rvalueExpression),
            _ => throw new CompilerInternalError("Unknown statement type")
        };
    }

    private DomStatement ParseReturnStatement(Return returnStatement)
    {
        if (returnStatement.ReturnValue == null)
        {
            return new ReturnStatement();
        }
        
        var expression = ParseExpression(returnStatement.ReturnValue);
        return new ReturnStatement(expression);
    }
    
    private DomStatement ParseIfStatement(If ifStatement)
    {
        var condition = ParseExpression(ifStatement.Condition);
        var @if = new ConditionStatement(condition);
        FillBlock(@if.Statements, ifStatement.Body);
        
        if (!ifStatement.ElseBody.IsEmpty)
        {
            FillBlock(@if.ElseStatements, ifStatement.ElseBody);
        }

        return @if;
    }

    private DomStatement ParseWhileLoop(While whileLoop)
    {
        var condition = ParseExpression(whileLoop.Condition);
        var @while = new LoopStatement(condition);
        FillBlock(@while.Statements, whileLoop.Body);

        return @while;
    }

    private DomStatement ParseSingleExpressionStatement(SyntaxExpression expression)
    {
        return new ExpressionStatement(ParseExpression(expression));
    }

    private DomStatement ParseVariableDeclaration(Variable variable)
    {
        var variableDeclaration = new VariableDeclarationStatement(variable.Identifier.Literal);
        
        if (variable.Type != null)
        {
            variableDeclaration.Type = ParseRawTypeReference(variable.Type);
        }

        variableDeclaration.InitExpression = ParseExpression(variable.Expression);
        
        return variableDeclaration;
    }
    
    private DomStatement ParseAssignmentStatement(Assignment assignment)
    {
        var lvalue = ParseExpression(assignment.Variable);
        var rvalue = ParseExpression(assignment.Value);
        return new AssignStatement(lvalue, rvalue);
    }
}