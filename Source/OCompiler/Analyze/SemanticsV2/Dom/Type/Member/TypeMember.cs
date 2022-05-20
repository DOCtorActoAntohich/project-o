namespace OCompiler.Analyze.SemanticsV2.Dom.Type.Member;

internal abstract class TypeMember : CodeObject
{
    public ClassDeclaration? Owner { get; set; }
    
    protected TypeMember(string name) : base(name)
    {
    }
}