using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.Core.Exceptions;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Identity.Models;
using LetsTalk.Server.Models.Authentication;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace LetsTalk.Server.Identity.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly LetsTalkDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthenticationService(
        LetsTalkDbContext context,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginServiceResult> Login(LoginServiceInput model)
    {
        // verify access token with facebook API to authenticate
        var client = new RestClient("https://graph.facebook.com/");
        var request = new RestRequest($"{model.Id}?fields=id,email,name,first_name,last_name,picture.type(large)&access_token={model.AuthToken}");
        var response = await client.GetAsync(request);

        if (!response.IsSuccessful)
            throw new AppException(response.ErrorMessage!);

        // get data from response and account from db
        var data = JsonConvert.DeserializeObject<FacebookResponse>(response.Content!)!;
        string externalId = data.Id!;
        var account = _context.Accounts.SingleOrDefault(x => x.ExternalId == externalId);

        // create new account if first time logging in
        if (account == null)
        {
            account = new Account
            {
                ExternalId = externalId,
                AccountType = _context.AccountTypes.SingleOrDefault(x => x.Id == (int)AccountTypes.Facebook),
                Name = data.Name,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                PhotoUrl = data.Picture!.Data!.Url
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        // generate jwt token to access secure routes on this API
        var token = _jwtService.GenerateJwtToken(account.Id);

        return new LoginServiceResult();
    }
}

