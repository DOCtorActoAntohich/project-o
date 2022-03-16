using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Analyze.Semantics.Expression;
using OCompiler.Analyze.Syntax.Declaration.Expression;
using DomAnyRef = OCompiler.StandardLibrary.CodeDom.Reference.AnyRef;
using DomIO     = OCompiler.StandardLibrary.CodeDom.Reference.IO;
using DomString = OCompiler.StandardLibrary.CodeDom.Reference.String;

using DomAnyVal = OCompiler.StandardLibrary.CodeDom.Value.AnyValue;
using DomBool   = OCompiler.StandardLibrary.CodeDom.Value.Boolean;
using DomInt    = OCompiler.StandardLibrary.CodeDom.Value.Integer;
using DomReal   = OCompiler.StandardLibrary.CodeDom.Value.Real;

namespace OCompiler.CodeGeneration;

internal static class CompileUnit
{
    public const string ResultingNamespace = "OLang";

    
    public static CodeCompileUnit FromAnnotatedSyntaxTree(TreeValidator ast)
    {
        var compileUnit = new CodeCompileUnit();
        
        var @namespace = new CodeNamespace(ResultingNamespace);
        @namespace.AddAllClasses(ast);
        compileUnit.Namespaces.Add(@namespace);
        
        return compileUnit;
    }

    
    private static void AddAllClasses(this CodeNamespace @namespace, TreeValidator ast)
    {
        foreach (var classInfo in ast.ValidatedClasses)
        {
            @namespace.AddClassInfo(classInfo);
        }
    }
    
    private static void AddClassInfo(this CodeNamespace @namespace, ClassInfo classInfo)
    {
        var typeDeclaration = new CodeTypeDeclaration(classInfo.Name)
        {
            Attributes = MemberAttributes.Public
        };

        if (classInfo.IsValueType())
        {
            typeDeclaration.IsClass  = false;
            typeDeclaration.IsStruct = true;
        }
        else if (classInfo.BaseClass != null)
        {
            typeDeclaration.IsClass  = true;
            typeDeclaration.IsStruct = false;
            typeDeclaration.AddBaseClass(classInfo);
        }


        switch (classInfo)
        {
            case BuiltClassInfo builtInClass:
                @namespace.Types.Add(GetBuiltClass(builtInClass));
                break;
            
            case ParsedClassInfo newClass:
                typeDeclaration.AddParsedClassContents(newClass);
                break;
            
            default:
                throw new Exception($"Class `{classInfo.Name}` was not found in StdLib, nor in OLang file.");
        }
        
        @namespace.Types.Add(typeDeclaration);
    }

    private static void AddBaseClass(this CodeTypeDeclaration typeDeclaration, ClassInfo classInfo)
    {
        var parent = classInfo.BaseClass switch
        {
            ClassInfo parentInfo => new CodeTypeReference(parentInfo.Name),
            Type type            => new CodeTypeReference(type),
            _ => new CodeTypeReference(typeof(object))
        };
        
        typeDeclaration.BaseTypes.Add(parent);
    }

    private static CodeTypeDeclaration GetBuiltClass(BuiltClassInfo builtClassInfo)
    {
        return builtClassInfo.Name switch
        {
            DomAnyRef.TypeName => DomAnyRef.Generate(),
            DomString.TypeName => DomString.Generate(),
            DomIO.TypeName     => DomIO.Generate(),
            
            DomAnyVal.TypeName => DomAnyVal.Generate(),
            DomBool.TypeName   => DomBool.Generate(),
            DomInt.TypeName    => DomInt.Generate(),
            DomReal.TypeName   => DomReal.Generate(),
            
            _ => throw new Exception($"SUS! This class is not found among Built-Ins: {builtClassInfo.Name}")
        };
    }

    private static void AddParsedClassContents(this CodeTypeDeclaration typeDeclaration, ParsedClassInfo classInfo)
    {
        foreach (var field in classInfo.Fields)
        { 
            typeDeclaration.AddParsedClassFields(field);
        }
    }

    private static void AddParsedClassFields(this CodeTypeDeclaration typeDeclaration, ParsedFieldInfo parsedField)
    {
        var field = new CodeMemberField
        {
            Name = parsedField.Name,
            Type = new CodeTypeReference(parsedField.Type),
            Attributes = MemberAttributes.Public,
            InitExpression = ParseVariableInitExpression(parsedField.Expression.Expression)
        };
        
        typeDeclaration.Members.Add(field);
    }

    private static CodeExpression ParseVariableInitExpression(Expression expression)
    {
        return expression switch
        {
            Call call => ParseConstructorCall(call),
            not null => new CodePrimitiveExpression(expression.Token),
            _ => throw new Exception("Uncool init expression XCG1")
        };
    }

    private static CodeExpression ParseConstructorCall(Call call)
    {
        var type = new CodeTypeReference(call.Token.Literal);
        var arguments = ParseCallArgumentList(call.Arguments);

        var ctorCall = new CodeObjectCreateExpression(type, arguments);

        var chainedAccess = ParseChainAccess(ctorCall, call.Child);
        return chainedAccess;
    }

    private static CodeExpression[] ParseCallArgumentList(IEnumerable<Expression> arguments)
    {
        return arguments.Select(ParseVariableInitExpression).ToArray();
    }

    private static CodeExpression ParseChainAccess(CodeExpression starterObject, Expression? expression)
    {
        switch (expression)
        {
            case null:
                return starterObject;
            
            case Call call:
            {
                var objectAfterCall = new CodeMethodInvokeExpression(
                    starterObject, call.Token.Literal, ParseCallArgumentList(call.Arguments));
                return ParseChainAccess(objectAfterCall, call.Child);
            }

            default:
            {
                var accessedField = new CodeFieldReferenceExpression(starterObject, expression.Token.Literal);
                return ParseChainAccess(accessedField, expression.Child);
            }
        }
    }
}