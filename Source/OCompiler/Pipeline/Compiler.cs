using System;
using System.Collections.Generic;
using OCompiler.Analyze.Lexical;
using System.Linq;
using OCompiler.Analyze.Lexical.Tokens;
using OCompiler.Analyze.Syntax;
using OCompiler.Analyze.Syntax.Declaration.Class;

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
            IEnumerator<Token> tokens = tokenizer.GetTokens().GetEnumerator();

            Boolean next = tokens.MoveNext();
            
            if (!Tree.TryParse(tokens, out List<Class>? tree))
            {
                throw new Exception("No classes.");
            }
            
            Console.Write(tree);
        }
    }
}
