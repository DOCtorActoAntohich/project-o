﻿using System;
using System.Linq;
using OCompiler.Analyze.Lexical;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Syntax;
using OCompiler.Generate;
using OCompiler.Utils;

namespace OCompiler.Pipeline
{
    internal class Compiler
    {
        public string SourceFilePath { get; }
        public string EntryClass { get; }
        public string[] EntrypointArgs { get; }

        public Compiler(string sourceFilePath, string entryClass, string[] entrypointArgs)
        {
            SourceFilePath = sourceFilePath;
            EntryClass = entryClass;
            EntrypointArgs = entrypointArgs;
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

            var generator = new Emitter(validator.ValidatedClasses);
            generator.Run();

            var entrypoint = Invoker.GetEntryPoint(validator.ValidatedClasses, EntryClass, EntrypointArgs);
            Console.WriteLine($"Entry point class: {entrypoint.Context.Class.Name}");
        }
    }
}
