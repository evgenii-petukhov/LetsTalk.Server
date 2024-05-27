using LetsTalk.Server.SignPackage.Models;

namespace LetsTalk.Server.UnitTests.Models.Signable;

public class NoPropertiesSignable : ISignable
{
    public string? Signature { get; set; }
}
