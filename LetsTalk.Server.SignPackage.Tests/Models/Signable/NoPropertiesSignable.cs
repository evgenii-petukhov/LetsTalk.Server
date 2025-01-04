using LetsTalk.Server.SignPackage.Models;

namespace LetsTalk.Server.SignPackage.Tests.Models.Signable;

public class NoPropertiesSignable : ISignable
{
    public string? Signature { get; set; }
}
