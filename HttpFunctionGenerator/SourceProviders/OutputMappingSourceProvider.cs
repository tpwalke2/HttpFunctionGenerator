using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace HttpFunctionGenerator.SourceProviders;

public static class OutputMappingSourceProvider
{
    public static SourceText HttpRequestDataMappingSource() => SourceText.From(
        $@"using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using {Constants.PackageBaseName}.Models;
using {Constants.PackageBaseName}.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace {Constants.PackageBaseName}.Mapping;

public static class HttpRequestDataOutputMappingExtension
{{
    public static HttpResponseData CreateResponse(
        this HttpRequestData request,
        Outcome handlerOutcome)
    {{
        return request.CreateResponseFromOutcome(handlerOutcome);
    }}
    
    public static async Task<HttpResponseData> CreateResponse<TResult>(
        this HttpRequestData request,
        Outcome<TResult> handlerOutcome)
    {{
        var response = request.CreateResponseFromOutcome(handlerOutcome);

        var content = BuildContent(handlerOutcome.Result);

        response.Headers.Add(""Content-Type"", $""{{content.ContentType}}; charset=utf-8"");

        if (content.Content != null) await response.WriteStringAsync(content.Content);

        return response;
    }}

    private static HttpResponseData CreateResponseFromOutcome(this HttpRequestData request, Outcome handlerOutcome)
    {{
        var response = request.CreateResponse(ToStatusCode(handlerOutcome.Status));

        if (!string.IsNullOrEmpty(handlerOutcome.Message))
            request.FunctionContext
                .GetLogger(request.FunctionContext.FunctionDefinition.Name)
                .LogInformation(handlerOutcome.Message);

        return response;
    }}
    
    private static (string ContentType, string Content) BuildContent<TResult>(TResult result)
    {{
        if (typeof(TResult) == typeof(string) || typeof(TResult).IsPrimitive) return (MediaTypeNames.Text.Plain, result?.ToString());

        return (MediaTypeNames.Application.Json, result == null ? null : Json.Serialize(result));
    }}

    private static HttpStatusCode ToStatusCode(Status outcomeStatus)
    {{
        switch (outcomeStatus)
        {{
            case Status.InvalidInput:
                return HttpStatusCode.BadRequest;
            case Status.NoContent:
                return HttpStatusCode.NoContent;
            case Status.NotFound:
                return HttpStatusCode.NotFound;
            case Status.Created:
                return HttpStatusCode.Created;
            case Status.Updated:
            case Status.Deleted:
            case Status.Success:
                return HttpStatusCode.OK;
            case Status.UnknownError:
            default:
                return HttpStatusCode.InternalServerError;
        }}
    }}
}}",
        Encoding.UTF8);
}