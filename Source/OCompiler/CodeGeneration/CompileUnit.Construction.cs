using System;
using System.CodeDom;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Callable;
using OCompiler.StandardLibrary.CodeDom;

namespace OCompiler.CodeGeneration;

internal static partial class CompileUnit
{
    private static void AddParsedClassField(this CodeTypeDeclaration typeDeclaration, ParsedFieldInfo parsedField)
    {
        var field = new CodeMemberField
        {
            Name = parsedField.Name,
            Type = new CodeTypeReference(parsedField.Type),
            Attributes = MemberAttributes.Public,
            InitExpression = ParseRvalueExpression(parsedField.Expression.Expression)
        };
        
        typeDeclaration.Members.Add(field);
    }

    private static void AddParsedCallable(this CodeTypeDeclaration typeDeclaration, CallableInfo callableInfo)
    {
        var callable = callableInfo switch
        {
            ParsedMethodInfo methodInfo => Base.EmptyPublicMethod(methodInfo.ReturnType, methodInfo.Name),
            ParsedConstructorInfo => Base.EmptyPublicConstructor(),
            _ => throw new Exception($"Unknown callable")
        };
        
        callable.FillParsedCallable(callableInfo);
        
        typeDeclaration.Members.Add(callable);
    }

    private static void FillParsedCallable(this CodeMemberMethod callable, CallableInfo callableInfo)
    {
        foreach (var parameter in callableInfo.Parameters)
        {
            var p = new CodeParameterDeclarationExpression(parameter.Type, parameter.Name);
            callable.Parameters.Add(p);
        }

        foreach (var statement in ParsedBody(callableInfo.Body))
        {
            callable.Statements.Add(statement);
        }
    }
}