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
                foreach (var token in ParseNextTerm(file))
                {
                    yield return token;
                };
            }
            yield return new Tokens.EndOfFile(_tokenBeginning);
        }

        private IEnumerable<Tokens.Token> ParseNextTerm(StreamReader stream)
        {
            char symbol = (char)stream.Peek();
            var term = ReadWhile(stream, c => c.IsIdentifierOrNumber() == symbol.IsIdentifierOrNumber());
            var len = term.Length;
            do
            {
                var possibleToken = term[..len];
                if (Tokens.Token.TryParse(_tokenBeginning, possibleToken, out Tokens.Token token))
                {
                    yield return token;
                    _tokenBeginning += len;
                    _newlines += token.Literal.Count('\n');
                    term = term[len..];
                    len = term.Length;
                }
                else if (--len <= 0)
                {
                    throw new Exception($"Unable to parse token '{possibleToken}' at line {_newlines+1}");
                }
            }
            while (term.Length > 0);
        }

        private static string ReadWhile(StreamReader stream, Func<char, bool> condition)
        {
            string word = "";
            int symbol;

            while ((symbol = stream.Peek()) != -1 && condition((char)symbol))
            {
                word += (char)stream.Read();
            }
            
            return word;
        }
    }
}
