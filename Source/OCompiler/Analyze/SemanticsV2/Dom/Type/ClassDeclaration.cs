using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type;

internal class ClassDeclaration : TypeMember, ICanHaveGenericTypes
{
    public List<TypeReference> GenericTypes { get; } = new();
    public bool HasGenerics => GenericTypes.Count > 0;

    public TypeReference? BaseType { get; set; }


    public List<MemberField> Fields { get; } = new();
    public List<MemberMethod> Methods { get; } = new();
    public List<MemberConstructor> Constructors { get; } = new();


    public ClassDeclaration(string name) : base(name)
    {
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
        foreach (var genericType in GenericTypes)
        {
            if (genericType.Name == name)
            {
                return genericType;
            }
        }

        return null;
    }

    public bool HasGenericType(string name)
    {
        return GetGenericType(name) != null;
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
}