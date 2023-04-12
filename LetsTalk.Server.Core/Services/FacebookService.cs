using LetsTalk.Server.Domain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Core.Models;
using LetsTalk.Server.Persistence.Models;
using LetsTalk.Server.API.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Configuration.Models;

namespace LetsTalk.Server.Core.Services;

public class FacebookService : IFacebookService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAuthenticationClient _authenticationClient;
    private readonly AuthenticationSettings _authenticationSettings;

    public FacebookService(
        IAccountRepository accountRepository,
        IAuthenticationClient authenticationClient,
        IOptions<AuthenticationSettings> options)
    {
        _accountRepository = accountRepository;
        _authenticationClient = authenticationClient;
        _authenticationSettings = options.Value;
    }

    public async Task<LoginResponseDto> Login(LoginServiceInput model)
    {
        // verify access token with facebook API to authenticate
        var client = new RestClient("https://graph.facebook.com/");
        var request = new RestRequest($"{model.Id}?fields=id,email,name,first_name,last_name,picture.type(large)&access_token={model.AuthToken}");
        try
        {
            var response = await client.GetAsync(request);

            if (!response.IsSuccessful)
                throw new BadRequestException(response.ErrorMessage!);

            // get data from response and account from db
            var data = JsonConvert.DeserializeObject<FacebookResponse>(response.Content!)!;

            string externalId = data.Id!;
            var firstName = data.FirstName;
            var lastName = data.LastName;
            var photoUrl = data.Picture!.Data!.Url;
            var account = await _accountRepository.GetByExternalIdAsync(externalId);

            // create new account if first time logging in
            if (account == null)
            {
                account = new Account
                {
                    ExternalId = externalId,
                    AccountTypeId = (int)AccountTypes.Facebook,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = data.Email,
                    PhotoUrl = photoUrl
                };
                await _accountRepository.CreateAsync(account);
            }
            else
            {
                await _accountRepository.UpdateAsync(account.Id, firstName, lastName, photoUrl);
            }

            // generate jwt token to access secure routes on this API
            var token = await _authenticationClient.GenerateJwtToken(_authenticationSettings.Url!, account.Id);

            return new LoginResponseDto
            {
                Success = true,
                Token = token
            };
        }
        catch (HttpRequestException e)
        {
            throw new BadRequestException($"Facebook response: {e.Message}");
        }
        catch (Exception e)
        {
            throw new BadRequestException(e.Message);
        }
    }
}
