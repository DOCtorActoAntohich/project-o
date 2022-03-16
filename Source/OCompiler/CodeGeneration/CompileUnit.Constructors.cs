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
    
    private static void AddParsedClassConstructor(
        this CodeTypeDeclaration typeDeclaration,
        ParsedConstructorInfo parsedCtor)
    {
        var ctor = Base.EmptyPublicConstructor();

        foreach (var statement in parsedCtor.Body)
        {
            // todo rm
            Console.WriteLine(statement);
            var parsedStatement = ParseCodeStatement(statement);
            ctor.Statements.Add(parsedStatement);
        }

        typeDeclaration.Members.Add(ctor);
    }
}