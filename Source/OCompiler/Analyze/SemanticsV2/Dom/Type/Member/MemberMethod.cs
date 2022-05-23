using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using DomStatement = OCompiler.Analyze.SemanticsV2.Dom.Statement.Statement;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal class MemberMethod : CallableMember, ICanHaveGenericTypes
{
    public TypeReference? ReturnType { get; set; }

    public List<TypeReference> GenericTypes { get; } = new();


    public MemberMethod(string name, TypeReference? returnType = null) : base(name)
    {
        ReturnType = returnType;
    }
    
    public MemberMethod(
        string name, 
        IEnumerable<ParameterDeclarationExpression> parameters, 
        TypeReference? returnType = null
        ) : this(name, returnType)
    {
        AddParameters(parameters);
    }
}