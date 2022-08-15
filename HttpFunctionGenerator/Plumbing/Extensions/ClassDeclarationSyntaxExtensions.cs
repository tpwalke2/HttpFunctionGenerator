using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace HttpFunctionGenerator.Plumbing.Extensions;

public static class ClassDeclarationSyntaxExtensions
{
    public static INamedTypeSymbol NamedTypeSymbol(this ClassDeclarationSyntax classDeclarationSyntax, Compilation compilation)
    {
        if (compilation == null) throw new ArgumentNullException(nameof(compilation));

        var model = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        return model.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
    }
}