using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.NameReference;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Primitive;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Builtins.Primitives;
using OCompiler.Exceptions;
using OCompiler.Exceptions.Semantic;
using Boolean = OCompiler.Builtins.Primitives.Boolean;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;
using String = OCompiler.Builtins.Primitives.String;

namespace OCompiler.Analyze.SemanticsV2;

internal partial class AnnotatedSyntaxTreeV2
{
    private Dictionary<MemberField, ValidationState> _fieldsValidationState = new();

    private void ValidateTree()
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
            ValidateFieldType(field);
        }

        foreach (var field in @class.Fields)
        {
            if (!IsUnique(@class.Fields, field))
            {
                throw new AnalyzeError($"Duplicate field: {@class.Name}::{field.Name}");
            }
        }
    }

    private void ValidateFieldType(MemberField field)
    {
        if (_fieldsValidationState[field] == ValidationState.Valid)
        {
            return;
        }

        if (_fieldsValidationState[field] == ValidationState.InProgress)
        {
            throw new AnalyzeError($"Circular dependency while resolving type" +
                                   $"of field {field.Owner!.Name}::{field.Name}");
        }

        _fieldsValidationState[field] = ValidationState.InProgress;
        if (field.InitExpression == null && field.Type == null)
        {
            throw new AnalyzeError($"Cannot determine type of field {field.Name} (in class {field.Owner!.Name})");
        }

        if (field.InitExpression == null && field.Type != null)
        {
            ValidateTypeReference(field.Owner!, field.Type);
            field.InitExpression = new ObjectCreateExpression(field.Type);
            _fieldsValidationState[field] = ValidationState.Valid;
            return;
        }

        if (field.InitExpression != null && field.Type == null)
        {
            field.Type = GetExpressionType(field.InitExpression);
            ValidateTypeReference(field.Owner!, field.Type);
            _fieldsValidationState[field] = ValidationState.Valid;
            return;
        }

        var expressionType = GetExpressionType(field.InitExpression!);
        ValidateTypeReference(field.Owner!, field.Type!);
        if (field.Type!.DifferentFrom(expressionType))
        {
            throw new AnalyzeError($"Cannot unambiguously determine type" +
                                   $"of field {field.Owner!.Name}{field.Name}");
        }

        _fieldsValidationState[field] = ValidationState.Valid;
    }

    private TypeReference GetExpressionType(DomExpression expression)
    {
        return expression switch
        {
            IntegerLiteralExpression => new TypeReference(nameof(Integer)),
            RealLiteralExpression => new TypeReference(nameof(Real)),
            BooleanLiteralExpression => new TypeReference(nameof(Boolean)),
            StringLiteralExpression => new TypeReference(nameof(String)),
            MethodCallExpression methodCallExpression => GetMethodCallType(methodCallExpression),
            ObjectCreateExpression objectCreateExpression => GetObjectCreationType(objectCreateExpression),
            FieldReferenceExpression fieldReferenceExpression => GetFieldReferenceType(fieldReferenceExpression),
            _ => throw new AnalyzeError($"Impossible to initialize field with this expression: {expression}")
        };
    }

    private List<TypeReference> GetTypesOfArguments(List<Expression> arguments)
    {
        return arguments.Select(GetExpressionType).ToList();
    }
    
    private TypeReference GetMethodCallType(MethodCallExpression call)
    {
        var sourceObjectType = GetExpressionType(call.SourceObject!);
        var @class = GetClass(sourceObjectType.Name);

        var argumentTypes = GetTypesOfArguments(call.Arguments);
        var method = @class.GetMethod(call.Name, argumentTypes);

        return method.ReturnType!;
    }

    private TypeReference GetObjectCreationType(ObjectCreateExpression objectCreation)
    {
        if (!HasClass(objectCreation.Name))
        {
            throw new AnalyzeError($"Unknown type name: {objectCreation.Name}");
        }

        var @class = GetClass(objectCreation.Name); 

        var argumentTypes = GetTypesOfArguments(objectCreation.Arguments);
        var constructor = @class.GetConstructor(argumentTypes);

        var typeReference = new TypeReference(constructor.Owner!.Name);
        typeReference.GenericTypes.AddRange(objectCreation.GenericTypes);

        return typeReference;
    }

    private TypeReference GetFieldReferenceType(FieldReferenceExpression fieldReference)
    {
        var sourceObjectType = GetExpressionType(fieldReference.SourceObject!);
        var @class = GetClass(sourceObjectType.Name);

        var field = @class.GetField(fieldReference.Name);
        
        ValidateFieldType(field);
        return field.Type!;
    }
}