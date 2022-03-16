using System;
using System.Linq;
using System.Reflection;

using OCompiler.Analyze.Lexical;
using OCompiler.Analyze.Semantics;
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
            var tokens = tokenizer.GetTokens().ToList();
            Formatter.ShowHighlightedCode(tokens);

            var tokenTree = new Tree(new TokenEnumerator(tokens));
            if (tokenTree.IsEmpty)
            {
                throw new AnalyzeError("No classes.");
            }
            
            Console.WriteLine(tokenTree.ToString());

            var validator = new TreeValidator(tokenTree);
            Console.WriteLine(validator.GetValidationInfo());

            var generator = new Emitter(validator.ValidatedClasses);
            return generator.Assembly;
        }
    }
}
