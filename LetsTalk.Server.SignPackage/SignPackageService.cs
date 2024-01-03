using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.SignPackage.Abstractions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace LetsTalk.Server.SignPackage;

public class SignPackageService: ISignPackageService
{
    private const char Separator = ';';

    private readonly SignPackageSettings _signPackageSettings;

    public SignPackageService(IOptions<SignPackageSettings> options)
    {
        _signPackageSettings = options.Value;
    }

    public void Sign(object obj)
    {
        if (obj is not ISignable signable)
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
                var isGoogleProtobuf = x.PropertyType.FullName?.StartsWith("Google.Protobuf.") ?? false;

                return !isSignature && !isGoogleProtobuf;
            })
            .Select(x => new
            {
                x.Name,
                Value = x.GetValue(signable)
            })
            .OrderBy(x => x.Name)
            .Select(x => $"{x.Name}={x.Value}");

        var sb = new StringBuilder();
        sb.AppendJoin(Separator, stringPairs);
        if (sb.Length > 0)
        {
            sb.Append(Separator);
        }
        sb.Append(_signPackageSettings.Salt);

        var hashBytes = MD5.HashData(Encoding.Default.GetBytes(sb.ToString()));
        return Convert.ToHexString(hashBytes);
    }
}
