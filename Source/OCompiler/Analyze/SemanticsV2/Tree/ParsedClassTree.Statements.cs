using System;
using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
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

            CreateGenericTypeParametersReferences(declaration, parsedClass);
            CreateBaseTypeReference(declaration, parsedClass.Extends);

            CreateConstructors(declaration, parsedClass);
            CreateMethods(declaration, parsedClass);
            CreateFields(declaration, parsedClass);
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

    private static void CreateGenericTypeParametersReferences(ClassDeclaration declaration, ParsedClassData parsedClass)
    {
        foreach (var genericType in parsedClass.Name.GenericTypes)
        {
            var genericTypeReference = new TypeReference(genericType.Name.Literal, isGeneric: true);
            declaration.GenericTypes.Add(genericTypeReference);
        }
    }

    private static void CreateBaseTypeReference(ClassDeclaration declaration, TypeAnnotation? baseType)
    {
        if (baseType == null)
        {
            declaration.BaseType = new TypeReference(InheritanceTree.RootClassName);
            return;
        }

        var baseTypeReference = new TypeReference(baseType.Name.Literal);
        foreach (var parentGenericType in baseType.GenericTypes)
        {
            var typeReference = TypeReferenceFromTypeAnnotation(declaration, parentGenericType);
            baseTypeReference.GenericTypes.Add(typeReference);
        }

        declaration.BaseType = baseTypeReference;
    }

    private static TypeReference TypeReferenceFromTypeAnnotation(ClassDeclaration declaration, TypeAnnotation type)
    {
        if (declaration.HasGenericType(type.Name.Literal))
        {
            return declaration.GetGenericType(type.Name.Literal)!;
        }
        
        return SpecializedGenericType(declaration, type);
    }
    
    private static TypeReference SpecializedGenericType(ClassDeclaration declaration, TypeAnnotation specializedType)
    {
        var reference = new TypeReference(specializedType.Name.Literal);
        foreach (var specialization in specializedType.GenericTypes)
        {
            reference.GenericTypes.Add(TypeReferenceFromTypeAnnotation(declaration, specialization));
        }

        return reference;
    }

    private void CreateFields(ClassDeclaration declaration, ParsedClassData parsedClass)
    {
        foreach (var field in parsedClass.Fields)
        {
            var memberField = new MemberField(field.Identifier.Literal)
            {
                InitExpression = ParseExpression(field.Expression)
            };

            if (field.Type != null)
            {
                memberField.Type = TypeReferenceFromTypeAnnotation(declaration, field.Type);
            }
            
            declaration.AddField(memberField);
        }
    }
    
    private void CreateConstructors(ClassDeclaration declaration, ParsedClassData parsedClass)
    {
        foreach (var constructor in parsedClass.Constructors)
        {
            var memberConstructor = new MemberConstructor(declaration.Name);
            CreateParameters(declaration, memberConstructor, constructor.Parameters);
            
            FillBlock(memberConstructor, constructor.Body);

            declaration.AddConstructor(memberConstructor);
        }
    }

    private void CreateParameters(
        ClassDeclaration declaration,
        CallableMember callable,
        IEnumerable<CallableParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            var parameterType = TypeReferenceFromTypeAnnotation(declaration, parameter.Type);

            var parameterDeclaration = new ParameterDeclarationExpression(parameter.Name.Literal, parameterType);
            callable.AddParameter(parameterDeclaration);
        }
    }

    private void CreateMethods(ClassDeclaration declaration, ParsedClassData parsedClass)
    {
        foreach (var method in parsedClass.Methods)
        {
            var memberMethod = new MemberMethod(declaration.Name);
            CreateParameters(declaration, memberMethod, method.Parameters);
            
            // TODO generics.
            
            if (method.ReturnType != null)
            {
                memberMethod.ReturnType = TypeReferenceFromTypeAnnotation(declaration, method.ReturnType);
            }
            
            FillBlock(memberMethod, method.Body);

            declaration.AddMethod(memberMethod);
        }
    }
    
    private void FillBlock(ICanHaveStatements block, Body parsedBody, bool alternative = false)
    {
        if (alternative && block is ConditionStatement @if)
        {
            FillElseBody(@if, parsedBody);
            return;
        }
        
        foreach (var parsedStatement in parsedBody)
        {
            var statement = ParseStatement(block, parsedStatement);
            block.AddStatement(statement);
        }
    }

    private void FillElseBody(ConditionStatement @if, Body parsedBody)
    {
        foreach (var parsedStatement in parsedBody)
        {
            var statement = ParseStatement(@if, parsedStatement);
            @if.AddElseStatement(statement);
        }
    }

    private DomStatement ParseStatement(ICanHaveStatements holder, IBodyStatement statement)
    {
        return statement switch
        {
            Return @return => ParseReturnStatement(holder, @return),
            If @if => ParseIfStatement(holder, @if),
            While @while => ParseWhileLoop(holder, @while),
            Assignment assignment => ParseAssignmentStatement(holder, assignment),
            Variable variable => ParseVariableDeclaration(holder, variable),
            SyntaxExpression rvalueExpression => ParseRValueExpressionStatement(holder, rvalueExpression),
            _ => throw new CompilerInternalError("Unknown statement type")
        };
    }

    private DomStatement ParseReturnStatement(ICanHaveStatements holder, Return returnStatement)
    {
        if (returnStatement.ReturnValue == null)
        {
            return new ReturnStatement();
        }
        
        var expression = ParseExpression(returnStatement.ReturnValue);
        return new ReturnStatement(expression);
    }
    
    private DomStatement ParseIfStatement(ICanHaveStatements holder, If ifStatement)
    {
        var condition = ParseExpression(ifStatement.Condition);
        var @if = new ConditionStatement(condition);
        FillBlock(@if, ifStatement.Body);
        
        if (!ifStatement.ElseBody.IsEmpty)
        {
            FillBlock(@if, ifStatement.ElseBody, alternative: true);
        }

        return @if;
    }

    private DomStatement ParseWhileLoop(ICanHaveStatements holder, While whileLoop)
    {
        var condition = ParseExpression(whileLoop.Condition);
        var @while = new LoopStatement(condition);
        FillBlock(@while, whileLoop.Body);

        return @while;
    }

    private DomStatement ParseRValueExpressionStatement(ICanHaveStatements holder, SyntaxExpression expression)
    {
        return new ExpressionStatement(ParseExpression(expression));
    }

    private DomStatement ParseVariableDeclaration(ICanHaveStatements holder, Variable variable)
    {
        var declaration = new VariableDeclarationStatement(variable.Identifier.Literal);
        
        if (variable.Type != null)
        {
            var root = GetRootHolder(holder);
            declaration.Type = TypeReferenceFromTypeAnnotation(root, variable.Type);
        }

        declaration.InitExpression = ParseExpression(variable.Expression);
        
        return declaration;
    }
    
    private DomStatement ParseAssignmentStatement(ICanHaveStatements holder, Assignment assignment)
    {
        var lvalue = ParseExpression(assignment.Variable);
        var rvalue = ParseExpression(assignment.Value);
        return new AssignStatement(lvalue, rvalue);
    }

    private ClassDeclaration GetRootHolder(ICanHaveStatements holder)
    {
        return (holder switch
        {
            Statement statement => GetRootHolder(statement.Holder!),
            TypeMember typeMember => typeMember.Owner,
            _ => throw new ArgumentOutOfRangeException(nameof(holder), holder, null)
        })!;
    }
}