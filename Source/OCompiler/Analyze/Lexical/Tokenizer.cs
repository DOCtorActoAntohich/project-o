using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using OCompiler.Extensions;

namespace OCompiler.Analyze.Lexical
{
    class Tokenizer
    {
        public string SourcePath { get; }
        private long _position;
        private long _currentLine;

        public Tokenizer(string sourcePath)
        {
            SourcePath = sourcePath;
        }

        public IEnumerable<Tokens.Token> GetTokens()
        {
            _position = 0;
            _currentLine = 1;
            Tokens.Token token;
            using var file = new StreamReader(SourcePath, Encoding.UTF8);
            do
            {
                token = GetNextToken(file);
                yield return token;
            }
            while (token is not Tokens.EndOfFile);
        }

        private Tokens.Token GetTokenCandidate(StreamReader stream)
        {
            if (stream.EndOfStream)
            {
                return new Tokens.EndOfFile(_position);
            }

            // Read until we find a valid token or whitespace
            var currentTerm = ReadWhile(stream, term => (
                !Tokens.Token.TryParse(_position, term, out var _) &&
                (term.Length == 0 || !char.IsWhiteSpace(term[^1]))
            ), strict: false);

            // Proceed reading while it still represents a valid token
            currentTerm += ReadWhile(stream, suffix => Tokens.Token.TryParse(_position, currentTerm + suffix, out var _), strict: false);

            bool parseSuccess = Tokens.Token.TryParse(_position, currentTerm, out var token);
            if (!parseSuccess)
            {
                throw new Exception($"Unable to parse token '{currentTerm}' at line {_currentLine}");
            }
            _position += currentTerm.Length;
            _currentLine += currentTerm.Count('\n');
            return token;
        }

        private Tokens.Token GetNextToken(StreamReader stream)
        {
            var token = GetTokenCandidate(stream);

            while (token is Tokens.CommentDelimiter startDelimiter)
            {
                // Read and skip all the comments
                var commentEnd = startDelimiter switch
                {
                    Tokens.CommentDelimiters.LineStart => "\n",
                    Tokens.CommentDelimiters.BlockStart => Tokens.CommentDelimiters.BlockEnd.Literal,
                    Tokens.CommentDelimiters.BlockEnd => throw new Exception($"Unexpected end of comment at line {_currentLine}"),
                    _ => throw new Exception($"Unknown comment delimiter at line {_currentLine}")
                };
                try
                {
                    var comment = ReadUntilSuffix(stream, commentEnd);
                    _position += comment.Length;
                    _currentLine += comment.Count('\n');
                }
                catch (EndOfStreamException)
                {
                    if (startDelimiter is Tokens.CommentDelimiters.BlockStart)
                    {
                        throw new Exception($"Unterminated block comment started at line {_currentLine}, reached end of file");
                    }
                }

                token = GetTokenCandidate(stream);
            }

            if (token is Tokens.Delimiters.StringQuote)
            {
                // Read and return the whole string
                var stringBoundary = Tokens.Delimiters.StringQuote.Literal;
                var stringEscape = Tokens.Delimiters.StringQuoteEscape.Literal;
                var stringContent = "";
                try
                {
                    // Read in a loop since a quote might be not an actual end of string
                    do
                    {
                        stringContent += ReadUntilSuffix(stream, stringBoundary);
                    }
                    while (stringContent.EndsWith(stringEscape));
                }
                catch (EndOfStreamException)
                {
                    throw new Exception($"Unterminated string started at line {_currentLine}, reached end of file");
                }

                token = new Tokens.StringLiteral(
                    _position,
                    stringContent.RemoveSuffix(stringBoundary).Replace(stringEscape, stringBoundary)
                );

                _position += stringContent.Length;
                _currentLine += stringContent.Count('\n');
            }

            return token;
        }

        private static string ReadWhile(StreamReader stream, Func<string, bool> term_condition, bool strict = true)
        {
            string term = "";
            while (AssertNotEnd(stream, strict) && term_condition(term + (char)stream.Peek()))
            {
                term += (char)stream.Read();
            }
            return term;
        }

        private static bool AssertNotEnd(StreamReader stream, bool strict)
        {
            if (stream.EndOfStream && strict)
            {
                throw new EndOfStreamException();
            }
            return !stream.EndOfStream;
        }

        private static string ReadUntilSuffix(StreamReader stream, string suffix)
        {
            var result = ReadWhile(stream, term => (
                !term.EndsWith(suffix)
            ));
            // Extract the last suffix char from the stream
            if (!stream.EndOfStream)
            {
                result += (char)stream.Read();
            }
            return result;
        }
    }
}
