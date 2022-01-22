namespace OCompiler.Tokens
{
    sealed class ReservedWord : CodeEntity
    {

        public static ReservedWord If      { get; } = new("if");
        public static ReservedWord Is      { get; } = new("is");
        public static ReservedWord End     { get; } = new("end");
        public static ReservedWord Var     { get; } = new("var");
        public static ReservedWord This    { get; } = new("this");
        public static ReservedWord Loop    { get; } = new("loop");
        public static ReservedWord Then    { get; } = new("then");
        public static ReservedWord Else    { get; } = new("else");
        public static ReservedWord Class   { get; } = new("class");
        public static ReservedWord While   { get; } = new("while");
        public static ReservedWord Method  { get; } = new("method");
        public static ReservedWord Return  { get; } = new("return");
        public static ReservedWord Extends { get; } = new("extends");

        private ReservedWord(string literal) : base(literal) { }
    }
}
