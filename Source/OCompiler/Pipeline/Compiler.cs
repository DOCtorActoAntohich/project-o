using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax;
using OCompiler.Analyze.Syntax.Declaration.Class;
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
            
            if (!Tree.TryParse(new TokenEnumerator(tokens), out List<Class>? tree))
            {
                throw new Exception("No classes.");
            }
            
            Console.Write(Tree.ToString(tree));
        }
    }
}
