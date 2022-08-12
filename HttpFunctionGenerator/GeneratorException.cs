using Microsoft.CodeAnalysis;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HttpFunctionGenerator;

public class GeneratorException : Exception
{
    public enum Reason
    {
        Unknown,
        [Description("HFG100")]
        NoMethod
    }

    private readonly Reason _reason = Reason.Unknown;
    private readonly Location _location = Location.None;
    private readonly string _reasonContext = "";
    
    public GeneratorException()
    {
    }

    public GeneratorException(string message)
        : base(message)
    {
    }

    public GeneratorException(string message, Exception inner)
        : base(message, inner)
    {
    }
    public GeneratorException(Reason reason, Location location, string reasonContext = "")
    {
        _reason = reason;
        _location = location;
        _reasonContext = reasonContext;
    }

    public void ReportDiagnostic(GeneratorExecutionContext context, string filePath = "")
    {
        var location = _location;

        if (_location == Location.None)
        {
            location = DiagnosticDescriptors.GetLocation(filePath, LineNumber());
        }

        context.ReportDiagnostic(Diagnostic.Create(CreateDescriptor(_reason, _reasonContext), location));
    }

    private static DiagnosticDescriptor CreateDescriptor(Reason reason, string context) => reason switch
    {
        Reason.Unknown => throw new ArgumentOutOfRangeException(reason.ToString()),
        Reason.NoMethod => DiagnosticDescriptors.NoMethod(context, reason.Description()),
        _ => throw new ArgumentOutOfRangeException(reason.ToString())
    };

    private int LineNumber()
    {
        var match = Regex.Match(StackTrace, @":\w*\s(?<line>\d+)");
        if (match.Success)
        {
            return Convert.ToInt32(match.Groups["line"].Value, CultureInfo.InvariantCulture);
        }

        return -1;
    }
}