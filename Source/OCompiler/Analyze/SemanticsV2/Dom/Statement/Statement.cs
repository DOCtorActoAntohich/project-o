namespace OCompiler.Analyze.SemanticsV2.Dom.Statement;

internal abstract class Statement : CodeObject
{
    public ICanHaveStatements? Holder { get; set; }
    
    public Statement() : base("")
    {
    }
}