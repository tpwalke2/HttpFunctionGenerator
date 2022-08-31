using HttpFunctionGenerator;
using System.Linq;
using Xunit;

namespace HttpFunctionGeneratorTests;

public class GenerateSimplePostFunctionTests
{
    private static readonly string Source = $@"using {Constants.PackageBaseName}.Attributes;
using {Constants.PackageBaseName}.Models;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {{
    public Outcome PostResource() {{
        return new Outcome(Status.Created);
    }}
}}";

    [Fact]
    public void ShouldNotHaveDiagnosticErrors()
    {
        var result = GeneratorTestFactory.RunGenerator(Source);
        Assert.Empty(result.Diagnostics.After);
    }

    [Fact]
    public void ShouldCreateSingleContainerClass()
    {
        var result = GeneratorTestFactory.RunGenerator(Source);
        var s = result.Compilation?.GetSymbolsWithName("C_Functions");
        Assert.NotNull(s);
        Assert.Single(s);
    }
    
    [Fact]
    public void ShouldBuildSingleMethod()
    {
        var expected = $@"using {Constants.PackageBaseName}.Mapping;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace HttpFunctionGeneratorTest;

public class C_Functions
{{
    private readonly C _controller;

    public C_Functions(C controller)
    {{
        _controller = controller;
    }}

    [Function(""PostResource"")]
    public HttpResponseData PostResource(
        [HttpTrigger(
            AuthorizationLevel.Function,
            ""post"",
            Route = null)] HttpRequestData req,
        FunctionContext executionContext)
    {{
        var outcome = _controller.PostResource();
        return req.CreateResponse(outcome);
    }}
}}";
        
        var result = GeneratorTestFactory.RunGenerator(Source);
        Assert.Equal(expected, result.RunResult?.GeneratedTrees.First(t => t.FilePath.Contains("C_Functions")).GetText().ToString());
    }
}