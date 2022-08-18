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
        var s = result.Compilation.GetSymbolsWithName("C_Functions");
        Assert.Empty(s);
    }
    
    [Fact]
    public void TestNoPublicClass()
    {
        const string source = @"
using HttpFunction.Attributes;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
class C { }
";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.False(result.Diagnostics.After.Any(x => x.Severity == DiagnosticSeverity.Error));
        var s = result.Compilation.GetSymbolsWithName("C_Functions");
        Assert.Empty(s);
    }

    [Fact]
    public void TestWithAttributeNoMethod()
    {
        const string source = @"
using HttpFunction.Attributes;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {}";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.Single(result.Diagnostics.After);
        Assert.Equal("HFG100", result.Diagnostics.After[0].Id);
        var s = result.Compilation.GetSymbolsWithName("C_Functions");
        Assert.Empty(s);
    }
    
    [Fact]
    public void TestWithAttributeNoPublicMethod()
    {
        const string source = @"
using HttpFunction.Attributes;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {
    private void DoSomething() {}
}";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.Single(result.Diagnostics.After);
        Assert.Equal("HFG100", result.Diagnostics.After[0].Id);
        var s = result.Compilation.GetSymbolsWithName("C_Functions");
        Assert.Empty(s);
    }
    
    [Fact]
    public void TestWithAttributeNoPublicMethodOfCorrectReturnType()
    {
        const string source = @"
using HttpFunction.Attributes;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {
    public void DoSomething() {}
}";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.Single(result.Diagnostics.After);
        Assert.Equal("HFG100", result.Diagnostics.After[0].Id);
        var s = result.Compilation.GetSymbolsWithName("C_Functions");
        Assert.Empty(s);
    }
}