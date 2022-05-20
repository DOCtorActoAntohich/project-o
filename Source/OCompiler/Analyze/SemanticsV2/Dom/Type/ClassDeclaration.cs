using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type;

internal class ClassDeclaration : TypeMember, ICanHaveGenericTypes
{
    public List<TypeReference> GenericTypes { get; } = new();
    public bool HasGenerics => GenericTypes.Count > 0;

    public List<TypeReference> BaseTypes { get; } = new();


    public List<MemberField> Fields { get; } = new();
    public List<MemberMethod> Methods { get; } = new();
    public List<MemberConstructor> Constructors { get; } = new();

    // For nested definitions.
    public List<TypeMember> OtherMembers { get; } = new();
    
    
    public ClassDeclaration(string name) : base(name)
    {
    }
}