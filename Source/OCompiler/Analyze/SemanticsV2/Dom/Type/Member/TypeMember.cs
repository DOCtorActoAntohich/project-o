namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal abstract class TypeMember : CodeObject
{
    public ClassDeclaration? Owner { get; set; }
    
    public System.Reflection.MemberInfo? DotnetType;
    
    protected TypeMember(string name) : base(name)
    {
    }
}