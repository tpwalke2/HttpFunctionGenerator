using HttpFunctionGenerator;
using System.Linq;
using Xunit;

namespace HttpFunctionGeneratorTests;

public class GenerateSimpleGetFunctionTests
{
    private static readonly string Source = $@"using {Constants.PackageBaseName}.Attributes;
using {Constants.PackageBaseName}.Models;
using System.Threading.Tasks;

namespace HttpFunctionGeneratorTest;

public record ResourceResult(string Name); 

[HttpFunction]
public class C {{
    public Task<Outcome<ResourceResult>> GetResource() {{
        return Task.FromResult(new Outcome<ResourceResult>(Status.Created, new ResourceResult(""John Doe"")));
    }}
}}";
    
    [Fact]
    public void ShouldNotHaveDiagnosticErrors()
    {
        var result = GeneratorTestFactory.RunGenerator(Source);
        Assert.Empty(result.Diagnostics.After);
    }

    [Fact]
    public void ShouldBuildGetFunction()
    {
        var expected = $@"using {Constants.PackageBaseName}.Mapping;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;

namespace HttpFunctionGeneratorTest;

public class C_Functions
{{
    private readonly C _controller;

    public C_Functions(C controller)
    {{
        _controller = controller;
    }}

    [Function(""GetResource"")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(
            AuthorizationLevel.Function,
            ""get"",
            Route = null)] HttpRequestData req,
        FunctionContext executionContext)
    {{
        var outcome = await _controller.GetResource();
        return await req.CreateResponse(outcome);
    }}
}}";
        
        var result = GeneratorTestFactory.RunGenerator(Source);
        Assert.Equal(expected, result.RunResult.GeneratedTrees.First(t => t.FilePath.Contains("C_Functions")).GetText().ToString());
    }
}