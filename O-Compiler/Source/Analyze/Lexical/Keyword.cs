namespace OCompiler.Analyze.Lexical
{
    sealed class Keyword : CodeEntity
    {

        public static Keyword If      { get; } = new("if");
        public static Keyword Is      { get; } = new("is");
        public static Keyword End     { get; } = new("end");
        public static Keyword Var     { get; } = new("var");
        public static Keyword This    { get; } = new("this");
        public static Keyword Loop    { get; } = new("loop");
        public static Keyword Then    { get; } = new("then");
        public static Keyword Else    { get; } = new("else");
        public static Keyword Class   { get; } = new("class");
        public static Keyword While   { get; } = new("while");
        public static Keyword Method  { get; } = new("method");
        public static Keyword Return  { get; } = new("return");
        public static Keyword Extends { get; } = new("extends");

        private Keyword(string literal) : base(literal) { }
    }
}
