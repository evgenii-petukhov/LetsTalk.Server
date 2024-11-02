using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.LinkPreview.Utility;

public static class LinkPreviewUtilityRegistration
{
    public static IServiceCollection AddLinkPreviewUtility(
        this IServiceCollection services)
    {
        services.AddHttpClient(nameof(DownloadService));
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<IRegexService, RegexService>();
        services.AddScoped<ILinkPreviewService, LinkPreviewService>();

        return services;
    }
}
