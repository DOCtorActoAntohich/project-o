namespace OCompiler.Analyze.Lexical
{
    internal class TokenPosition
    {
        public long Line { get; set; }
        public long Column { get; set; }

        public TokenPosition(long line, long column)
        {
            Line = line;
            Column = column;
        }

        public TokenPosition() { }
    }
}
