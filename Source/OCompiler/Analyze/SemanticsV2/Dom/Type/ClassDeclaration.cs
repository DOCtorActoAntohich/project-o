using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Exceptions;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type;

internal class ClassDeclaration : TypeMember
{
    public List<TypeReference> GenericTypes { get; private set; } = new();
    public bool HasGenerics => GenericTypes.Count > 0;

    public TypeReference? BaseType { get; set; }
    
    public List<MemberField> Fields { get; private set; } = new();
    public List<MemberMethod> Methods { get; private set; } = new();
    public List<MemberConstructor> Constructors { get; private set; } = new();

    public AnnotatedSyntaxTreeV2 Ast { get; set; } = null!;

    public ClassDeclaration(string name) : base(name)
    {
        Owner = this;
    }

    public void AddField(MemberField field)
    {
        Fields.Add(field);
        field.Owner = this;
    }

    public void AddMethod(MemberMethod method)
    {
        Methods.Add(method);
        method.Owner = this;
    }

    public void AddConstructor(MemberConstructor constructor)
    {
        Constructors.Add(constructor);
        constructor.Owner = this;
    }

    public TypeReference? GetGenericType(string name)
    {
        return GenericTypes.FirstOrDefault(genericType => genericType.Name == name);
    }

    public bool HasGenericType(string name)
    {
        return GetGenericType(name) != null;
    }

    public static bool AreParametersSame(List<TypeReference> left, List<TypeReference> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        
        for (var i = 0; i < left.Count; ++i)
        {
            if (left[i].DifferentFrom(right[i]))
            {
                return false;
            }
        }

        return true;
    }
    
    public MemberConstructor GetConstructor(List<TypeReference> parameters)
    {
        foreach (var constructor in Constructors)
        {
            var ctorParameterTypes = constructor.Parameters
                .Select(t => t.Type).ToList();
            if (AreParametersSame(ctorParameterTypes, parameters))
            {
                return constructor;
            }
        }

        if (BaseType != null)
        {
            var parentClass = Ast.GetClass(BaseType.Name);
            return parentClass.GetConstructor(parameters);
        }
        

        throw new AnalyzeError($"Couldn't find suitable constructor with {parameters.Count} parameters");
    }
    
    public MemberMethod GetMethod(string targetMethodName, List<TypeReference> parameters)
    {
        foreach (var method in Methods)
        {
            if (method.Name != targetMethodName)
            {
                continue;
            }
            
            var methodParameterTypes = method.Parameters
                .Select(t => t.Type).ToList();
            if (AreParametersSame(methodParameterTypes, parameters))
            {
                return method;
            }
        }
        
        if (BaseType != null)
        {
            var parentClass = Ast.GetClass(BaseType.Name);
            return parentClass.GetMethod(targetMethodName, parameters);
        }

        throw new AnalyzeError($"Couldn't find method: {targetMethodName}({parameters.Count})");
    }

    public MemberField GetField(string targetFieldName)
    {
        foreach (var field in Fields)
        {
            if (field.Name == targetFieldName)
            {
                return field;
            }
        }
        
        if (BaseType != null)
        {
            var parentClass = Ast.GetClass(BaseType.Name);
            return parentClass.GetField(targetFieldName);
        }

        throw new AnalyzeError($"Couldn't find field: {targetFieldName}");
    }
    
    public override string ToString()
    {
        var stringBuilder = new StringBuilder(Name);

        if (HasGenerics)
        {
            stringBuilder
                .Append('<')
                .Append(string.Join(", ", GenericTypes))
                .Append('>');
        }

        if (BaseType != null && BaseType.Name != "Object")
        {
            stringBuilder.Append($" ---> {BaseType}");
        }

        stringBuilder.Append('\n');

        
        for (var i = 0; i < Fields.Count; ++i)
        {
            var isLast = (i + 1 == Fields.Count) && (Constructors.Count > 0) && (Methods.Count > 0);
            var prefix = isLast ? "└── " : "├── ";
            stringBuilder.Append(Fields[i].ToString(prefix))
                .Append('\n');
        }

        for (var i = 0; i < Constructors.Count; ++i)
        {
            var isLast = (i + 1 == Constructors.Count) && (Methods.Count == 0);
            if (isLast)
            {
                stringBuilder.Append(
                    Constructors[i].ToString(prefix: "└── ", nestedPrefix: "    "));
                break;
            }

            stringBuilder.Append(
                Constructors[i].ToString(prefix: "├── ", nestedPrefix: "│   "));
            stringBuilder.Append('\n');
        }

        for (var i = 0; i < Methods.Count; ++i)
        {
            var isLast = (i + 1 == Methods.Count);
            if (isLast)
            {
                stringBuilder.Append(
                    Methods[i].ToString(prefix: "└── ", nestedPrefix: "    "));
                break;
            }

            stringBuilder.Append(
                Methods[i].ToString(prefix: "├── ", nestedPrefix: "│   "));
            stringBuilder.Append('\n');
        }

        return stringBuilder.ToString();
    }
    
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}