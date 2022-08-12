using Microsoft.CodeAnalysis;
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
        
        var x = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
        if (x.GetAttributes().Any(ad => ad.AttributeClass.ToDisplayString().Equals("HttpFunction.HttpFunctionAttribute")))
        {
            _candidateClasses.Add(classDeclarationSyntax);
        }
    }
}