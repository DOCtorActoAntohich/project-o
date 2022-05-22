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
    public Dictionary<string, ClassDeclaration> BuiltinClasses { get; }
    public Dictionary<string, ClassDeclaration> ParsedClasses { get; } = new();


    public AnnotatedSyntaxTreeV2(Syntax.Tree syntaxTree)
    {
        BuiltinClasses = new BuiltinClassTree().Classes;
        
        CreateDeclarationsFrom(syntaxTree);

        var inheritanceTree = new InheritanceTree(this);
    }

    public int ClassesCount => BuiltinClasses.Count + ParsedClasses.Count;

    public IEnumerable<string> AllNames()
    {
        foreach (var name in BuiltinClasses.Keys)
        {
            yield return name;
        }

        foreach (var name in ParsedClasses.Keys)
        {
            yield return name;
        }
    }
    
    public IEnumerable<ClassDeclaration> AllClasses()
    {
        foreach (var @class in BuiltinClasses.Values)
        {
            yield return @class;
        }

        foreach (var @class in ParsedClasses.Values)
        {
            yield return @class;
        }
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

    public bool IsValid(TypeReference typeReference)
    {
        if (typeReference.IsGeneric)
        {
            return true;
        }
        
        return HasClass(typeReference.Name) && typeReference.GenericTypes.All(IsValid);
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
        
        return ParseSpecializedGenericType(type);
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

    private static void CreateAllMembers(ClassDeclaration declaration, ParsedClassData parsedClass)
    {

    }
}