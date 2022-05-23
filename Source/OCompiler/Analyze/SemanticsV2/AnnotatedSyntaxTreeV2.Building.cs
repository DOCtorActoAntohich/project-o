using System;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.Nested;
using OCompiler.Analyze.SemanticsV2.Dom.Statement.SingleLine;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Analyze.SemanticsV2.Tree;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Analyze.Syntax.Declaration.Class.Member;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using OCompiler.Analyze.Syntax.Declaration.Statement;
using OCompiler.Exceptions;
using OCompiler.Exceptions.Semantic;
using ParsedClassData = OCompiler.Analyze.Syntax.Declaration.Class.Class;
using ParsedConstructorData = OCompiler.Analyze.Syntax.Declaration.Class.Member.Constructor;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;
using SyntaxExpression = OCompiler.Analyze.Syntax.Declaration.Expression.Expression;

namespace OCompiler.Analyze.SemanticsV2;

internal partial class AnnotatedSyntaxTreeV2
{
    private delegate DomStatement StatementParsingMethod(ICanHaveStatements owner, IBodyStatement statement);
    
    private void CreateDeclarationsFrom(Syntax.Tree syntaxTree)
    {
        foreach (var parsedClass in syntaxTree)
        {
            var declaration = CreateEmptyDeclaration(parsedClass);
            AddClassDeclaration(declaration);

            CreateGenericTypeParametersReferences(declaration, parsedClass);
            CreateBaseTypeReference(declaration, parsedClass.Extends);

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
        
    }
    
    private void CreateConstructors(ClassDeclaration declaration, ParsedClassData parsedClass)
    {
        foreach (var constructor in parsedClass.Constructors)
        {
            var memberConstructor = new MemberConstructor(declaration.Name);
            foreach (var parameter in constructor.Parameters)
            {
                var parameterType = TypeReferenceFromTypeAnnotation(declaration, parameter.Type);
                /*if (!IsValid(parameterType))
                {
                    throw new AnalyzeError(
                        parameter.Name.Position,
                        $"Parameter type {parameterType} in constructor" +
                        $"{declaration.Name}({constructor.Parameters.Count}) is not found");
                }*/
                
                var parameterDeclaration = new ParameterDeclarationExpression(parameter.Name.Literal, parameterType);
                memberConstructor.AddParameter(parameterDeclaration);
            }
            
            declaration.AddConstructor(memberConstructor);
        }
    }

    private void CreateMethods(ClassDeclaration declaration, ParsedClassData parsedClass)
    {
        
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
            var statement = ParseStatement(parsedStatement);
            block.AddStatement(statement);
        }
    }

    private void FillElseBody(ConditionStatement @if, Body parsedBody)
    {
        foreach (var parsedStatement in parsedBody)
        {
            var statement = ParseStatement(parsedStatement);
            @if.AddElseStatement(statement);
        }
    }

    private DomStatement ParseStatement(IBodyStatement statement)
    {
        return statement switch
        {
            Return @return => ParseReturnStatement(@return),
            If @if => ParseIfStatement(@if),
            While @while => ParseWhileLoop(@while),
            Assignment assignment => ParseAssignment(assignment),
            Field field => ParseField(field),
            Variable variable => ParseVariable(variable),
            _ => throw new CompilerInternalError("Unknown statement type")
        };
    }

    private DomStatement ParseReturnStatement(Return returnStatement)
    {
        if (returnStatement.ReturnValue == null)
        {
            return new ReturnStatement();
        }
        
        var expression = ParseRValueExpression(returnStatement.ReturnValue);
        return new ReturnStatement(expression);
    }
    
    private DomStatement ParseIfStatement(If ifStatement)
    {
        var condition = ParseRValueExpression(ifStatement.Condition);
        var @if = new ConditionStatement(condition);
        FillBlock(@if, ifStatement.Body);
        
        if (!ifStatement.ElseBody.IsEmpty)
        {
            FillBlock(@if, ifStatement.ElseBody, alternative: true);
        }

        return @if;
    }

    private DomStatement ParseWhileLoop(While whileLoop)
    {
        var condition = ParseRValueExpression(whileLoop.Condition);
        var @while = new LoopStatement(condition);
        FillBlock(@while, whileLoop.Body);

        return @while;
    }

    private DomExpression ParseRValueExpression(SyntaxExpression expression)
    {
        return expression switch
        {
            DictDefinition dictDefinition => throw new NotImplementedException(),
            Call call => throw new NotImplementedException(),
            ListDefinition listDefinition => throw new NotImplementedException(),
            SimpleExpression simpleExpression => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }
    
    
    private DomStatement ParseMethodCall(ICanHaveStatements owningBlock, Call call)
    {
        return new ReturnStatement(new ThisReferenceExpression());
    }

    private DomStatement ParseDictDefinition(ICanHaveStatements owningBlock, DictDefinition call)
    {
        return new ReturnStatement(new ThisReferenceExpression());
    }
    
    private DomStatement ParseListDefinition(ICanHaveStatements owningBlock, ListDefinition call)
    {
        return new ReturnStatement(new ThisReferenceExpression());
    }
    
    private DomStatement ParseSimpleExpression(ICanHaveStatements owningBlock, SimpleExpression call)
    {
        return new ReturnStatement(new ThisReferenceExpression());
    }
    
    private DomStatement ParseField(ICanHaveStatements owningBlock, Field call)
    {
        return new ReturnStatement(new ThisReferenceExpression());
    }
    
    private DomStatement ParseAssignment(ICanHaveStatements owningBlock, Assignment call)
    {
        return new ReturnStatement(new ThisReferenceExpression());
    }

    private DomStatement ParseVariable(ICanHaveStatements owningBlock, Variable call)
    {
        return new ReturnStatement(new ThisReferenceExpression());
    }
}