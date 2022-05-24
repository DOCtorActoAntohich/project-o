using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Exceptions;
using OCompiler.Exceptions.Semantic;

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
        
        ValidateConstructorsSignature(@class);
        ValidateMethodsSignature(@class);

        ValidateFields(@class);
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

    private void ValidateConstructorsSignature(ClassDeclaration @class)
    {
        foreach (var constructor in @class.Constructors)
        {
            ValidateCallableParameters(@class, constructor);
        }

        foreach (var constructor in @class.Constructors)
        {
            if (!IsUnique(@class.Constructors, constructor))
            {
                throw new AnalyzeError($"Duplicate constructor: {constructor}");
            }
        }
    }

    private void ValidateMethodsSignature(ClassDeclaration @class)
    {
        foreach (var method in @class.Methods)
        {
            ValidateCallableParameters(@class, method);
            ValidateMethodReturnType(@class, method);
        }
        
        foreach (var constructor in @class.Constructors)
        {
            if (!IsUnique(@class.Constructors, constructor))
            {
                throw new AnalyzeError($"Duplicate method: {constructor}");
            }
        }
    }
    
    private int CountCallable(IEnumerable<CallableMember> callables, CallableMember target)
    {
        return callables.Count(callable => callable.SameSignatureAs(target));
    }
    
    private int CountFields(IEnumerable<MemberField> fields, MemberField target)
    {
        return fields.Count(field => field.SameNameAs(target));
    }

    private bool IsUnique(IEnumerable<CallableMember> callables, CallableMember target)
    {
        return CountCallable(callables, target) == 1;
    }

    private bool IsUnique(IEnumerable<MemberField> fields, MemberField target)
    {
        return CountFields(fields, target) == 1;
    }
    
    private void ValidateCallableParameters(ClassDeclaration @class, CallableMember constructor)
    {
        foreach (var parameter in constructor.Parameters)
        {
            if (constructor.Context.ContainsKey(parameter.Name))
            {
                throw new AnalyzeError($"Duplicate parameter: {parameter.Name}");
            }
            
            ValidateTypeReference(@class, parameter.Type);
            
            constructor.Context.Add(parameter.Name, parameter.Type);
        }
    }

    private void ValidateMethodReturnType(ClassDeclaration @class, MemberMethod method)
    {
        method.ReturnType ??= new TypeReference("Void");
        
        ValidateTypeReference(@class, method.ReturnType);
    }

    private void ValidateFields(ClassDeclaration @class)
    {
        foreach (var field in @class.Fields)
        {
            ValidateField(@class, field);
        }

        foreach (var field in @class.Fields)
        {
            if (!IsUnique(@class.Fields, field))
            {
                throw new AnalyzeError($"Duplicate field: {@class.Name}::{field.Name}");
            }
        }
    }

    private void ValidateField(ClassDeclaration @class, MemberField field)
    {
        if (field.InitExpression == null)
        {
            throw new AnalyzeError("Impossible to create field with no initializer");
        }
        
        // TODO ValidateExpression(field).
        // TODO if (field.InitExpression.ReturnType == null) { throw new AnalyzeError($"Cannot explicitly determine field type: {field.Name}"); }
        
        if (field.Type != null)
        {
            ValidateTypeReference(@class, field.Type);
        }
        
        // TODO if (field.Type != null && field.Type != field.InitExpression.Type)
        
        
    }
}