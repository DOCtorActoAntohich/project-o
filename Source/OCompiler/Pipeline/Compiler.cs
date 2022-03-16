﻿using System;
using System.Linq;
using OCompiler.Analyze.Lexical;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Syntax;
using OCompiler.CodeGeneration;
using OCompiler.CodeGeneration.Translation.CSharp;
using OCompiler.Utils;

namespace OCompiler.Pipeline
{
    internal class Compiler
    {
        public string SourceFilePath { get; }
        public Compiler(string sourceFilePath)
        {
            SourceFilePath = sourceFilePath;
        }

        public void Run()
        {
            var tokenizer = new Tokenizer(SourceFilePath);
            var tokens = tokenizer.GetTokens().ToList();
            Formatter.ShowHighlightedCode(tokens);

            var tokenTree = new Tree(new TokenEnumerator(tokens));
            if (tokenTree.IsEmpty)
            {
                throw new Exception("No classes.");
            }
            
            Console.WriteLine(tokenTree.ToString());

            var validator = new TreeValidator(tokenTree);
            Console.WriteLine(validator.GetValidationInfo());
            
            Console.WriteLine("\n\n\n");

            var compileUnit = CompileUnit.FromAnnotatedSyntaxTree(validator);
            //var csharpCode = new Code(compileUnit);
            //var compiler = new CodeGenerator(csharpCode);
            //compiler.Build("mogus/bin/mogus.exe");
        }
    }
}
