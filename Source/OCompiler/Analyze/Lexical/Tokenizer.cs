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
        private long _tokenBeginning;
        private long _newlines;

        public Tokenizer(string sourcePath)
        {
            SourcePath = sourcePath;
        }

        public IEnumerable<Tokens.Token> GetTokens()
        {
            _tokenBeginning = 0;
            _newlines = 0;
            using var file = new StreamReader(SourcePath, Encoding.UTF8);
            while (!file.EndOfStream)
            {
                yield return GetNextToken(file);
            }
            yield return new Tokens.EndOfFile(_tokenBeginning);
        }

        private string GetNextTerm(StreamReader stream)
        {
            // Read until we find a valid token or whitespace
            var currentTerm = ReadWhile(stream, term => (
                !Tokens.Token.TryParse(_tokenBeginning, term, out var _) &&
                (term.Length == 0 || !char.IsWhiteSpace(term[^1]))
            ));

            // Proceed reading while it still represents a valid token
            currentTerm += ReadWhile(stream, suffix => Tokens.Token.TryParse(_tokenBeginning, currentTerm + suffix, out var _));
            return currentTerm;
        }

        private Tokens.Token GetNextToken(StreamReader stream)
        {
            var possibleToken = GetNextTerm(stream);
            bool parseSuccess = Tokens.Token.TryParse(_tokenBeginning, possibleToken, out var token);

            _tokenBeginning += possibleToken.Length;
            _newlines += possibleToken.Count('\n');

            var stringBoundary = Literals.Delimiter.StringQuote.Value;
            var stringEscape = Literals.Delimiter.StringQuoteEscape.Value;
            if (!parseSuccess)
            {
                throw new Exception($"Unable to parse token '{possibleToken}' at line {_newlines + 1}");
            }
            if (token.Literal == stringBoundary)
            {
                var stringContent = "";
                // Read in a loop since a quote might be not an actual end of string
                do
                {
                    stringContent += ReadUntilSuffix(stream, stringBoundary);
                }
                while (stringContent.EndsWith(stringEscape));
                stringContent = stringContent.RemoveSuffix(stringBoundary);

                var stringNewLines = stringContent.Count('\n');
                token = new Tokens.StringLiteral(_tokenBeginning, stringContent.Replace(stringEscape, stringBoundary));

                // Note: _tokenBeginning will be increased by the amount of the quote escapes,
                //       even though they're not passed to the token itself
                _tokenBeginning += stringContent.Length + stringBoundary.Length;
                _newlines += stringNewLines;

                if (stream.EndOfStream)
                {
                    throw new Exception($"Unterminated string at line {_newlines + 1} (started at line {_newlines + 1 - stringNewLines})");
                }
                return token;
            }
            else if (token is not Tokens.CommentDelimiter)
            {
                return token;
            }

            while (parseSuccess && token is Tokens.CommentDelimiter delimiter)
            {
                if (delimiter.IsLineCommentStart)
                {
                    var comment = ReadWhile(stream, term => term[^1] != '\n');
                    _tokenBeginning += comment.Length;
                    _newlines += comment.Count('\n');
                }
                else if (delimiter.IsBlockCommentStart)
                {
                    var blockEnd = Literals.CommentDelimiter.BlockEnd.Value;
                    var comment = ReadUntilSuffix(stream, blockEnd);
                    _tokenBeginning += comment.Length;
                    _newlines += comment.Count('\n');

                    if (stream.EndOfStream)
                    {
                        throw new Exception($"Unterminated block comment at line {_newlines + 1} (reached end of file)");
                    }
                }
                else if (delimiter.IsBlockCommentEnd)
                {
                    throw new Exception($"Unexpected end of comment at line {_newlines + 1}");
                }

                possibleToken = GetNextTerm(stream);
                parseSuccess = Tokens.Token.TryParse(_tokenBeginning, possibleToken, out token);
                _tokenBeginning += possibleToken.Length;
                _newlines += possibleToken.Count('\n');
            }

            return token;
        }

        private static string ReadWhile(StreamReader stream, Func<string, bool> term_condition)
        {
            string term = "";
            while (!stream.EndOfStream && term_condition(term + (char)stream.Peek()))
            {
                term += (char)stream.Read();
            }
            return term;
        }

        private static string ReadUntilSuffix(StreamReader stream, string suffix)
        {
            var result = ReadWhile(stream, term => (
                !term.EndsWith(suffix)
            ));
            // Extract the last suffix char from the stream
            result += (char)stream.Read();
            return result;
        }
    }
}
