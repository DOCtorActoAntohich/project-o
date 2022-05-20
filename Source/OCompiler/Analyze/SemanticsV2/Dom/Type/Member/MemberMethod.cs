using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal class MemberMethod : TypeMember, ICanHaveParameters, ICanHaveGenericTypes, ICanHaveStatements
{
    public List<ParameterDeclarationExpression> Parameters { get; } = new();
    public TypeReference ReturnType { get; set; }

    public List<TypeReference> GenericTypes { get; } = new();

    public List<DomStatement> Statements { get; } = new();
    
    
    public MemberMethod(
        string name, 
        IEnumerable<ParameterDeclarationExpression> parameters, 
        TypeReference returnType
        ) : base(name)
    {
        Parameters.AddRange(parameters);
        ReturnType = returnType;
    }
}