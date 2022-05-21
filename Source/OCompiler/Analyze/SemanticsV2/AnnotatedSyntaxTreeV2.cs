using System;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Tree;
using OCompiler.Analyze.Syntax.Declaration;
using OCompiler.Exceptions.Semantic;
using ParsedClassData = OCompiler.Analyze.Syntax.Declaration.Class.Class;

namespace OCompiler.Analyze.SemanticsV2;

internal class AnnotatedSyntaxTreeV2
{
    private static readonly string RootClassName = "Class";
    
    public Dictionary<string, ClassDeclaration> BuiltinClasses { get; }
    public Dictionary<string, ClassDeclaration> ParsedClasses { get; } = new();


    public AnnotatedSyntaxTreeV2(Syntax.Tree syntaxTree)
    {
        BuiltinClasses = new BuiltinClassTree().Classes;
        
        CreateDeclarationsFrom(syntaxTree);
    }

    public ClassDeclaration GetClass(string name)
    {
        if (BuiltinClasses.ContainsKey(name))
        {
            return BuiltinClasses[name];
        }

        if (ParsedClasses.ContainsKey(name))
        {
            return ParsedClasses[name];
        }

        throw new KeyNotFoundException($"The class {name} hasn't been found");
    }

    public bool HasClass(string name)
    {
        return BuiltinClasses.ContainsKey(name) || ParsedClasses.ContainsKey(name);
    }

    private void CreateDeclarationsFrom(Syntax.Tree syntaxTree)
    {
        foreach (var parsedClass in syntaxTree)
        {
            var declaration = CreateEmptyDeclaration(parsedClass);
            AddClassDeclaration(declaration);

            CreateGenericTypeParametersReferences(declaration, parsedClass);
            CreateBaseTypeReference(declaration, parsedClass.Extends);
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
            return;
        }

        var baseTypeReference = new TypeReference(baseType.Name.Literal);
        foreach (var parentGenericType in baseType.GenericTypes)
        {
            if (declaration.HasGenericType(parentGenericType.Name.Literal))
            {
                var genericType = declaration.GetGenericType(parentGenericType.Name.Literal);
                baseTypeReference.GenericTypes.Add(genericType!);
                continue;
            }

            var specializedGenericType = ParseSpecializedGenericType(parentGenericType);
            baseTypeReference.GenericTypes.Add(specializedGenericType);
        }

        declaration.BaseType = baseTypeReference;
    }

    private static TypeReference ParseSpecializedGenericType(TypeAnnotation specializedType)
    {
        var reference = new TypeReference(specializedType.Name.Literal);
        foreach (var specialization in specializedType.GenericTypes)
        {
            reference.GenericTypes.Add(ParseSpecializedGenericType(specialization));
        }

        return reference;
    }
}