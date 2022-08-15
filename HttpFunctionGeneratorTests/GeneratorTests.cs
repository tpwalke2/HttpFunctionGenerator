using Microsoft.CodeAnalysis;
using System.Linq;
using Xunit;

namespace HttpFunctionGeneratorTests;

public class GeneratorSimpleFunctionTests
{
    [Fact]
    public void CreateSingleContainerClass()
    {
        const string source = @"
using HttpFunction;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {
    public void CreateResource() {}
}";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.Empty(result.Diagnostics);

        var s = result.Compilation.GetSymbolsWithName("C_Functions");
        Assert.Single(s);
    }
}