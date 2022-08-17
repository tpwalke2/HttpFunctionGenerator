using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace HttpFunctionGenerator.SourceProviders;

public static class OutcomeSourceProvider
{
    public static SourceText OutcomeSource() => SourceText.From(@"namespace HttpFunction.Models;

public record Outcome(Status Status, string Message = null);
public record Outcome<TResult>(Status Status, TResult Result = default, string Message = null) : Outcome(Status, Message);

public enum Status
{
    UnknownError,
    InvalidInput,
    NoContent,
    NotFound,
    Created,
    Updated,
    Deleted,
    Success
}",
        Encoding.UTF8);
}