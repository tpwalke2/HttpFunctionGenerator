using HttpFunctionGenerator.Extensions;
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
        context.RegisterForPostInitialization(i =>
        {
            i.AddSource("HttpFunctionAttribute.g.cs", AttributeSourceProvider.FunctionAttributeSource());
            i.AddSource("Outcome.g.cs", OutcomeSourceProvider.OutcomeSource());
            i.AddSource("JsonSerialization.g.cs", SerializationSourceProvider.JsonSerializationSource());
            i.AddSource("HttpRequestDataOutputMappingExtension.g.cs", OutputMappingSourceProvider.HttpRequestDataMappingSource());
            i.AddSource("ServiceCollectionExtensions.g.cs", ExtensionsSourceProvider.ServiceCollectionExtensionsSource());
            i.AddSource("HostBuilderExtensions.g.cs", ExtensionsSourceProvider.HostBuilderConfigurationSource());
            i.AddSource("EnumerableExtensions.g.cs", ExtensionsSourceProvider.EnumerableExtensionsSource());
            i.AddSource("TypeHelpers.g.cs", HelpersSourceProvider.TypeHelpersSource());
        });

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

        //var attributeSymbol = context.Compilation.GetTypeByMetadataName("HttpFunction.Attributes.HttpFunctionAttribute");
        var outcomeTypeSymbol = context.Compilation.GetTypeByMetadataName("HttpFunction.Models.Outcome");
        
        foreach (var classDeclarationSyntax in receiver.CandidateClasses)
        {
            var publicMethods = classDeclarationSyntax
                .Members
                .Where(member => member.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PublicKeyword)))
                .Where(member => member.IsKind(SyntaxKind.MethodDeclaration))
                .Cast<MethodDeclarationSyntax>()
                .Where(method => SymbolEqualityComparer
                    .Default
                    .Equals(
                        method.GetReturnType(context.Compilation.GetSemanticModel(method.SyntaxTree)),
                        outcomeTypeSymbol))
                .ToList();

            if (!publicMethods.Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(
                                             DiagnosticDescriptors.NoMethod(
                                                 classDeclarationSyntax.Identifier.ValueText),
                                             classDeclarationSyntax.GetLocation()));
                continue;
            }

            context.AddSource(
                $"{classDeclarationSyntax.Identifier.ValueText}_Functions.g.cs",
                CreateFunctionClass(classDeclarationSyntax, publicMethods, context));
        }
    }

    private static SourceText CreateFunctionClass(
        ClassDeclarationSyntax classDeclarationSyntax,
        IList<MethodDeclarationSyntax> publicMethods,
        GeneratorExecutionContext context)
    {
        var namespaceName = classDeclarationSyntax.NamedTypeSymbol(context.Compilation).ContainingNamespace.ToDisplayString();

        var source = new StringBuilder($@"using HttpFunction.Mapping;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace {namespaceName};

public class {classDeclarationSyntax.Identifier.ValueText}_Functions
{{
    private readonly {classDeclarationSyntax.Identifier.ValueText} _controller;

    public {classDeclarationSyntax.Identifier.ValueText}_Functions({classDeclarationSyntax.Identifier.ValueText} controller)
    {{
        _controller = controller;
    }}
");
        
        publicMethods.ForEach(pm =>
        {
            source.AppendLine($@"
    [Function(""{pm.Identifier.Text}"")]
    public HttpResponseData Run(
        [HttpTrigger(
            AuthorizationLevel.Function,
            ""post"",
            Route = null)] HttpRequestData req,
        FunctionContext executionContext)
    {{
        var outcome = _controller.{pm.Identifier.Text}();
        return req.CreateResponse(outcome);
    }}");
        });
        
        source.Append(@"}");

        return SourceText.From(source.ToString(), Encoding.UTF8);
    }
}