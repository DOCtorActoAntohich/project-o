using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Exceptions;
using OCompiler.Exceptions.Semantic;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;
using Boolean = OCompiler.Builtins.Primitives.Boolean;
using String = OCompiler.Builtins.Primitives.String;

namespace OCompiler.Analyze.SemanticsV2;

internal partial class AnnotatedSyntaxTreeV2
{
    private readonly Dictionary<MemberField, ValidationState> _fieldsValidationState = new();
    
    private void InferTypes()
    {
        foreach (var @class in ParsedClasses.Values)
        {
            foreach (var field in @class.Fields)
            {
                _fieldsValidationState.Add(field, ValidationState.Untouched);
            }
        }
        
        foreach (var @class in ParsedClasses.Values)
        {
            ValidateClass(@class);
        }
    }
    
    private void ValidateClass(ClassDeclaration @class)
    {
        ValidateOwnGenericTypes(@class);
        
        ValidateBaseTypeReference(@class);
        
        ValidateConstructorsSignature(@class);
        ValidateMethodsSignature(@class);

        ValidateFields(@class);
    }

    private void ValidateOwnGenericTypes(ClassDeclaration @class)
    {
        var typesToValidate = new List<TypeReference>(@class.GenericTypes);
        @class.GenericTypes.Clear();
        foreach (var genericType in typesToValidate)
        {
            ValidateOwnGenericType(@class, genericType);
            genericType.IsGeneric = true;
            @class.GenericTypes.Add(genericType);
        }
    }

    private void ValidateOwnGenericType(ClassDeclaration @class, TypeReference type)
    {
        if (type.IsGeneric && type.HasGenerics)
        {
            throw new GenericTypeSpecializationError(@class, type);
        }

        if (HasClass(type.Name))
        {
            throw new AnalyzeError($"Generic type `{type.Name}` is already defined as a concrete type");
        }
        
        if (@class.HasGenericType(type.Name))
        {
            throw new AnalyzeError($"In class {@class.Name}: Duplicate generic type: {type}");
        }
    }
    
    private void ValidateBaseTypeReference(ClassDeclaration @class)
    {
        ValidateTypeReference(@class, @class.BaseType!);
    }
    
    private void ValidateTypeReference(ClassDeclaration currentClass, TypeReference type)
    {
        if (type.IsGeneric && type.HasGenerics)
        {
            throw new GenericTypeSpecializationError(currentClass, type);
        }
        
        var isDefinedInClass = currentClass.HasGenericType(type.Name);
        if (type.IsGeneric && isDefinedInClass)
        {
            return;
        }
        
        if (isDefinedInClass ^ type.IsGeneric)
        {
            throw new CompilerInternalError($"Failed to build a type reference: {type}");
        }

        var isClassName = HasClass(type.Name);
        if (!isClassName)
        {
            throw new AnalyzeError($"The type is not a name of a class: {type}");
        }

        var referredClass = GetClass(type.Name);
        var nRequiredGenerics = referredClass.GenericTypes.Count;
        var nProvidedGenerics = type.GenericTypes.Count;
        if (nProvidedGenerics != nRequiredGenerics)
        {
            throw new AnalyzeError(
                $"Class {referredClass.Name} has {nRequiredGenerics} generic types, " +
                $"but {nProvidedGenerics} were provided: {type}");
        }

        foreach (var innerReference in type.GenericTypes)
        {
            ValidateTypeReference(currentClass, innerReference);
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
    
    private void ValidateCallableParameters(ClassDeclaration @class, CallableMember callableMember)
    {
        foreach (var parameter in callableMember.Parameters)
        {
            if (callableMember.Statements.VariableTable.Has(parameter.Name))
            {
                throw new DuplicateParameterError(@class, callableMember, parameter.Name);
            }

            ValidateTypeReference(@class, parameter.Type);
            
            callableMember.Statements.VariableTable.Add(parameter.Name, parameter.Type);
        }
    }
    
    private void ValidateMethodReturnType(ClassDeclaration @class, MemberMethod method)
    {
        ValidateTypeReference(@class, method.ReturnType);
    }
}