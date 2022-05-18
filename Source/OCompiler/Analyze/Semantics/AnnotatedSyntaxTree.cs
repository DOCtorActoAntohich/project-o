using System.Collections.Generic;
using OCompiler.Analyze.Semantics.Class;
using OCompiler.Utils;

namespace OCompiler.Analyze.Semantics;

internal class AnnotatedSyntaxTree
{
    public List<ClassInfo> Classes { get; } = new();
    
    
    public AnnotatedSyntaxTree(Syntax.Tree syntaxTree)
    {
        foreach (var @class in syntaxTree.GetEnumerator().MakeEnumerable())
        {
            
        }
    }
}