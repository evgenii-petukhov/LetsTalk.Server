using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Authentication.Models;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LetsTalk.Server.Authentication.Services;

public class JwtStorageService: IJwtStorageService
{
    private const string CLAIM_ID = "id";

    private readonly JwtSettings _jwtSettings;
    private readonly SymmetricSecurityKey _symmetricSecurityKey;
    private readonly SigningCredentials _signingCredentials;

    public JwtStorageService(
        IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key!);
        _symmetricSecurityKey = new SymmetricSecurityKey(key);
        _signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        if (string.IsNullOrEmpty(_jwtSettings.Key))
        {
            throw new AppException("JWT secret not configured");
        }
    }

    public async Task<StoredToken?> GetStoredTokenAsync(string? token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationResult = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _symmetricSecurityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            ClockSkew = TimeSpan.Zero
        });

        var jwtToken = (JwtSecurityToken)validationResult.SecurityToken;
        var accountId = int.Parse(jwtToken.Claims.First(x => x.Type.Equals(CLAIM_ID, StringComparison.Ordinal)).Value);

        return new StoredToken
        {
            AccountId = accountId,
            Token = token,
            ValidTo = jwtToken.ValidTo
        };
    }

    public Task<StoredToken> GenerateJwtToken(int accountId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(CLAIM_ID, accountId.ToString())
            }),
            Expires = DateTime.UtcNow.AddYears(1),
            SigningCredentials = _signingCredentials
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = (JwtSecurityToken)token;
        return Task.FromResult(new StoredToken
        {
            AccountId = accountId,
            Token = tokenHandler.WriteToken(token),
            ValidTo = jwtToken.ValidTo
        });
    }
}
