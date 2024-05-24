using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Attributes;
using LetsTalk.Server.Core.Constants;
using LetsTalk.Server.Core.Features.Authentication.Commands.Login;
using LetsTalk.Server.Core.Models.Authentication;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Newtonsoft.Json;
using RestSharp;

namespace LetsTalk.Server.Core.Services;

[OpenAuthProviderId(OpenAuthProviderIdentifiers.FACEBOOK)]
public class FacebookOpenAuthProvider(
    IAuthenticationClient authenticationClient,
    IAccountAgnosticService accountAgnosticService) : IOpenAuthProvider
{
    private const string FACEBOOK_URL = "https://graph.facebook.com/";

    private readonly IAuthenticationClient _authenticationClient = authenticationClient;
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;

    public async Task<LoginResponseDto> LoginAsync(LoginCommand model, CancellationToken cancellationToken)
    {
        // verify access token with facebook API to authenticate
        using var client = new RestClient(FACEBOOK_URL);
        var request = new RestRequest($"{model.Id}?fields=id,name,first_name,last_name,picture.type(large)&access_token={model.AuthToken}");
        try
        {
            var response = await client.GetAsync(request, cancellationToken);

            if (!response.IsSuccessful)
            {
                throw new BadRequestException(response.ErrorMessage!);
            }

            // get data from response and account from db
            var data = JsonConvert.DeserializeObject<FacebookResponse>(response.Content!)!;

            var accountId = await _accountAgnosticService.CreateOrUpdateAsync(
                data.Id!,
                AccountTypes.Facebook,
                data.FirstName!,
                data.LastName!,
                data.Picture!.Data!.Url!,
                cancellationToken);

            // generate jwt token to access secure routes on this API
            var token = await _authenticationClient.GenerateJwtTokenAsync(accountId);

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
