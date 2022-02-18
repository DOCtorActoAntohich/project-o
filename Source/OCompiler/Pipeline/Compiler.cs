using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical;
using OCompiler.Analyze.Lexical.Tokens;
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
            //var tokens = tokenizer.GetTokens().ToList();
            //Formatter.ShowHighlightedCode(tokens);
            //Formatter.ShowTokens(tokens);
            IEnumerable<Token> tokens = tokenizer.GetTokens();

            var tokenTree = new Tree(new TokenEnumerator(tokens));
            if (tokenTree.IsEmpty)
            {
                throw new Exception("No classes.");
            }
            
            Console.Write(tokenTree.ToString());
        }
    }
}
