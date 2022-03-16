using System;
using System.CodeDom;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Callable;
using DomBase = OCompiler.StandardLibrary.CodeDom.Base;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit2
{
    private void AddParsedClassField(ParsedFieldInfo parsedField)
    {
        var field = new CodeMemberField
        {
            Name = parsedField.Name,
            Type = new CodeTypeReference(parsedField.Type),
            Attributes = MemberAttributes.Public,
            InitExpression = ParsedRvalueExpression(parsedField.Expression.Expression)
        };
        
        _currentTypeDeclaration.Members.Add(field);
    }

    private void AddParsedCallable(CallableInfo callableInfo)
    {
        var callable = callableInfo switch
        {
            ParsedMethodInfo methodInfo => DomBase.EmptyPublicMethod(methodInfo.ReturnType, methodInfo.Name),
            ParsedConstructorInfo => DomBase.EmptyPublicConstructor(),
            _ => throw new Exception($"Unknown callable")
        };
        
        callable.FillParsedCallable(callableInfo);
        
        _currentTypeDeclaration.Members.Add(callable);
    }

    private void FillParsedCallable(CallableInfo callableInfo)
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