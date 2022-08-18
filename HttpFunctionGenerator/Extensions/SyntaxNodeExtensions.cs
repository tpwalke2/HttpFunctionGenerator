using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HttpFunctionGenerator.Extensions;

public static class SyntaxNodeExtensions
{
    public static ITypeSymbol GetReturnType(this MethodDeclarationSyntax expressionSyntax, SemanticModel semanticModel)
    {
        return semanticModel.GetTypeInfo(expressionSyntax.ReturnType).Type;
    }
}