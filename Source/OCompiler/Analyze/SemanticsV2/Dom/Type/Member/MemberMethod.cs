using System.Collections.Generic;
using System.Text;
using OCompiler.Analyze.SemanticsV2.Dom.Expression.Special;
using OCompiler.Analyze.SemanticsV2.Dom.Statement;
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

    public string ToString(string prefix = "", string nestedPrefix = "")
    {
        var stringBuilder = new StringBuilder(prefix)
            .Append(Name)
            .Append('(')
            .Append(string.Join(", ", Parameters))
            .Append(')');

        if (ReturnType != null)
        {
            stringBuilder.Append($": {ReturnType}");
        }

        stringBuilder.Append('\n');
        
        stringBuilder.Append(ICanHaveStatements.StatementsString(Statements, nestedPrefix));

        return stringBuilder.ToString();
    }
}