using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace HttpFunctionGenerator;

public static class AttributeSourceProvider
{
    public static SourceText FunctionAttributeSource() => SourceText.From(@"
using System;
namespace HttpFunction
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class HttpFunctionAttribute : Attribute
    {
        public HttpFunctionAttribute()
        {
        }
    }
}
",
        Encoding.UTF8);
    
    
}