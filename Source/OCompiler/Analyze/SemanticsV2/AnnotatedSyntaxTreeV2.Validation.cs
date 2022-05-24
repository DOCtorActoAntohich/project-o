using System;
using System.Linq;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Exceptions;

namespace OCompiler.Analyze.SemanticsV2;

internal partial class AnnotatedSyntaxTreeV2
{
    private void ValidateTree()
    {
        foreach (var @class in ParsedClasses.Values)
        {
            ValidateClass(@class);
        }
    }

    private void ValidateClass(ClassDeclaration @class)
    {
        ValidateBaseTypeReference(@class);
        
        foreach (var field in @class.Fields)
        {
            
        }
    }

    private void ValidateBaseTypeReference(ClassDeclaration @class)
    {
        ValidateTypeReference(@class, @class.BaseType!);
    }

    private void ValidateTypeReference(ClassDeclaration thisClass, TypeReference typeReference)
    {
        if (typeReference.IsGeneric && typeReference.HasGenerics)
        {
            throw new AnalyzeError($"The generic type cannot have specialization: {typeReference}");
        }
        
        var isClassGenericType = thisClass.HasGenericType(typeReference.Name);
        if (isClassGenericType && typeReference.IsGeneric)
        {
            return;
        }
        
        if (isClassGenericType ^ typeReference.IsGeneric)
        {
            throw new CompilerInternalError($"Failed to build a type reference: {typeReference}");
        }

        var isClassName = HasClass(typeReference.Name);
        if (!isClassName)
        {
            throw new AnalyzeError($"The type is not a name of a class: {typeReference}");
        }

        var referredClass = GetClass(typeReference.Name);
        var nRequiredGenerics = referredClass.GenericTypes.Count;
        var nProvidedGenerics = typeReference.GenericTypes.Count;
        if (nProvidedGenerics != nRequiredGenerics)
        {
            throw new AnalyzeError(
                $"Class {referredClass.Name} has {nRequiredGenerics} generic types, " +
                $"but {nProvidedGenerics} were provided: {typeReference}");
        }

        foreach (var innerReference in typeReference.GenericTypes)
        {
            ValidateTypeReference(thisClass, innerReference);
        }
    }
    
    private void ValidateField(MemberField field)
    {
        
    }
}