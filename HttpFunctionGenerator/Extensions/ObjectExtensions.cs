using System.Collections.Generic;

namespace HttpFunctionGenerator.Extensions;

public static class ObjectExtensions
{
    public static bool IsDefault<T>(this T value)
    {
        return EqualityComparer<T>.Default.Equals(value, default);
    }
}
