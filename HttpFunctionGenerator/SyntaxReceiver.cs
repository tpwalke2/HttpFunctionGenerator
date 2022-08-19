using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace HttpFunctionGenerator;

public class SyntaxReceiver : ISyntaxContextReceiver
{
    private readonly HashSet<ClassDeclarationSyntax> _candidateClasses = new();
    public IReadOnlyCollection<ClassDeclarationSyntax> CandidateClasses => _candidateClasses;
        
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax
            || !classDeclarationSyntax.AttributeLists.Any()) return;
        
        var x = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclarationSyntax);
        if (x != null
            && !x.GetAttributes().Any(ad => ad.AttributeClass != null
                                            && ad.AttributeClass.ToDisplayString().Equals($"{Constants.PackageBaseName}.Attributes.HttpFunctionAttribute")))
            return;

        if (!classDeclarationSyntax.Modifiers.Any(y => y.IsKind(SyntaxKind.PublicKeyword))) return;
        
        _candidateClasses.Add(classDeclarationSyntax);
    }
}