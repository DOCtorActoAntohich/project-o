using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace OCompiler.Tokens
{
    class Tokenizer
    {
        public string SourcePath { get; }
        private int _charsRead;

        public Tokenizer(string sourcePath)
        {
            SourcePath = sourcePath;
        }

        public IEnumerable<Token> GetTokens()
        {
            _charsRead = 0;
            using (var file = new StreamReader(SourcePath, Encoding.UTF8))
            {
                while (!file.EndOfStream)
                {
                    foreach (var token in ReadNextToken(file)) {
                        yield return token;
                    };
                }
            }
            yield return Token.EndOfFile(_charsRead);
        }

        private IEnumerable<Token> ReadNextToken(StreamReader stream)
        {
            string literal;
            int tokenStart = _charsRead;
            char symbol = (char)stream.Peek();
            if (char.IsWhiteSpace(symbol))
            {
                literal = ReadWhile(stream, char.IsWhiteSpace);
                yield return new Token(tokenStart, _charsRead, literal, TokenType.Whitespace);
            }
            else if (IsValidIdentifier(symbol, isFirstChar: true))
            {
                literal = ReadWhile(stream, c => IsValidIdentifier(c, isFirstChar: false));
                var entity = CodeEntity.GetByLiteral(literal);
                var tokenType = entity switch
                {
                    BooleanLiteral _ => TokenType.BooleanLiteral,
                    Delimiter _      => TokenType.Delimiter,
                    Keyword _        => TokenType.ReservedWord,
                    CodeEntity e when e == CodeEntity.Empty => TokenType.Identifier,
                    _ => TokenType.Unknown,
                };
                yield return new Token(tokenStart, _charsRead, literal, tokenType, entity);
            }
            else if (char.IsDigit(symbol))
            {
                literal = ReadWhile(stream, char.IsDigit);

                // ToList here prevents from further file reading
                var tokens = (IEnumerable<Token>)(
                    ReadNextToken(stream)
                        .Concat(ReadNextToken(stream))
                        .Prepend(new Token(tokenStart, _charsRead, literal, TokenType.IntegerLiteral))
                ).ToList();

                var remaining = tokens.Skip(3);
                tokens = tokens.Take(3);

                if (TokensRepresentReal(tokens))
                {
                    // Replace the three tokens (Integer, Dot, Integer) with a Real
                    literal = tokens.Aggregate("", (buffer, token) => buffer + token.Literal);
                    yield return new Token(tokenStart, _charsRead, literal, TokenType.RealLiteral);
                }
                else
                {
                    foreach (var token in tokens)
                    {
                        yield return token;
                    }
                }
                foreach (var token in remaining)
                {
                    yield return token;
                }
            }
            else
            {
                literal = ReadWhile(stream, c => !(char.IsWhiteSpace(c) || IsValidIdentifier(c, false)));
                if (literal == Delimiter.Assign.Value)
                {
                    yield return new Token(tokenStart, _charsRead, literal, TokenType.Delimiter, Delimiter.Assign);
                }
                else
                {
                    // There might be multiple tokens at once (e.g. multiple brackets)
                    int len = 0;
                    do
                    {
                        // Take more and more symbols until a valid delimiter is found
                        var entityLiteral = literal[..++len];
                        var entity = CodeEntity.GetByLiteral(entityLiteral);
                        if (entity != CodeEntity.Empty)
                        {
                            yield return new Token(tokenStart, tokenStart + len, entityLiteral, TokenType.Delimiter, entity);
                            tokenStart += len;
                            // Trim the beginning and go to the start
                            literal = literal[len..];
                            len = 0;
                        }
                    }
                    while (len < literal.Length);

                    if (literal.Length > 0)
                    {
                        // If there's something left unparsed, throw the error
                        throw new Exception($"Parse error at offset {tokenStart}, the literal is {literal}");
                    }
                }
            }
        }

        private string ReadWhile(StreamReader stream, Func<char, bool> condition)
        {
            string word = "";
            while (condition((char)stream.Peek()))
            {
                word += (char)stream.Read();
                ++_charsRead;
            }
            return word;
        }

        private static bool IsValidIdentifier(char c, bool isFirstChar = false)
        {
            var isUnderScore = (c == '_');
            if (isFirstChar)
            {
                return isUnderScore || char.IsLetter(c);
            }
            return isUnderScore || char.IsLetterOrDigit(c);
        }

        private static bool TokensRepresentReal(IEnumerable<Token> tokens)
        {
            TokenType[] realNumberTokenTypes = { TokenType.IntegerLiteral, TokenType.Delimiter, TokenType.IntegerLiteral };
            return (
                tokens.Select(token => token.Type).SequenceEqual(realNumberTokenTypes) &&
                tokens.ElementAt(1).Entity == Delimiter.Dot
            );
        }
    }
}
