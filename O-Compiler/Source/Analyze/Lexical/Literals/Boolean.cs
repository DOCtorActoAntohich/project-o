namespace OCompiler.Analyze.Lexical.Literals
{
    sealed class Boolean : ReservedLiteral
    {
        public static Boolean True  { get; } = new("true");
        public static Boolean False { get; } = new("false");

        private Boolean(string literal) : base(literal) { }
    }
}
