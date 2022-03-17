using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Semantics.Class;

namespace OCompiler.CodeGeneration;

internal partial class CompileUnit
{
    private void AddEntryPoint(string mainClass)
    {
        var classInfo = ParsedClassInfo.GetByName(mainClass);
        if (classInfo is not ParsedClassInfo)
        {
            throw new Exception($"{mainClass} is a bad entry point.");
        }

        var pc = (ParsedClassInfo) classInfo;
        if (pc.Constructors.Any(ctor => ctor.Parameters.Count == 0 && ctor.Body.IsEmpty))
        {
            throw new Exception($"A chosen constructor of {mainClass} has empty default constructor");
        }
        
        var entryClass = new CodeTypeDeclaration
        {
            Name = $"reserved_entry_point_for_{mainClass}",
            Attributes = MemberAttributes.Public | MemberAttributes.Static
        };
        var entryPoint = new CodeEntryPointMethod
        {
            Attributes = MemberAttributes.Public | MemberAttributes.Static,
            ReturnType = new CodeTypeReference(typeof(void))
        };

        var newInstance = new CodeObjectCreateExpression(new CodeTypeReference(mainClass));
        entryPoint.Statements.Add(newInstance);
        
        entryClass.Members.Add(entryPoint);
        
        _codeNamespace.Types.Add(entryClass);
    }
}