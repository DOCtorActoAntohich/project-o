using System.Collections.Generic;

using OCompiler.Tokens;

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
            // This is the only reference to ReservedWord for now
            // Required to prevent skipping the constructor of the class
            Keyword.Loop.ToString();

            var tokenizer = new Tokenizer(SourceFilePath);
            var tokens = tokenizer.GetTokens();
            Formatter.ShowHighlightedCode(tokens);
            Formatter.ShowTokens(tokens);
        }
    }
}
