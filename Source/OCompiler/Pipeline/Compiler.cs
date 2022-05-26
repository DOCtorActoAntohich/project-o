﻿using System;
using System.Linq;
using System.Reflection;

using OCompiler.Analyze.Lexical;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.SemanticsV2;
using OCompiler.Analyze.Syntax;
using OCompiler.Exceptions;
using OCompiler.Generate;
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

        public Assembly Run()
        {
            var tokenizer = new Tokenizer(SourceFilePath);
            var tokens = tokenizer.GetTokens();

            var tokenTree = new Tree(new TokenEnumerator(tokens));
            if (tokenTree.IsEmpty)
            {
                throw new AnalyzeError("No classes.");
            }

            var annotatedSyntaxTree = new AnnotatedSyntaxTreeV2(tokenTree);
            Console.WriteLine(annotatedSyntaxTree);
            //var validator = new AnnotatedSyntaxTree(tokenTree);
            
            //var generator = new Emitter(validator.ValidatedClasses);
            var generator = new EmitterV2(annotatedSyntaxTree);

            return generator.Assembly;
        }

        public Assembly RunVerbose()
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

            var validator = new AnnotatedSyntaxTree(tokenTree);

            var generator = new Emitter(validator.ValidatedClasses);
            return generator.Assembly;
        }
    }
}
