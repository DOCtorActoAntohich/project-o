using System;
using System.Linq;
using OCompiler.Analyze.Lexical;
using OCompiler.Analyze.Semantics;
using OCompiler.Analyze.Syntax;
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
            
            Console.Write(tokenTree.ToString());

            var _ = new TreeValidator(tokenTree);
        }
    }
}
