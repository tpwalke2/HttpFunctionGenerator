using HttpFunctionGenerator;
using Microsoft.CodeAnalysis;
using System.Linq;
using Xunit;

namespace HttpFunctionGeneratorTests;

public class GeneratorFailureTests
{
    [Fact]
    public void TestNoAttribute()
    {
        const string source = @"
namespace HttpFunctionGeneratorTest;

public class C { }
";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.False(result.Diagnostics.After.Any(x => x.Severity == DiagnosticSeverity.Error));
        var s = result.Compilation?.GetSymbolsWithName("C_Functions");
        Assert.NotNull(s);
        Assert.Empty(s);
    }
    
    [Fact]
    public void TestNoPublicClass()
    {
        var source = $@"
using {Constants.PackageBaseName}.Attributes;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
class C {{}}
";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.False(result.Diagnostics.After.Any(x => x.Severity == DiagnosticSeverity.Error));
        var s = result.Compilation?.GetSymbolsWithName("C_Functions");
        Assert.NotNull(s);
        Assert.Empty(s);
    }

    [Fact]
    public void TestWithAttributeNoMethod()
    {
        var source = $@"
using {Constants.PackageBaseName}.Attributes;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {{}}";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.Single(result.Diagnostics.After);
        Assert.Equal("HFG100", result.Diagnostics.After[0].Id);
        var s = result.Compilation?.GetSymbolsWithName("C_Functions");
        Assert.NotNull(s);
        Assert.Empty(s);
    }
    
    [Fact]
    public void TestWithAttributeNoPublicMethod()
    {
        var source = $@"
using {Constants.PackageBaseName}.Attributes;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {{
    private void DoSomething() {{}}
}}";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.Single(result.Diagnostics.After);
        Assert.Equal("HFG100", result.Diagnostics.After[0].Id);
        var s = result.Compilation?.GetSymbolsWithName("C_Functions");
        Assert.NotNull(s);
        Assert.Empty(s);
    }
    
    [Fact]
    public void TestWithAttributeNoPublicMethodOfCorrectReturnType()
    {
        var source = $@"
using {Constants.PackageBaseName}.Attributes;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {{
    public void DoSomething() {{}}
}}";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.Single(result.Diagnostics.After);
        Assert.Equal("HFG100", result.Diagnostics.After[0].Id);
        var s = result.Compilation?.GetSymbolsWithName("C_Functions");
        Assert.NotNull(s);
        Assert.Empty(s);
    }
    
    [Fact]
    public void TestWithAttributeNoPublicMethodWithCorrectNumberOfParameters()
    {
        var source = $@"using {Constants.PackageBaseName}.Attributes;
using {Constants.PackageBaseName}.Models;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {{
    public Outcome CreateResource(int parameter1, int invalidExtraParameter) {{
        return new Outcome(Status.Created);
    }}
}}";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.Single(result.Diagnostics.After);
        Assert.Equal("HFG100", result.Diagnostics.After[0].Id);
        var s = result.Compilation?.GetSymbolsWithName("C_Functions");
        Assert.NotNull(s);
        Assert.Empty(s);
    }
}