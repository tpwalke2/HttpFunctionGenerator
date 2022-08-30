using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HttpFunctionGenerator.Extensions;

public static class SyntaxNodeExtensions
{
    public static ITypeSymbol? GetReturnType(this MethodDeclarationSyntax methodDeclarationSyntax, SemanticModel semanticModel)
    {
        return semanticModel.GetTypeInfo(methodDeclarationSyntax.ReturnType).Type;
    }

    public static ITypeSymbol? GetParameterType(
        this MethodDeclarationSyntax methodDeclarationSyntax,
        SemanticModel semanticModel)
    {
        var parameterType = methodDeclarationSyntax.ParameterList.Parameters.FirstOrDefault()?.Type;

        return parameterType == null
            ? null
            : semanticModel.GetTypeInfo(parameterType).Type;
    }
}