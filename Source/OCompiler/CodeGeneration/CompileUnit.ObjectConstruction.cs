using System;
using System.CodeDom;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Callable;
using DomBase = OCompiler.StandardLibrary.CodeDom.Base;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit
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
        _currentCallable = callableInfo switch
        {
            ParsedMethodInfo methodInfo => DomBase.EmptyPublicMethod(methodInfo.ReturnType, methodInfo.Name),
            ParsedConstructorInfo => DomBase.EmptyPublicConstructor(),
            _ => throw new Exception($"Unknown callable")
        };
        _currentCallableInfo = callableInfo;
        
        FillParsedCallable(callableInfo);
        
        _currentTypeDeclaration.Members.Add(_currentCallable);

        _currentCallable = null;
        _currentCallableInfo = null;
    }

    private void FillParsedCallable(CallableInfo callableInfo)
    {
        if (_currentCallable == null)
        {
            throw new Exception("Somehow the current callable to fill is null...");
        }
        
        foreach (var parameter in callableInfo.Parameters)
        {
            var p = new CodeParameterDeclarationExpression(parameter.Type, parameter.Name);
            _currentCallable.Parameters.Add(p);
        }

        foreach (var statement in ParsedBody(callableInfo.Body))
        {
            _currentCallable.Statements.Add(statement);
        }
    }
}