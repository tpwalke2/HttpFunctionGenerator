using Xunit;

namespace HttpFunctionGeneratorTests;

public class GeneratorSimpleFunctionTests
{
    [Fact]
    public void CreateSingleContainerClass()
    {
        const string source = @"
using HttpFunction.Attributes;
using HttpFunction.Models;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {
    public Outcome CreateResource() {
        return new Outcome(Status.Created);
    }
}";
        var result = GeneratorTestFactory.RunGenerator(source);
        Assert.Empty(result.Diagnostics);

        var s = result.Compilation.GetSymbolsWithName("C_Functions");
        Assert.Single(s);
    }
}