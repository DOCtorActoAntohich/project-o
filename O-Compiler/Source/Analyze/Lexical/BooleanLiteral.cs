namespace OCompiler.Analyze.Lexical
{
    sealed class BooleanLiteral : CodeEntity
    {
        public static BooleanLiteral True  { get; } = new("true");
        public static BooleanLiteral False { get; } = new("false");

        private BooleanLiteral(string literal) : base(literal) { }
    }
}
