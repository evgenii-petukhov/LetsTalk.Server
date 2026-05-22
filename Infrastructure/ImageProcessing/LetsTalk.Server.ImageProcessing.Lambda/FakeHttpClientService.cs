using LetsTalk.Server.ImageProcessing.Utility.Abstractions;

namespace LetsTalk.Server.ImageProcessing.Lambda;

public class FakeHttpClientService() : IHttpClientService
{
    public HttpClient GetHttpClient()
    {
        return new HttpClient();
    }
}