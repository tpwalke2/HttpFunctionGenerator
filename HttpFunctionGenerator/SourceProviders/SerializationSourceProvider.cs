using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace HttpFunctionGenerator.SourceProviders;

public static class SerializationSourceProvider
{
    public static SourceText JsonSerializationSource() => SourceText.From(
        $@"using System.Text.Json;
using System.Text.Json.Serialization;

namespace {Constants.PackageBaseName}.Serialization;

public static class Json
{{
    public static T Deserialize<T>(string value)
    {{
        return JsonSerializer.Deserialize<T>(
            value,
            new JsonSerializerOptions()
            {{
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {{
                    new JsonStringEnumConverter()
                }}
            }});
    }}

    public static string Serialize<T>(T input)
    {{
        return JsonSerializer.Serialize(
            input,
            new JsonSerializerOptions()
            {{
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {{
                    new JsonStringEnumConverter()
                }}
            }});
    }}
}}",
        Encoding.UTF8);
}