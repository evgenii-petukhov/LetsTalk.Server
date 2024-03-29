﻿using LetsTalk.Server.LinkPreview.Abstractions;

namespace LetsTalk.Server.LinkPreview.Services;

public class DownloadService(IHttpClientFactory httpClientFactory) : IDownloadService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<string> DownloadAsStringAsync(string url)
    {
        using var client = _httpClientFactory.CreateClient(nameof(DownloadService));
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        return await client.GetStringAsync(url);
    }
}
