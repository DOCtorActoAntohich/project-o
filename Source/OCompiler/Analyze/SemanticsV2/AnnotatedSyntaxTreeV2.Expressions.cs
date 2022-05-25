using System;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.NameReference;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Primitive;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Builtins.Primitives;
using OCompiler.Exceptions;
using OCompiler.Exceptions.Semantic;
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;
using Boolean = OCompiler.Builtins.Primitives.Boolean;
using String = OCompiler.Builtins.Primitives.String;

namespace OCompiler.Analyze.SemanticsV2;

internal partial class AnnotatedSyntaxTreeV2
{
    private readonly Dictionary<MemberField, ValidationState> _fieldsValidationState = new();
    
    private void ValidateClassMembers()
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
        
        ValidateConstructorSignatures(@class);
        ValidateMethodSignatures(@class);

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
    
    private int CountCallables(IEnumerable<CallableMember> callables, CallableMember target)
    {
        return callables.Count(callable => callable.SameSignatureAs(target));
    }
    
    private int CountFields(IEnumerable<MemberField> fields, MemberField target)
    {
        return fields.Count(field => field.SameNameAs(target));
    }

    private bool IsUnique(IEnumerable<CallableMember> callables, CallableMember target)
    {
        return CountCallables(callables, target) == 1;
    }

    private bool IsUnique(IEnumerable<MemberField> fields, MemberField target)
    {
        return CountFields(fields, target) == 1;
    }
    
    private void ValidateConstructorSignatures(ClassDeclaration @class)
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
    
    private void ValidateMethodSignatures(ClassDeclaration @class)
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
    
    private void ValidateFields(ClassDeclaration @class)
    {
        foreach (var field in @class.Fields)
        {
            DetermineFieldType(field);
        }

        foreach (var field in @class.Fields)
        {
            if (!IsUnique(@class.Fields, field))
            {
                throw new AnalyzeError($"Duplicate field: {@class.Name}::{field.Name}");
            }
        }
    }
    
    private void DetermineFieldType(MemberField field)
    {
        if (_fieldsValidationState[field] == ValidationState.Valid)
        {
            return;
        }

        if (_fieldsValidationState[field] == ValidationState.InProgress)
        {
            throw new AnalyzeError(
                $"Circular dependency while resolving type of field {field.Owner!.Name}::{field.Name}");
        }

        _fieldsValidationState[field] = ValidationState.InProgress;
        
        if (!field.HasTypeAnnotation && !field.HasInitExpression)
        {
            throw new AnalyzeError(
                $"The type of field {field.Owner!.Name}{field.Name} was not specified.");
        }
        
        if (!field.HasTypeAnnotation && field.HasInitExpression)
        {
            DetermineExpressionType(field.InitExpression);
            field.Type = field.InitExpression.Type;
            _fieldsValidationState[field] = ValidationState.Valid;
            return;
        }
        
        if (field.HasTypeAnnotation && !field.HasInitExpression)
        {
            ValidateTypeReference(field.Owner!, field.Type);
            field.InitExpression = new ObjectCreateExpression(field.Type);
            DetermineExpressionType(field.InitExpression);
            _fieldsValidationState[field] = ValidationState.Valid;
            return;
        }

        DetermineExpressionType(field.InitExpression);
        ValidateTypeReference(field.Owner!, field.Type);
        if (field.Type.DifferentFrom(field.InitExpression.Type))
        {
            throw new AnalyzeError(
                $"Types of annotation and init expression don't match: {field.Owner!.Name}::{field.Name}");
        }

        _fieldsValidationState[field] = ValidationState.Valid;
    }
    
