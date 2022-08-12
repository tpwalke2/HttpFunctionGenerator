using Microsoft.CodeAnalysis;

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

        var attributeSymbol = context.Compilation.GetTypeByMetadataName("HttpFunction.HttpFunctionAttribute");
    }
}