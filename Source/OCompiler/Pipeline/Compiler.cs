using OCompiler.Analyze.Lexical;

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
            var tokens = tokenizer.GetTokens();
            Formatter.ShowHighlightedCode(tokens);
            Formatter.ShowTokens(tokens);
        }
    }
}
