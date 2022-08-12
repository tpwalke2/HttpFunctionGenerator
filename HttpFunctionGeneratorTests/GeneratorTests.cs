using HttpFunctionGenerator;
using Microsoft.CodeAnalysis;
using System.Linq;
using Xunit;

namespace HttpFunctionGeneratorTests;

public class GeneratorTests
{
    [Fact]
    public void TestNoAttribute()
    {
        const string source = @"
class C { }
";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.False(result.Diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void TestWithAttributeNoMethod()
    {
        const string source = @"
[HttpFunction]
class C { }
";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.True(result.Diagnostics.Any(x => x.Id == GeneratorException.Reason.NoMethod.Description()));
    }
}