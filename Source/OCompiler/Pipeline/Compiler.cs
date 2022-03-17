using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using OCompiler.Analyze.Lexical;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Lexical.Tokens.BooleanLiterals;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Syntax;
using OCompiler.Exceptions;
using OCompiler.Generate;
using OCompiler.CodeGeneration;
using OCompiler.CodeGeneration.Translation.CSharp;
using OCompiler.Utils;

namespace OCompiler.Pipeline
{
    internal class Compiler
    {
        public string SourceFilePath { get; }
        public string PathToExecutable { get; }
        public string MainClass { get; }

        public Compiler(string sourceFilePath, string exePath, string mainClass)
        {
            SourceFilePath = sourceFilePath;
            PathToExecutable = exePath;
            MainClass = mainClass;
        }

        public void Run()
        {
            var tokenizer = new Tokenizer(SourceFilePath);
            var tokens = tokenizer.GetTokens().ToList();
            Formatter.ShowHighlightedCode(tokens);
            Console.WriteLine();

            var tokenTree = new Tree(new TokenEnumerator(tokens));
            if (tokenTree.IsEmpty)
            {
                throw new AnalyzeError("No classes.");
            }

            Formatter.ShowAST(tokenTree);
            Console.WriteLine();

            var validator = new TreeValidator(tokenTree);

            var compileUnit = new CompileUnit(validator, MainClass);
            var code = new Code(compileUnit);
            
            
            using (var file = new StreamWriter(PathToExecutable.Replace("exe", "cs")))
            {
                file.Write(code.Text);
                file.Flush();
            }

            var codeGenerator = new CodeGenerator(compileUnit);
            codeGenerator.Build(PathToExecutable);

            //var generator = new Emitter(validator.ValidatedClasses);
            //return generator.Assembly;
        }
    }
}
