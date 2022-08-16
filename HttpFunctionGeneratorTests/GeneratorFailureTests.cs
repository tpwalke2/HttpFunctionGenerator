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
        Assert.False(result.Diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error));
        Assert.Equal(2, result.Compilation.SyntaxTrees.Count());
    }
    
    [Fact]
    public void TestNoPublicClass()
    {
        const string source = @"
using HttpFunction;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
class C { }
";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.False(result.Diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error));
        Assert.Equal(2, result.Compilation.SyntaxTrees.Count());
    }

    [Fact]
    public void TestWithAttributeNoMethod()
    {
        const string source = @"
using HttpFunction;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {}";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.Single(result.Diagnostics);
        Assert.Equal("HFG100", result.Diagnostics[0].Id);
        Assert.Equal(2, result.Compilation.SyntaxTrees.Count());
    }
}