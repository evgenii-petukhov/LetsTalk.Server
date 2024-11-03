using LetsTalk.Server.LinkPreview.Utility.Abstractions;

namespace LetsTalk.Server.LinkPreview.Services;

public class HttpClientService(IHttpClientFactory httpClientFactory) : IHttpClientService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public HttpClient GetHttpClient()
    {
        return _httpClientFactory.CreateClient(nameof(HttpClientService));
    }
}
