using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace LetsTalk.Server.API.Core.Services;

public partial class IceServerConfigurationService(
    IOptions<CloudflareSettings> options,
    IHttpClientFactory httpClientFactory) : IIceServerConfigurationService
{
    private readonly CloudflareSettings _cloudflareSettings = options.Value;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<string> GetIceServerConfigurationAsync(CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient(nameof(IceServerConfigurationService));
        
        var url = $"https://rtc.live.cloudflare.com/v1/turn/keys/{_cloudflareSettings.TurnTokenId}/credentials/generate-ice-servers";
        
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_cloudflareSettings.ApiToken}");
        
        var json = JsonSerializer.Serialize(new
        {
            ttl = _cloudflareSettings.TokenTtl
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content, cancellationToken);

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
