using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.SignPackage.Abstractions;
using LetsTalk.Server.SignPackage.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace LetsTalk.Server.SignPackage;

public class SignPackageService(IOptions<SignPackageSettings> options) : ISignPackageService
{
    private const char Separator = ';';
    private readonly string _salt = options.Value?.Salt ?? string.Empty;
    private readonly string[] _supportedTypes = ["System.Int32", "System.String"];

    public void Sign(object obj)
    {
        if (obj == null || obj is not ISignable signable)
        {
            return;
        }

        signable.Signature = GetSignature(signable);
    }

    public bool Validate(ISignable signable)
    {
        return string.Equals(GetSignature(signable), signable.Signature, StringComparison.OrdinalIgnoreCase);
    }

    private string GetSignature(ISignable signable)
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
            throw new Exception("There is no supported properties to sign");
        }

        var s = new StringBuilder()
            .AppendJoin(Separator, stringPairs)
            .Append(stringPairs.Count != 0 ? Separator : string.Empty)
            .Append("salt=")
            .Append(_salt)
            .ToString();

        var hashBytes = MD5.HashData(Encoding.Default.GetBytes(s));
        return Convert.ToHexString(hashBytes);
    }
}
