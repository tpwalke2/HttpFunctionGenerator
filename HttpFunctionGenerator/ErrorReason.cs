using System.ComponentModel;

namespace HttpFunctionGenerator;

public enum ErrorReason
{
    Unknown,
    [Description("HFG100")]
    NoMethod
}