using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace HttpFunctionGenerator.Plumbing.Extensions;

public static class ClassDeclarationSyntaxExtensions
{
    public static INamedTypeSymbol? NamedTypeSymbol(this ClassDeclarationSyntax classDeclarationSyntax, Compilation compilation)
    {
        if (compilation == null) throw new ArgumentNullException(nameof(compilation));

        return compilation
            .GetSemanticModel(classDeclarationSyntax.SyntaxTree)
            .GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
    }
}