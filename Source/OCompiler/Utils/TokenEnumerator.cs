using System.Collections.Generic;
using System.Linq;
using OCompiler.Analyze.Lexical.Tokens;

namespace OCompiler.Utils;

internal class TokenEnumerator
{
    private readonly List<Token> _tokens;
    private int _index;
    
    public TokenEnumerator(IEnumerable<Token> tokens)
    {
        _tokens = tokens.ToList();
        _index = 0;
    }
    
    public Token Next(bool skipWhitespaces = true)
    {
        // Skip whitespaces.
        while (MoveNext() && skipWhitespaces && _tokens[_index] is Whitespace) { }
        
        return _tokens[_index];
    }
    
    public Token Current(bool skipWhitespaces = true)
    {
        // Skip whitespaces.
        while (skipWhitespaces && _tokens[_index] is Whitespace && MoveNext()) { }

        return _tokens[_index];
    }

    public Token Back(bool skipWhitespaces = true)
    {
        // Skip whitespaces.
        while (MoveBack() && skipWhitespaces && _tokens[_index] is Whitespace) { }
        
        return _tokens[_index];
    }
    
    private bool MoveNext()
    {
        if (_tokens.Count - 1 == _index)
        {
            return false;
        }
        
        ++_index;
        return true;
    }
    
    private bool MoveBack()
    {
        if (_index == 0)
        {
            return false;
        }
        
        --_index;
        return true;
    }
}
