using Grpc.Net.Client;
using LetsTalk.Server.Authentication;
using static LetsTalk.Server.Authentication.JwtTokenGrpcService;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.AuthenticationClient;

public class AuthenticationClient: IAuthenticationClient, IDisposable
{
    private readonly HttpClientHandler _httpClientHandler;
    private readonly GrpcChannel _grpcChannel;
    private bool _disposedValue;

    public AuthenticationClient(IOptions<AuthenticationSettings> options)
    {
        _httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        _grpcChannel = GrpcChannel.ForAddress(options.Value.Url!, new GrpcChannelOptions
        {
            HttpHandler = _httpClientHandler
        });
    }

    public async Task<string> GenerateJwtTokenAsync(string url, int accountId)
    {
        var response = await new JwtTokenGrpcServiceClient(_grpcChannel).GenerateJwtTokenAsync(new GenerateJwtTokenRequest
        {
            AccountId = accountId
        });

        return response.Token;
    }

    public async Task<int?> ValidateJwtTokenAsync(string url, string? token)
    {
        var response = await new JwtTokenGrpcServiceClient(_grpcChannel).ValidateJwtTokenAsync(new ValidateJwtTokenRequest
        {
            Token = token
        });

        return response.AccountId;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _grpcChannel.Dispose();
                _httpClientHandler.Dispose();
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
