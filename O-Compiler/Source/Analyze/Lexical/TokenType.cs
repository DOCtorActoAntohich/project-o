namespace OCompiler.Analyze.Lexical
{
    enum TokenType
    {
        Whitespace, Identifier, ReservedWord, Delimiter,
        IntegerLiteral, RealLiteral, BooleanLiteral,
        EndOfFile, Unknown
    }
}
