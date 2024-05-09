namespace LetsTalk.Server.SignPackage.Abstractions;

public interface ISignable
{
    public string? Signature { get; set; }

    public long SignatureDate { get; set; }
}