    private void DetermineExpressionType(DomExpression expression)
    {
        switch (expression)
        {
            case IntegerLiteralExpression:
                expression.Type = new TypeReference(nameof(Integer));
                break;
            
            case RealLiteralExpression:
                expression.Type = new TypeReference(nameof(Real));
                break;
            
            case BooleanLiteralExpression:
                expression.Type = new TypeReference(nameof(Boolean));
                break;
            
            case StringLiteralExpression:
                expression.Type = new TypeReference(nameof(String));
                break;
            
            case MethodCallExpression methodCallExpression:
                DetermineMethodCallType(methodCallExpression);
                break;
            
            case ObjectCreateExpression objectCreateExpression:
                DetermineObjectCreationType(objectCreateExpression);
                break;
            
            case FieldReferenceExpression fieldReferenceExpression:
                DetermineFieldReferenceType(fieldReferenceExpression);
                break;
            
            case ThisReferenceExpression thisReferenceExpression:
                DetermineThisReferenceType(thisReferenceExpression);
                break;
            
            case BaseConstructorCallExpression baseConstructorCallExpression:
                DetermineBaseConstructorCall(baseConstructorCallExpression);
                break;
            
            case VariableReferenceExpression variableReferenceExpression:
                DetermineVariableType(variableReferenceExpression);
                break;

            default:
                throw new AnalyzeError($"Impossible to initialize field with this expression: {expression}");
        }
    }
    
    private List<TypeReference> GetTypesOfArguments(List<DomExpression> arguments)
    {
        foreach (var argument in arguments)
        {
            DetermineExpressionType(argument);
        }

        return arguments.Select(arg => arg.Type).ToList();
    }
    
    private void DetermineMethodCallType(MethodCallExpression call)
    {
        DetermineExpressionType(call.SourceObject);
        var @class = GetClass(call.SourceObject.Type.Name);

        var argumentTypes = GetTypesOfArguments(call.Arguments);
        var method = @class.GetMethod(call.Name, argumentTypes);

        call.Type = method.ReturnType;
        call.Method = method;
    }
    
    private void DetermineObjectCreationType(ObjectCreateExpression objectCreation)
    {
        if (!HasClass(objectCreation.Name))
        {
            throw new AnalyzeError($"Unknown type name: {objectCreation.Name}");
        }

        var @class = GetClass(objectCreation.Name); 

        var argumentTypes = GetTypesOfArguments(objectCreation.Arguments);
        var constructor = @class.GetConstructor(argumentTypes);

        objectCreation.Type = new TypeReference(constructor.Owner!.Name);
        objectCreation.Type.GenericTypes.AddRange(objectCreation.GenericTypes);
    }

    private void DetermineFieldReferenceType(FieldReferenceExpression fieldReference)
    {
        DetermineExpressionType(fieldReference.SourceObject);
        
        var @class = GetClass(fieldReference.SourceObject.Type.Name);

        var field = @class.GetField(fieldReference.Name);
        
        DetermineFieldType(field);

        fieldReference.Type = field.Type;
        fieldReference.Field = field;
    }

    private void DetermineThisReferenceType(ThisReferenceExpression thisReference)
    {
        var @class = thisReference.ParentStatement.RootHolder.Owner!;

        thisReference.Type = new TypeReference(@class.Name);
    }

    private void DetermineBaseConstructorCall(BaseConstructorCallExpression baseCall)
    {
        var rootHolder = baseCall.ParentStatement.RootHolder;
        if (rootHolder is MemberMethod)
        {
            throw new AnalyzeError("Cannot call to base constructor from inside the method");
        }

        var thisClass = rootHolder.Owner!;
        var baseClass = GetClass(thisClass.BaseType!.Name);

        var argumentTypes = GetTypesOfArguments(baseCall.Arguments);
        var calledConstructor = baseClass.GetConstructor(argumentTypes);

        baseCall.Constructor = calledConstructor;
    }

    private void DetermineVariableType(VariableReferenceExpression variableReference)
    {
        var statement = variableReference.ParentStatement;
        var variableTable = statement.ParentBody.VariableTable;
        if (!variableTable.Has(variableReference.Name))
        {
            throw new AnalyzeError($"Referenced an unknown variable: {variableReference.Name}");
        }

        variableReference.Type = variableTable.GetType(variableReference.Name);
    }
}