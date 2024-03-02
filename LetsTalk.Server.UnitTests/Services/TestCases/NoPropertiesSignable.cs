using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.UnitTests.Services.TestCases;

public class NoPropertiesSignable : ISignable
{
    public string? Signature { get; set; }
}
