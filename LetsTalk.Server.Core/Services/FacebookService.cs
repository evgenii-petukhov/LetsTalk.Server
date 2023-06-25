using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Core.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Models.Login;

namespace LetsTalk.Server.Core.Services;

public class FacebookService : IFacebookService
{
    private const string FACEBOOK_URL = "https://graph.facebook.com/";

    private readonly IAccountDataLayerService _accountDataLayerService;
    private readonly IAuthenticationClient _authenticationClient;
    private readonly AuthenticationSettings _authenticationSettings;

    public FacebookService(
        IAccountDataLayerService accountDataLayerService,
        IAuthenticationClient authenticationClient,
        IOptions<AuthenticationSettings> options)
    {
        _accountDataLayerService = accountDataLayerService;
        _authenticationClient = authenticationClient;
        _authenticationSettings = options.Value;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginServiceInput model, CancellationToken cancellationToken)
    {
        // verify access token with facebook API to authenticate
        using var client = new RestClient(FACEBOOK_URL);
        var request = new RestRequest($"{model.Id}?fields=id,email,name,first_name,last_name,picture.type(large)&access_token={model.AuthToken}");
        try
        {
            var response = await client.GetAsync(request, cancellationToken: cancellationToken);

            if (!response.IsSuccessful)
                throw new BadRequestException(response.ErrorMessage!);

            // get data from response and account from db
            var data = JsonConvert.DeserializeObject<FacebookResponse>(response.Content!)!;

            var accountId = await _accountDataLayerService.CreateOrUpdateAsync(
                data.Id!,
                AccountTypes.Facebook,
                data.FirstName,
                data.LastName,
                data.Email,
                data.Picture!.Data!.Url,
                cancellationToken);

            // generate jwt token to access secure routes on this API
            var token = await _authenticationClient.GenerateJwtTokenAsync(_authenticationSettings.Url!, accountId);

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
