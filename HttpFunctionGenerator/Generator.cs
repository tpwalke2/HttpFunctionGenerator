using HttpFunctionGenerator.Extensions;
using HttpFunctionGenerator.Plumbing.Extensions;
using HttpFunctionGenerator.SourceProviders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
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
        var typeSymbol = context.Compilation.GetTypeByMetadataName("Microsoft.Azure.Functions.Worker.FunctionAttribute");
        if (typeSymbol == null)
        {
            var loc = DiagnosticDescriptors.GetLocation(DiagnosticDescriptors.FilePath(),
                DiagnosticDescriptors.LineNumber());
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingDependencies(), loc));
            return;
        }
        
        // if there is no SyntaxRetriever, there is no work to do
        if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
        {
            var loc = DiagnosticDescriptors.GetLocation(DiagnosticDescriptors.FilePath(), DiagnosticDescriptors.LineNumber());
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.HFG1SyntaxReceiver(), loc));
            return;
        }

        var outcomeTypeSymbol = context.Compilation.GetTypeByMetadataName($"{Constants.PackageBaseName}.Models.Outcome");
        
        foreach (var classDeclarationSyntax in receiver.CandidateClasses)
        {
            var publicMethods = classDeclarationSyntax
                .Members
                .Where(member => member.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PublicKeyword)))
                .Where(member => member.IsKind(SyntaxKind.MethodDeclaration))
                .Cast<MethodDeclarationSyntax>()
                .Select(method => (method, method.GetReturnType(context.Compilation.GetSemanticModel(method.SyntaxTree))))
                .Where(x => HasValidReturnType(x.Item2, outcomeTypeSymbol))
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
                BuildFunctionClass(classDeclarationSyntax, publicMethods, context));
        }
    }

    private static bool HasValidReturnType(
        ITypeSymbol methodReturnTypeSymbol,
        ITypeSymbol outcomeTypeSymbol)
    {
        if (methodReturnTypeSymbol.IsAssignableFrom(outcomeTypeSymbol)) return true;

        if (methodReturnTypeSymbol is not INamedTypeSymbol { IsGenericType: true } namedTypeSymbol) return false;

        return namedTypeSymbol.TypeArguments.Length == 1 
               && namedTypeSymbol.TypeArguments[0].IsAssignableFrom(outcomeTypeSymbol);
    }

    private static SourceText BuildFunctionClass(
        ClassDeclarationSyntax classDeclarationSyntax,
        IEnumerable<(MethodDeclarationSyntax Method, ITypeSymbol ReturnType)> publicMethods,
        GeneratorExecutionContext context)
    {
        var namespaceName = classDeclarationSyntax.NamedTypeSymbol(context.Compilation).ContainingNamespace.ToDisplayString();

        var builtMethods = publicMethods
            .Select(pm => BuildFunctionMethod(pm, context))
            .ToList();

        var asyncUsing = builtMethods.Any(x => x.IsAsync)
            ? @"
using System.Threading.Tasks;"
            : "";

        var source = new StringBuilder($@"using {Constants.PackageBaseName}.Mapping;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;{asyncUsing}

namespace {namespaceName};

public class {classDeclarationSyntax.Identifier.ValueText}_Functions
{{
    private readonly {classDeclarationSyntax.Identifier.ValueText} _controller;

    public {classDeclarationSyntax.Identifier.ValueText}_Functions({classDeclarationSyntax.Identifier.ValueText} controller)
    {{
        _controller = controller;
    }}
");

        builtMethods.ForEach(x => source.AppendLine(x.MethodText));
        
        source.Append(@"}");

        return SourceText.From(source.ToString(), Encoding.UTF8);
    }

    private static (bool IsAsync, string MethodText) BuildFunctionMethod(
        (MethodDeclarationSyntax Method, ITypeSymbol ReturnType) methodInfo,
        GeneratorExecutionContext context)
    {
        var methodName = methodInfo.Method.Identifier.Text;
        var verb = methodName.StartsWith("Get", StringComparison.OrdinalIgnoreCase)
            ? "get"
            : "post";

            var taskSymbol = context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
        var isAsync = methodInfo.ReturnType.InheritsFrom(taskSymbol);
        
        var returnType = isAsync
            ? "async Task<HttpResponseData>"
            : "HttpResponseData";
        var awaitPrefix = isAsync ? "await " : "";
        var asyncSuffix = isAsync ? "Async" : "";

        return (isAsync, $@"
    [Function(""{methodName}"")]
    public {returnType} Run{asyncSuffix}(
        [HttpTrigger(
            AuthorizationLevel.Function,
            ""{verb}"",
            Route = null)] HttpRequestData req,
        FunctionContext executionContext)
    {{
        var outcome = {awaitPrefix}_controller.{methodName}();
        return {awaitPrefix}req.CreateResponse(outcome);
    }}");
    }
}