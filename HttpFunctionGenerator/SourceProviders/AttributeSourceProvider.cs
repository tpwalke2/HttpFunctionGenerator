using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace HttpFunctionGenerator.SourceProviders;

public static class AttributeSourceProvider
{
    public static SourceText FunctionAttributeSource() => SourceText.From($@"using System;

namespace {Constants.PackageBaseName}.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class HttpFunctionAttribute : Attribute
{{
    public HttpFunctionAttribute()
    {{
    }}
}}",
        Encoding.UTF8);
}