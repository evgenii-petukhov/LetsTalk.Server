using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.UnitTests.Models.Signable;

public class NoPropertiesSignable : ISignable
{
    public string? Signature { get; set; }
}
