namespace LetsTalk.Server.SignPackage.Abstractions;

public interface ISignPackageService
{
    void Sign(object objectToSign);

    bool Validate(ISignable signable);
}
