using System;
using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.SemanticsV2.Callable;
using OCompiler.Analyze.SemanticsV2.Expression;
using OCompiler.StandardLibrary.Type;
using OCompiler.Utils;
using ClassDeclaration = OCompiler.Analyze.Syntax.Declaration.Class.Class;

namespace OCompiler.Analyze.SemanticsV2;

internal class ClassInfo : IEquatable<ClassInfo>
{
    public string Name { get; set; }

    public ClassInfo? Parent { get; set; }

    public List<VariableDeclaration> Fields { get; set; } = new();
    public List<MethodDeclaration> Methods { get; set; } = new();
    public List<MethodDeclaration> Constructors { get; set; } = new();

    // public List<string> GenericTypes { get; set; } = new();
    // public bool IsGeneric => GenericTypes.Count > 0;


    public void AddField(string name, TypeReference type)
    {
        
    }
    
    public VariableDeclaration? GetField(string name)
    {
        return Fields.FirstOrDefault(field => field.Name == name);
    }

    public MethodDeclaration? GetMethod(string name, List<TypeReference> argumentTypes)
    {
        return Methods.FirstOrDefault(
            method => method.Name == name && argumentTypes.EqualsByValue(method.ArgumentTypes));
    }
    
    public MethodDeclaration? GetConstructor(List<TypeReference> argumentTypes)
    {
        return Constructors.FirstOrDefault(
            ctor => argumentTypes.EqualsByValue(ctor.ArgumentTypes));
    }
    
    
    public bool HasField(string name)
    {
        return GetField(name) != null;
    }

    public bool HasMethod(string name, List<TypeReference> argumentTypes)
    {
        return GetMethod(name, argumentTypes) != null;
    }

    public bool HasConstructor(List<TypeReference> argumentTypes)
    {
        return GetConstructor(argumentTypes) != null;
    }

    
    public bool Equals(ClassInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }
}