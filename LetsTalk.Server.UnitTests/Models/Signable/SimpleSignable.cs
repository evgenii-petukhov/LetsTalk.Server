using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.UnitTests.Models.Signable;

public class SimpleSignable : ISignable
{
    public int A { get; set; }

    public string? B { get; set; }

    public string? C { get; set; }

    public bool D { get; set; }

    public List<string>? E { get; set; }

    public string? Signature { get; set; }
}
