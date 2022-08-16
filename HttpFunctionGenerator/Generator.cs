using HttpFunctionGenerator.Plumbing.Extensions;
using HttpFunctionGenerator.SourceProviders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpFunctionGenerator;

[Generator]
public class Generator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // register known fixed source
        context.RegisterForPostInitialization(i => i.AddSource("HttpFunctionAttribute.g.cs", AttributeSourceProvider.FunctionAttributeSource()));

        // register a receive to accumulate nodes of interest
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // if there is no SyntaxRetriever, there is no work to do
        if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
        {
            var loc = DiagnosticDescriptors.GetLocation(DiagnosticDescriptors.FilePath(), DiagnosticDescriptors.LineNumber());
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.HFG1SyntaxReceiver(), loc));
            return;
        }

        //var attributeSymbol = context.Compilation.GetTypeByMetadataName("HttpFunction.HttpFunctionAttribute");
        
        foreach (var classDeclarationSyntax in receiver.CandidateClasses)
        {
            var publicMembers = classDeclarationSyntax
                .Members
                .Where(member => member.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PublicKeyword)))
                .ToList();

            if (!publicMembers.Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(
                                             DiagnosticDescriptors.NoMethod(
                                                 classDeclarationSyntax.Identifier.ValueText),
                                             classDeclarationSyntax.GetLocation()));
                continue;
            }

            context.AddSource(
                $"{classDeclarationSyntax.Identifier.ValueText}_Functions.g.cs",
                CreateFunctionClass(classDeclarationSyntax, publicMembers, context));
        }
    }

    private SourceText CreateFunctionClass(
        ClassDeclarationSyntax classDeclarationSyntax,
        IList<MemberDeclarationSyntax> publicMembers,
        GeneratorExecutionContext context)
    {
        var namespaceName = classDeclarationSyntax.NamedTypeSymbol(context.Compilation).ContainingNamespace.ToDisplayString();
        
        var source = new StringBuilder($@"using System;

namespace {namespaceName};

public class {classDeclarationSyntax.Identifier.ValueText}_Functions
{{
}}
");

        return SourceText.From(source.ToString(), Encoding.UTF8);
    }
}