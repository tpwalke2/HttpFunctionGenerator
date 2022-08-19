using Microsoft.CodeAnalysis;

namespace HttpFunctionGenerator.Extensions;

public static class TypeSymbolExtensions
{
    public static bool InheritsFrom(this ITypeSymbol symbol, ITypeSymbol baseTypeCandidate)
    {
        var baseType = symbol.BaseType;

        while (baseType != null)
        {
            if (SymbolEqualityComparer.Default.Equals(baseTypeCandidate, baseType)) return true;
            baseType = baseType.BaseType;
        }

        return false;
    }
    
    public static bool IsAssignableFrom(this ITypeSymbol methodReturnTypeSymbol, ITypeSymbol assignableTypeCandidate)
    {
        return SymbolEqualityComparer.Default.Equals(methodReturnTypeSymbol, assignableTypeCandidate)
               || methodReturnTypeSymbol.InheritsFrom(assignableTypeCandidate);
    }
}