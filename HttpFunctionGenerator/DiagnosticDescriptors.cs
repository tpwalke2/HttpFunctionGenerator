using HttpFunctionGenerator.Plumbing.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace HttpFunctionGenerator;

public static class DiagnosticDescriptors
    {
        public static DiagnosticDescriptor HFG1SyntaxReceiver()
        {
            return new DiagnosticDescriptor(
                "HFG1",
                "No SyntaxReceiver",
                "Can not populate SyntaxReceiver",
                "Compilation",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true);
        }

        public static DiagnosticDescriptor NoMethod(string className)
        {
            return new DiagnosticDescriptor(
                ErrorReason.NoMethod.Description(),
                "Base",
                $"{className} has no public methods with the correct return type",
                "Compilation",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);
        }

        public static DiagnosticDescriptor MissingDependencies()
        {
            return new DiagnosticDescriptor(
                ErrorReason.MissingDependencies.Description(),
                "Base",
                "Missing required dependencies. Ensure that the Microsoft.Azure.Functions.Worker, Microsoft.Azure.Functions.Worker.Extensions.Http, and Microsoft.AzureFunctions.Worker.Sdk NuGet packages are added as dependencies.",
                "Compilation",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);
        }
        
        public static DiagnosticDescriptor Info(string title, string message)
        {
            return new DiagnosticDescriptor(
                "Info",
                title,
                message,
                "Compilation",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: false);
        }

        public static string FilePath([System.Runtime.CompilerServices.CallerFilePath] string filePath = "")
        {
            return filePath;
        }

        public static int LineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }

        public static Location GetLocation(string path, int line)
        {
            var linePosition = new LinePosition(line, 0);
            return Location.Create(path, new TextSpan(0, 0), new LinePositionSpan(linePosition, linePosition));
        }
    }