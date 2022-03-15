using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using OCompiler.Analyze.Semantics;

namespace OCompiler.CodeGeneration.Translation.CSharp;

internal class Code
{
    public string Text { get; }
    
    public Code(CodeCompileUnit compileUnit)
    {
        Text = GenerateCode(compileUnit);
    }

    
    private static string GenerateCode(CodeCompileUnit compileUnit)
    {
        var csProvider = new CSharpCodeProvider();

        var stringBuilder = new StringBuilder();
        using var writer = new StringWriter(stringBuilder);

        var options = new CodeGeneratorOptions();
        csProvider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
        
        // Let me put my rant here.
        // If only `csProvider.CompileAssemblyFromDom` worked...
        // No one would have to write a thing that compiles C#... in C#. (-_-)
        // But since we're using .NET 6 (which is undeniably cool),
        // Some old and clumsy features (like CodeDOM) are not supported yet.
        // Come on, even Win10 PowerShell cannot run .NET 6 apps by default,
        // And needs a workaround like `dotnet <path-to-exe>`.
        // Some NuGet packages designed to fix that problem only impose more of the other ones.
        // If/when Windows fully supports modern .NET, I'll just ask you to
        // Go for `CompileAssemblyFromDom`.
        // Now suffer :)

        writer.Flush();

        return stringBuilder.ToString();
    }
}