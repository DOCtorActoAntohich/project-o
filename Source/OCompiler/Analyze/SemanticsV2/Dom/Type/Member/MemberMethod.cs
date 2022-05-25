using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal class MemberMethod : CallableMember
{
    private TypeReference _returnType = null!;
    public TypeReference ReturnType
    {
        get => _returnType;
        set
        {
            _returnType = value;
            IsReturnTypeSpecified = true;
        }
    }
    public bool IsReturnTypeSpecified { get; set; }

    public List<TypeReference> GenericTypes { get; } = new();


    public MemberMethod(string name) : base(name)
    {
        IsReturnTypeSpecified = false;
    }
    
    public MemberMethod(string name, TypeReference returnType) : this(name)
    {
        ReturnType = returnType;
    }

    public MemberMethod(string name, IEnumerable<ParameterDeclarationExpression> parameters) : this(name)
    {
        Parameters.AddRange(parameters);
    }

    public MemberMethod(string name, IEnumerable<ParameterDeclarationExpression> parameters, TypeReference returnType)
        : this(name)
    {
        Parameters.AddRange(parameters);
        ReturnType = returnType;
    }

    public string ToString(string prefix = "", string nestedPrefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append(Owner != null ? $"{Owner.Name}::" : "")
            .Append(Name)
            .Append('(')
            .Append(Parameters)
            .Append(')');

        if (IsReturnTypeSpecified)
        {
            stringBuilder.Append($": {ReturnType}");
        }

        if (Statements.Count > 0)
        {
            stringBuilder.Append('\n')
                .Append(Statements.ToString(nestedPrefix));
        }

        return stringBuilder.ToString();
    }
}