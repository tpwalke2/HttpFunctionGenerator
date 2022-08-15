using System;
using System.Collections.Generic;

namespace HttpFunctionGenerator.Plumbing.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> itemAction)
    {
        foreach (var item in source)
        {
            itemAction(item);
        }
    }
}