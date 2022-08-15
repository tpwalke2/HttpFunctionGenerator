using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace HttpFunctionGenerator.Plumbing.Extensions;

public static class EnumExtensions
{
    public static string Description(this Enum value)
    {
        return
            value
                .GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description
            ?? value.ToString();
    }
}