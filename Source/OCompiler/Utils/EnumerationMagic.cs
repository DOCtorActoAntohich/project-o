using System.Collections.Generic;

namespace OCompiler.Utils;

public static class EnumerationMagic
{
    public static IEnumerable<T> MakeEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}