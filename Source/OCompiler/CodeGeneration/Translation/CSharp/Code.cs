using OCompiler.Analyze.Semantics;

namespace OCompiler.CodeGeneration.Translation.CSharp;

internal class Code
{
    public string Text { get; private set; }
    
    public Code(TreeValidator ast)
    {
        Text = "";
    }
}