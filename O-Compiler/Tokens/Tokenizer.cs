using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace OCompiler.Tokens
{
    class Tokenizer
    {
        public readonly string SourcePath;
        private int charsRead = 0;

        public Tokenizer(string sourcePath)
        {
            SourcePath = sourcePath;
        }

        public List<Token> GetTokens()
        {
            var tokens = new List<Token>();
            using (var file = new StreamReader(SourcePath, Encoding.UTF8))
            {
                while (!file.EndOfStream)
                {
                    string literal;
                    Token token;
                    int tokenStart = charsRead;
                    char symbol = (char)file.Peek();
                    if (char.IsWhiteSpace(symbol))
                    {
                        literal = ReadWhile(file, char.IsWhiteSpace);
                        token = new Token(tokenStart, charsRead, literal, TokenType.Whitespace);
                        tokens.Add(token);
                    }
                    else if (IsValidIdentifier(symbol, isFirstChar: true))
                    {
                        literal = ReadWhile(file, c => IsValidIdentifier(c, isFirstChar: false));
                        var entity = CodeEntity.GetByLiteral(literal);
                        TokenType tokenType;
                        switch (entity)
                        {
                            case BooleanLiteral _:
                                tokenType = TokenType.BooleanLiteral;
                                break;
                            case Delimiter _:
                                tokenType = TokenType.Delimiter;
                                break;
                            case ReservedWord _:
                                tokenType = TokenType.ReservedWord;
                                break;
                            case CodeEntity e when e == CodeEntity.Empty:
                                tokenType = TokenType.Identifier;
                                break;
                            default:
                                tokenType = TokenType.Unknown;
                                break;
                        }
                        tokens.Add(new Token(tokenStart, charsRead, literal, tokenType, entity));
                    }
                    else if (char.IsDigit(symbol))
                    {
                        literal = ReadWhile(file, char.IsDigit);
                        tokens.Add(new Token(tokenStart, charsRead, literal, TokenType.IntegerLiteral));

                        var lastThreeTokens = tokens.TakeLast(3);
                        if (TokensRepresentReal(lastThreeTokens))
                        {
                            // Replace the last three tokens (Integer, Dot, Integer) with a Real
                            tokenStart = lastThreeTokens.First().StartOffset;
                            var realNumber = lastThreeTokens.Aggregate("", (s, token) => s + token.Literal);
                            var realToken = new Token(tokenStart, charsRead, realNumber, TokenType.IntegerLiteral);
                            tokens = tokens.SkipLast(3).Append(realToken).ToList();
                        }
                    }
                    else
                    {
                        literal = ReadWhile(file, c => !(char.IsWhiteSpace(c) || IsValidIdentifier(c, false)));
                        if (literal == Delimiter.Assign.Value)
                        {
                            tokens.Add(new Token(tokenStart, charsRead, literal, TokenType.Delimiter, Delimiter.Assign));
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
                                    tokens.Add(new Token(tokenStart, tokenStart + len, entityLiteral, TokenType.Delimiter, entity));
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
            }
            return tokens;
        }

        private string ReadWhile(StreamReader stream, Func<char, bool> condition)
        {
            string word = "";
            while (condition((char)stream.Peek()))
            {
                word += (char)stream.Read();
                charsRead++;
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
