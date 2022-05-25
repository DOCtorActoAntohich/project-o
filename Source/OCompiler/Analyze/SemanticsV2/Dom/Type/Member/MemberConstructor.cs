using System;
using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal class MemberConstructor : CallableMember
{
    public MemberConstructor(string name) : base(name)
    {
    }

    public MemberConstructor(string name, IEnumerable<ParameterDeclarationExpression> parameters) : this(name)
    {
        Parameters.AddRange(parameters);
    }

    public string ToString(string prefix = "", string nestedPrefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append(Owner?.Name ?? Name)
            .Append($"::{Name}")
            .Append('(')
            .Append(string.Join(", ", Parameters))
            .Append(')')
            .Append('\n');

        stringBuilder.Append(Statements.ToString(nestedPrefix));

        return stringBuilder.ToString();
    }
}