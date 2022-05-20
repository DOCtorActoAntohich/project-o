namespace OCompiler.Analyze.SemanticsV2.Dom;

internal abstract class CodeObject
{
    public string Name { get; set; }

    public CodeObject(string name)
    {
        Name = name;
    }
}