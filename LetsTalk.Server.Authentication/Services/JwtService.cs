using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LetsTalk.Server.Authentication.Services;

public class JwtService : IJwtService, IDisposable
{
    private const string CLAIM_ID = "id";

    private readonly JwtSettings _jwtSettings;
    private readonly SigningCredentials _signingCredentials;
    private readonly SymmetricSecurityKey _symmetricSecurityKey;
    private readonly IMemoryCache _memoryCache;
    private bool _disposedValue;

    public JwtService(
        IMemoryCache memoryCache,
        IOptions<JwtSettings> jwtSettings)
    {
        _memoryCache = memoryCache;
        _jwtSettings = jwtSettings.Value;
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key!);
        _symmetricSecurityKey = new SymmetricSecurityKey(key);
        _signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        if (string.IsNullOrEmpty(_jwtSettings.Key))
        {
            throw new AppException("JWT secret not configured");
        }
    }

    public string GenerateJwtToken(int accountId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(CLAIM_ID, accountId.ToString()) }),
            Expires = DateTime.UtcNow.AddYears(1),
            SigningCredentials = _signingCredentials
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = (JwtSecurityToken)token;
        _memoryCache.Set(jwtToken.RawData, accountId, jwtToken.ValidTo);
        return tokenHandler.WriteToken(token);
    }

    public int? ValidateJwtToken(string? token)
    {
        return token == null
            ? null
            : _memoryCache.GetOrCreate(token, cacheEntry =>
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _symmetricSecurityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            cacheEntry.AbsoluteExpiration = jwtToken.ValidTo;
            return int.Parse(jwtToken.Claims.First(x => x.Type.Equals(CLAIM_ID, StringComparison.Ordinal)).Value);
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _memoryCache.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
