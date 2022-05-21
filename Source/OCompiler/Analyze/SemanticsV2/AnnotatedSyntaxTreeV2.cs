using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using OCompiler.Analyze.SemanticsV2.Dom.Expression;
using OCompiler.Analyze.SemanticsV2.Dom.Type;
using OCompiler.Analyze.SemanticsV2.Dom.Type.Member;
using OCompiler.Analyze.SemanticsV2.Tree;

namespace OCompiler.Analyze.SemanticsV2;

internal class AnnotatedSyntaxTreeV2
{
    public Dictionary<string, ClassDeclaration> BuiltinClasses { get; }
    public Dictionary<string, ClassDeclaration> ParsedClasses { get; } = new();


    public AnnotatedSyntaxTreeV2(Syntax.Tree syntaxTree)
    {
        BuiltinClasses = new BuiltinClassTree().Classes;
    }
}