﻿using System;
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
        private long _newlines;

        public Tokenizer(string sourcePath)
        {
            SourcePath = sourcePath;
        }

        public IEnumerable<Tokens.Token> GetTokens()
        {
            _position = 0;
            _newlines = 0;
            using var file = new StreamReader(SourcePath, Encoding.UTF8);
            while (!file.EndOfStream)
            {
                yield return GetNextToken(file);
            }
            yield return new Tokens.EndOfFile(_position);
        }

        private Tokens.Token ParseNextToken(StreamReader stream)
        {
            // Read until we find a valid token or whitespace
            var currentTerm = ReadWhile(stream, term => (
                !Tokens.Token.TryParse(_position, term, out var _) &&
                (term.Length == 0 || !char.IsWhiteSpace(term[^1]))
            ));

            // Proceed reading while it still represents a valid token
            currentTerm += ReadWhile(stream, suffix => Tokens.Token.TryParse(_position, currentTerm + suffix, out var _));

            bool parseSuccess = Tokens.Token.TryParse(_position, currentTerm, out var token);
            if (!parseSuccess)
            {
                throw new Exception($"Unable to parse token '{currentTerm}' at line {_newlines + 1}");
            }
            _position += currentTerm.Length;
            _newlines += currentTerm.Count('\n');
            return token;
        }

        private Tokens.Token GetNextToken(StreamReader stream)
        {
            var token = ParseNextToken(stream);

            var stringBoundary = Literals.Delimiter.StringQuote.Value;
            var stringEscape = Literals.Delimiter.StringQuoteEscape.Value;

            if (token.Literal == stringBoundary)
            {
                var stringContent = "";
                // Read in a loop since a quote might be not an actual end of string
                do
                {
                    stringContent += ReadUntilSuffix(stream, stringBoundary);
                }
                while (stringContent.EndsWith(stringEscape));

                if (stream.EndOfStream)
                {
                    throw new Exception($"Unterminated string started at line {_newlines + 1}, reached end of file");
                }

                token = new Tokens.StringLiteral(
                    _position,
                    stringContent.RemoveSuffix(stringBoundary).Replace(stringEscape, stringBoundary)
                );

                _position += stringContent.Length;
                _newlines += stringContent.Count('\n');
            }

            while (token is Tokens.CommentDelimiter delimiter)
            {
                if (delimiter.IsLineCommentStart)
                {
                    var comment = ReadWhile(stream, term => term[^1] != '\n');
                    _position += comment.Length;
                }
                else if (delimiter.IsBlockCommentStart)
                {
                    var blockEnd = Literals.CommentDelimiter.BlockEnd.Value;
                    var comment = ReadUntilSuffix(stream, blockEnd);

                    if (stream.EndOfStream)
                    {
                        throw new Exception($"Unterminated block comment started at line {_newlines + 1}, reached end of file");
                    }

                    _position += comment.Length;
                    _newlines += comment.Count('\n');
                }
                else if (delimiter.IsBlockCommentEnd)
                {
                    throw new Exception($"Unexpected end of comment at line {_newlines + 1}");
                }

                token = ParseNextToken(stream);
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
