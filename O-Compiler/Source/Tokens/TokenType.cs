namespace OCompiler.Tokens
{
    enum TokenType
    {
        Whitespace, Identifier, ReservedWord, Delimiter,
        IntegerLiteral, RealLiteral, BooleanLiteral,
        EndOfFile, Unknown
    }
}
