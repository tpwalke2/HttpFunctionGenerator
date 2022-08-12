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

        public static DiagnosticDescriptor NoMethod(string className, string id)
        {
            return new DiagnosticDescriptor(
                 $"{id}",
                "Base",
                $"{className} has no public methods",
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