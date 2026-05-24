using LetsTalk.Server.LinkPreview.Utility.Abstractions;

namespace LetsTalk.Server.LinkPreview.Lambda;

public class FakeHttpClientService() : IHttpClientService
{
    public HttpClient GetHttpClient()
    {
        return new HttpClient();
    }
}
