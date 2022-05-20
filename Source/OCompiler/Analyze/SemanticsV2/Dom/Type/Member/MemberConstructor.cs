using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal class MemberConstructor : TypeMember, ICanHaveParameters, ICanHaveStatements
{
    public List<ParameterDeclarationExpression> Parameters { get; } = new();
    
    public List<DomStatement> Statements { get; } = new();
    
    
    public MemberConstructor() : base("")
    {
    }
}