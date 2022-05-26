using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

namespace OCompiler.Analyze.SemanticsV2.Dom.Expression.Call;

internal class ObjectCreateExpression : CallExpression
{
    public List<TypeReference> GenericTypes { get; } = new();

    public MemberConstructor Constructor { get; set; } = null!;

    public ObjectCreateExpression(TypeReference type) : base(type.Name)
    {
        GenericTypes.AddRange(type.GenericTypes);
    }
    
    public ObjectCreateExpression(TypeReference type, IEnumerable<Expression> arguments) : base(type.Name)
    {
        GenericTypes.AddRange(type.GenericTypes);
        AddArguments(arguments);
    }
}