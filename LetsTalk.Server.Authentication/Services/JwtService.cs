using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LetsTalk.Server.Authentication.Services;

public class JwtService : IJwtService
{
    private const string CLAIM_ID = "id";

    private readonly JwtSettings _jwtSettings;
    private readonly SigningCredentials _signingCredentials;
    private readonly SymmetricSecurityKey _symmetricSecurityKey;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key!);
        _symmetricSecurityKey = new SymmetricSecurityKey(key);
        _signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        if (string.IsNullOrEmpty(_jwtSettings.Key))
            throw new AppException("JWT secret not configured");
    }

    public string GenerateJwtToken(int accountId)
    {
        // generate token that is valid for 15 minutes
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(CLAIM_ID, accountId.ToString()) }),
            Expires = DateTime.UtcNow.AddYears(1),
            SigningCredentials = _signingCredentials
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public int? ValidateJwtToken(string? token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
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

            // return account id from JWT token if validation successful
            return int.Parse(jwtToken.Claims.First(x => x.Type.Equals(CLAIM_ID, StringComparison.Ordinal)).Value);
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }
}
