namespace OCompiler.Analyze.Lexical.Literals
{
    sealed class Delimiter : ReservedLiteral
    {
        public static Delimiter Dot    { get; } = new(".");
        public static Delimiter Comma  { get; } = new(",");
        public static Delimiter Colon  { get; } = new(":");
        public static Delimiter Assign { get; } = new(":=");

        public static Delimiter LeftParenthesis { get; } = new("(");
        public static Delimiter RightParenthesis { get; } = new(")");
        public static Delimiter LeftSquareBracket { get; } = new("[");
        public static Delimiter RightSquareBracket { get; } = new("]");

        public static Delimiter StringQuote { get; } = new("\"");

        private Delimiter(string literal) : base(literal) { }
    }
}
