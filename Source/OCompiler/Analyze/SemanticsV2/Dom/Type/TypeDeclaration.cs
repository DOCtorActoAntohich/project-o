using System.Collections.Generic;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

namespace OCompiler.Analyze.SemanticsV2.Dom.Type;

internal class TypeDeclaration : TypeMember, ICanHaveGenericTypes
{
    public List<TypeReference> GenericTypes { get; } = new();
    public bool HasGenerics => GenericTypes.Count > 0;

    public List<TypeReference> BaseTypes { get; } = new();


    public List<MemberField> Fields { get; } = new();
    public List<MemberMethod> Methods { get; } = new();
    public List<MemberConstructor> Constructors { get; } = new();

    // For nested definitions.
    public List<TypeMember> OtherMembers { get; } = new();
    
    
    public TypeDeclaration(string name) : base(name)
    {
    }
}