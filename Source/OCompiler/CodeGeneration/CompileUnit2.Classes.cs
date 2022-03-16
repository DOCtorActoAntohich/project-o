using System;
using System.CodeDom;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Semantics.Class;
using DomAnyRef = OCompiler.StandardLibrary.CodeDom.Reference.AnyRef;
using DomIO     = OCompiler.StandardLibrary.CodeDom.Reference.IO;
using DomString = OCompiler.StandardLibrary.CodeDom.Reference.String;
using DomAnyVal = OCompiler.StandardLibrary.CodeDom.Value.AnyValue;
using DomBool   = OCompiler.StandardLibrary.CodeDom.Value.Boolean;
using DomInt    = OCompiler.StandardLibrary.CodeDom.Value.Integer;
using DomReal   = OCompiler.StandardLibrary.CodeDom.Value.Real;
using DomVoid   = OCompiler.StandardLibrary.CodeDom.Value.Void;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit2
{
    private void AddAllClasses(TreeValidator ast)
    {
        foreach (var classInfo in ast.ValidatedClasses)
        {
            AddClassInfo(classInfo);
        }
    }
    
    private void AddClassInfo(ClassInfo classInfo)
    {
        _typeDeclaration = new CodeTypeDeclaration(classInfo.Name)
        {
            Attributes = MemberAttributes.Public
        };
        

        if (classInfo.IsValueType())
        {
            _typeDeclaration.IsClass  = false;
            _typeDeclaration.IsStruct = true;
        }
        else if (classInfo.BaseClass != null)
        {
            _typeDeclaration.IsClass  = true;
            _typeDeclaration.IsStruct = false;
            AddBaseClass(classInfo);
        }


        switch (classInfo)
        {
            case BuiltClassInfo builtInClass:
                _codeNamespace.Types.Add(GetBuiltClass(builtInClass));
                break;

            case ParsedClassInfo newClass:
                AddParsedClassContents(newClass);
                break;
            
            default:
                throw new Exception($"Class `{classInfo.Name}` was not found in StdLib, nor in OLang file.");
        }
        
        _codeNamespace.Types.Add(_typeDeclaration);
    }
    
    private void AddBaseClass(ClassInfo classInfo)
    {
        var parent = classInfo.BaseClass switch
        {
            ClassInfo parentInfo => new CodeTypeReference(parentInfo.Name),
            _ => new CodeTypeReference(typeof(object))
        };
        
        _typeDeclaration.BaseTypes.Add(parent);
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
            
            DomVoid.TypeName   => DomVoid.Generate(), // bad.
            
            _ => throw new Exception($"SUS! This class is not found among Built-Ins: {builtClassInfo.Name}")
        };
    }
    
    private void AddParsedClassContents(ParsedClassInfo classInfo)
    {
        foreach (var field in classInfo.Fields)
        {
            _typeDeclaration.AddParsedClassField(field);
        }
        
        foreach (var constructor in classInfo.Constructors)
        {
            _typeDeclaration.AddParsedCallable(constructor);
        }

        foreach (var method in classInfo.Methods)
        {
            _typeDeclaration.AddParsedCallable(method);
        }
    }
}