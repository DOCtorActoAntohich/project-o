using System.Collections.Generic;
using OCompiler.Analyze.Lexical.Tokens;

namespace OCompiler.Utils;

internal class TokenEnumerator
{
    public int Index { get; private set; }
    private readonly BufferedEnumerator<Token> _tokens;
    
    public TokenEnumerator(IEnumerable<Token> tokens)
    {
        _tokens = new BufferedEnumerator<Token>(tokens);
    }
    
    public Token Next(bool skipWhitespaces = true)
    {
        // Skip whitespaces.
        while (_tokens.MoveNext() && skipWhitespaces && _tokens.Current is Whitespace)
        {
            Index += 1;
        }

        Index += 1;
        
        return _tokens.Current;
    }
    
    public Token Current(bool skipWhitespaces = true)
    {
        // Skip whitespaces.
        while (skipWhitespaces && _tokens.Current is Whitespace && _tokens.MoveNext()) { }

        return _tokens.Current;
    }

    public Token Back(bool skipWhitespaces = true)
    {
        // Skip whitespaces.
        while (_tokens.MoveBack() && skipWhitespaces && _tokens.Current is Whitespace)
        {
            Index -= 1;
        }
        
        Index -= 1;
        
        return _tokens.Current;
    }

    public void RestoreIndex(int index)
    {
        // Back.
        if (Index > index)
        {
            while (Index != index)
            {
                Back();
            }
            
            return;
        }
        
        // Next.
        while (Index != index)
        {
            Next();
        }
    }
}
