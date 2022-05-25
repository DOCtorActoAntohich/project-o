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
using DomExpression = OCompiler.Analyze.SemanticsV2.Dom.Expression.Expression;
using Boolean = OCompiler.Builtins.Primitives.Boolean;
using String = OCompiler.Builtins.Primitives.String;

namespace OCompiler.Analyze.SemanticsV2;

internal partial class AnnotatedSyntaxTreeV2
{
    

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