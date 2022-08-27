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

    public static SourceText FromBodyAttributeSource() => SourceText.From($@"using System;

namespace {Constants.PackageBaseName}.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class FromBodyAttribute : Attribute
{{
    public FromBodyAttribute()
    {{
    }}
}}",
        Encoding.UTF8);
    
    public static SourceText FromRouteAttributeSource() => SourceText.From($@"using System;

namespace {Constants.PackageBaseName}.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class FromRouteAttribute : Attribute
{{
    public FromRouteAttribute()
    {{
    }}
}}",
        Encoding.UTF8);
    
    public static SourceText FromQueryAttributeSource() => SourceText.From($@"using System;

namespace {Constants.PackageBaseName}.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class FromQueryAttribute : Attribute
{{
    public FromQueryAttribute()
    {{
    }}
}}",
        Encoding.UTF8);
    
    public static SourceText FromHeaderAttributeSource() => SourceText.From($@"using System;

namespace {Constants.PackageBaseName}.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class FromHeaderAttribute : Attribute
{{
    public FromHeaderAttribute()
    {{
    }}
}}",
        Encoding.UTF8);
}