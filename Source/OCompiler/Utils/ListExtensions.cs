using System;
using System.Collections.Generic;
using System.Linq;

namespace OCompiler.Utils;

public static class ListExtensions
{
    public static bool EqualsByValue<T>(this List<T> me, List<T> other) where T : IEquatable<T>
    {
        if (me.Count != other.Count)
        {
            return false;
        }

        return !me.Where((t, i) => !t.Equals(other[i])).Any();
    }
}