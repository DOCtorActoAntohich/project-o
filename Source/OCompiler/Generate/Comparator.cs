using System;
using System.Collections.Generic;
using System.Text;

namespace OCompiler.Generate;

internal class Comparator : IEqualityComparer<Type[]>
{
    public bool Equals(Type[]? x, Type[]? y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }
        
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }
        
        return true;
    }

    public int GetHashCode(Type[] obj)
    {
        StringBuilder result = new StringBuilder();
        foreach (var type in obj)
        {
            result.Append($"{type}");
        }

        return result.ToString().GetHashCode();
    }
}
