using HttpFunctionGenerator;
using HttpFunctionGenerator.SourceProviders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HttpFunctionGeneratorTests;

public class GeneratorSimpleFunctionTests
{
    private const string Source = @"using HttpFunction.Attributes;
using HttpFunction.Models;

namespace HttpFunctionGeneratorTest;

[HttpFunction]
public class C {
    public Outcome CreateResource() {
        return new Outcome(Status.Created);
    }
}";

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
        var s = result.Compilation.GetSymbolsWithName("C_Functions");
        Assert.Single(s);
    }
    
    [Fact]
    public void ShouldBuildSingleMethod()
    {
        const string expected = @"using HttpFunction.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace HttpFunctionGeneratorTest;

public class C_Functions
{
    private readonly C _controller;

    public C_Functions(C controller)
    {
        _controller = controller;
    }

    [Function(""CreateResource"")]
    public HttpResponseData Run(
        [HttpTrigger(AuthorizationLevel.Function, ""post"", Route=null)] HttpRequestData req,
        FunctionContext executionContext)
    {
        var outcome = _controller.CreateResource();
        return req.CreateResponse(outcome);
    }
}";
        
        var result = GeneratorTestFactory.RunGenerator(Source);
        Assert.Equal(expected, result.RunResult.GeneratedTrees.First(t => t.FilePath.Contains("C_Functions")).GetText().ToString());
    }
}