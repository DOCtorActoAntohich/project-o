using System;
using System.CodeDom;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Class;

namespace OCompiler.CodeGeneration;

internal static class CompileUnit
{
    public const string ResultingNamespace = "OLang";

    
    public static CodeCompileUnit FromAnnotatedSyntaxTree(TreeValidator ast)
    {
        var compileUnit = new CodeCompileUnit();
        
        var @namespace = new CodeNamespace(ResultingNamespace);
        @namespace.AddAllClasses(ast);
        compileUnit.Namespaces.Add(@namespace);
        
        return compileUnit;
    }

    
    private static void AddAllClasses(this CodeNamespace @namespace, TreeValidator ast)
    {
        foreach (var classInfo in ast.ValidatedClasses)
        {
            @namespace.AddClassInfo(classInfo);
        }
    }
    
    private static void AddClassInfo(this CodeNamespace @namespace, ClassInfo classInfo)
    {
        var typeDeclaration = new CodeTypeDeclaration(classInfo.Name)
        {
            Attributes = MemberAttributes.Public
        };

        if (classInfo.IsValueType())
        {
            typeDeclaration.IsClass  = false;
            typeDeclaration.IsStruct = true;
        }
        else if (classInfo.BaseClass != null)
        {
            typeDeclaration.IsClass  = true;
            typeDeclaration.IsStruct = false;
            typeDeclaration.AddBaseClass(classInfo);
        }


        switch (classInfo)
        {
            case BuiltClassInfo: // They'll be generated differently, in another place.
                break;
            
            case ParsedClassInfo newClass:
                typeDeclaration.AddParsedClassContents(newClass);
                break;
            
            default:
                throw new Exception($"Class `{classInfo.Name}` was not found in StdLib, nor in OLang file.");
        }
        
        @namespace.Types.Add(typeDeclaration);
    }

    private static void AddBaseClass(this CodeTypeDeclaration typeDeclaration, ClassInfo classInfo)
    {
        var parent = classInfo.BaseClass switch
        {
            ClassInfo parentInfo => new CodeTypeReference(parentInfo.Name),
            Type type => new CodeTypeReference(type),
            _ => new CodeTypeReference(typeof(object))
        };
        
        typeDeclaration.BaseTypes.Add(parent);
    }

    private static void AddParsedClassContents(this CodeTypeDeclaration typeDeclaration, ParsedClassInfo classInfo)
    {
        
    }
}