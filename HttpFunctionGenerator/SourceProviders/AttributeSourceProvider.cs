using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace HttpFunctionGenerator.SourceProviders;

public static class AttributeSourceProvider
{
    public static SourceText FunctionAttributeSource() => SourceText.From($@"using System;

namespace {Constants.PackageBaseName}.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class HttpFunctionAttribute : Attribute
{{
    public HttpFunctionAttribute()
    {{
    }}
}}",
        Encoding.UTF8);
    
    public static SourceText FromAttributeEnumSource() => SourceText.From(
        $@"namespace {Constants.PackageBaseName}.Attributes;

public enum FromSource {{
    Unspecified,
    Body,
    Header,
    Query,
    Route
}}",
        Encoding.UTF8);
    
    public static SourceText BaseFromAttributeSource() => SourceText.From($@"using System;

namespace {Constants.PackageBaseName}.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public abstract class BaseFromAttribute : Attribute
{{
    public BaseFromAttribute(FromSource source = FromSource.Unspecified)
    {{
        Source = source;
    }}

    public FromSource Source {{ get; init; }}
}}",
        Encoding.UTF8);

    public static SourceText FromAttribute(string source) => SourceText.From($@"using System;

namespace {Constants.PackageBaseName}.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class From{source}Attribute : BaseFromAttribute
{{
    public From{source}Attribute() : base(FromSource.{source})
    {{
    }}
}}",
        Encoding.UTF8);
}