using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal class MemberConstructor : CallableMember
{
    public MemberConstructor(string name = "") : base(name)
    {
    }

    public MemberConstructor(IEnumerable<ParameterDeclarationExpression> parameters, string name = "") : this(name)
    {
        AddParameters(parameters);
    }

    public override string ToString()
    {
        return new StringBuilder(Owner?.Name ?? Name)
            .Append($"::{Name}")
            .Append('(')
            .Append(string.Join(", ", Parameters))
            .Append(')')
            .ToString();
    }
}