using LetsTalk.Server.SignPackage.Abstractions;
using LetsTalk.Server.SignPackage.Models;
using System.Text;

namespace LetsTalk.Server.SignPackage;

public class SignPackageService : ISignPackageService
{
    private const char Separator = ';';
    private readonly string[] _supportedTypes = ["System.Int32", "System.String"];

    public void Sign(object objectToSign)
    {
        if (objectToSign == null || objectToSign is not ISignable signable)
        {
            return;
        }

        signable.Signature = BCrypt.Net.BCrypt.EnhancedHashPassword(
            GetPropertiesAsString(signable),
            11,
            BCrypt.Net.HashType.SHA384);
    }

    public bool Validate(ISignable signable)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(
            GetPropertiesAsString(signable),
            signable.Signature,
            BCrypt.Net.HashType.SHA384);
    }

    private string GetPropertiesAsString(ISignable signable)
    {
        var stringPairs = signable
            .GetType()
            .GetProperties()
            .Where(x => {
                var isSignature = string.Equals(x.Name, nameof(ISignable.Signature), StringComparison.Ordinal);
                var isSupported = _supportedTypes.Contains(x.PropertyType.FullName);

                return !isSignature && isSupported && x.GetValue(signable) != null;
            })
            .Select(x => new
            {
                x.Name,
                Value = x.GetValue(signable)
            })
            .OrderBy(x => x.Name)
            .Select(x => $"{x.Name}={x.Value}")
            .ToList();

        if (stringPairs.Count == 0)
        {
            throw new InvalidOperationException("There are no supported properties to sign");
        }

        return new StringBuilder()
            .AppendJoin(Separator, stringPairs)
            .Append(stringPairs.Count != 0 ? Separator : string.Empty)
            .ToString();
    }
}
